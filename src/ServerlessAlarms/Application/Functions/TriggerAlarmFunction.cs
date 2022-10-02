namespace ServerlessAlarm.Application.Functions;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Domain.Aggregators.Alarms;
using ServerlessAlarm.Application.Models.Inputs;
using ServerlessAlarm.Application.Models.EventsData;
using MediatR;
using ServerlessAlarm.Domain.Events;

public class TriggerAlarmFunction
{

    private readonly IAlarmRepository alarmsRepository;
    private readonly IMediator mediator;
    private readonly ILogger logger;
    private readonly CancellationTokenSource cts;

    public TriggerAlarmFunction(
        IAlarmRepository alarmsRepository,
        IMediator mediator,
        ILogger<TriggerAlarmFunction> logger)
    {
        this.alarmsRepository = alarmsRepository;
        this.mediator = mediator;
        this.logger = logger;
        cts = new CancellationTokenSource();
    }

    [FunctionName(nameof(TriggerAlarmFunction))]
    public async Task Run(
        [OrchestrationTrigger]
        IDurableOrchestrationContext context)
    {
        try
        {

            // Get input
            var args = context.GetInput<TriggerAlarmInput>();
            var alarm = await alarmsRepository.FindByIdAsync(args.AlarmId);
            if (alarm == null)
            {
                logger.LogCritical($"Alarm {args.AlarmId}: Scheduled but not found in the repository.");
                return;
            }

            // Build list of awaitable external events
            var externalEventTasks = new List<Task<ExternalEvent>>();
            if (alarm.SnoozePolicy != null && args.Snoozes < alarm.SnoozePolicy.Repeat)
            {
                var snoozeEventTask = context.WaitForExternalEvent<ExternalEvent>(
                    name: nameof(ExternalEvent.Snooze),
                    defaultValue: ExternalEvent.Timeout,
                    timeout: alarm.Timeout,
                    cancelToken: cts.Token);
                externalEventTasks.Add(snoozeEventTask);
            }
            var dismissEventTask = context.WaitForExternalEvent<ExternalEvent>(
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
                logger.LogInformation($"Alarm {args.AlarmId}: Dismissed");
                await mediator.Publish(new AlarmDismissedEvent()
                {
                    AlarmId = alarm.Id
                });
            }
            else if (achievedTask.Result == ExternalEvent.Snooze)
            {
                logger.LogInformation($"Alarm {args.AlarmId}: Snoozed");
                args.Snoozes++;
                await mediator.Publish(new AlarmSnoozedEvent()
                {
                    AlarmId = alarm.Id
                });
                context.ContinueAsNew(args, false);
            }
            else
            {
                logger.LogInformation($"Alarm {args.AlarmId}: Timedout");
                await mediator.Publish(new AlarmTimedoutEvent()
                {
                    AlarmId = alarm.Id
                });
            }

        }
        catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

}
