namespace ServerlessAlarm.Application.Functions.Durable;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Domain.Aggregators.Alarms;
using Application.Models.Durable;
using NCrontab;
using MediatR;
using ServerlessAlarm.Domain.Events;
using System.Collections.Generic;

public class OrchestrateAlarmFunction
{

    private readonly IAlarmRepository alarmsRepository;
    private readonly IMediator mediator;
    private readonly ILogger logger;
    private readonly CancellationTokenSource cts;

    public OrchestrateAlarmFunction(
        IAlarmRepository alarmsRepository,
        IMediator mediator,
        ILogger<OrchestrateAlarmFunction> logger)
    {
        this.alarmsRepository = alarmsRepository;
        this.mediator = mediator;
        this.logger = logger;
        cts = new CancellationTokenSource();
    }

    [FunctionName(nameof(OrchestrateAlarmFunction))]
    public async Task Run(
        [OrchestrationTrigger]
        IDurableOrchestrationContext context)
    {
        try
        {

            // Get input
            var input = context.GetInput<OrchestrateAlarmInput>();
            var alarm = await alarmsRepository.FindByIdAsync(input.AlarmId);

            // Schedule the alarm
            var recurrence = CrontabSchedule.Parse(alarm.Recurrence);
            var nextOcurrence = recurrence.GetNextOccurrence(DateTime.Now);
            logger.LogInformation($"Alarm {alarm.Id}: Scheduled to trigger at {nextOcurrence}");

            // Await for trigger alarm
            await context.CreateTimer(nextOcurrence, cts.Token);
            logger.LogInformation($"Alarm {alarm.Id}: Triggered");

            // Build list of awaitable external events
            var externalEventTasks = new List<Task<ExternalEvent>>();
            if (alarm.SnoozePolicy != null && input.Snoozes < alarm.SnoozePolicy.Repeat)
            {
                var snoozeEventTask = context.WaitForExternalEvent(
                    name: nameof(ExternalEvent.Snooze),
                    defaultValue: ExternalEvent.Timeout,
                    timeout: alarm.Timeout,
                    cancelToken: cts.Token);
                externalEventTasks.Add(snoozeEventTask);
            }
            var dismissEventTask = context.WaitForExternalEvent(
                    name: nameof(ExternalEvent.Dismissed),
                    defaultValue: ExternalEvent.Timeout,
                    timeout: alarm.Timeout,
                    cancelToken: cts.Token);
            externalEventTasks.Add(dismissEventTask);

            // Await for any external event (or timeout)
            logger.LogInformation($"Alarm {alarm.Id}: Waiting for external events");
            var achievedTask = await Task.WhenAny(externalEventTasks);
            if (achievedTask.Result == ExternalEvent.Dismissed)
            {
                logger.LogInformation($"Alarm {input.AlarmId}: Dismissed");
                await mediator.Publish(new AlarmDismissedEvent()
                {
                    AlarmId = alarm.Id
                });
            }
            else if (achievedTask.Result == ExternalEvent.Snooze)
            {
                logger.LogInformation($"Alarm {input.AlarmId}: Snoozed");
                input.Snoozes++;
                await mediator.Publish(new AlarmSnoozedEvent()
                {
                    AlarmId = alarm.Id
                });
                context.ContinueAsNew(input, false);
            }
            else
            {
                logger.LogInformation($"Alarm {input.AlarmId}: Timedout");
                await mediator.Publish(new AlarmTimedoutEvent()
                {
                    AlarmId = alarm.Id
                });
            }

            // Execute domain event
            await mediator.Publish(new AlarmTriggeredEvent()
            {
                AlarmId = alarm.Id
            });

            // Durable loop
            context.ContinueAsNew(input, false);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

}
