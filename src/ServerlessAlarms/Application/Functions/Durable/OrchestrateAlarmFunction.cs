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
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using System.Security.Claims;

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

            // Await for alarm's trigger
            await context.CreateTimer(nextOcurrence, cts.Token);
            logger.LogInformation($"Alarm {alarm.Id}: Triggered");
            await mediator.Publish(new AlarmTriggeredEvent()
            {
                AlarmId = alarm.Id
            });

            // Await for any external event (or timeout)
            logger.LogInformation($"Alarm {alarm.Id}: Waiting for external events");
            var externalEventTasks = GetExternalEventsTasks(context, alarm, input.Snoozes);
            var achievedTask = await Task.WhenAny(externalEventTasks);

            // Handle external event
            await HandleExternalEventAsync(
                context, achievedTask.Result, alarm, input);

            // Durable loop
            context.ContinueAsNew(input, false);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

    private IEnumerable<Task<ExternalEvent>> GetExternalEventsTasks(
        IDurableOrchestrationContext context, 
        Alarm alarm, 
        int snoozesPerformed = 0)
    {
        yield return context.WaitForExternalEvent(
            name: nameof(ExternalEvent.Dismissed),
            defaultValue: ExternalEvent.Timeout,
            timeout: alarm.Timeout,
            cancelToken: cts.Token);
        if (alarm.SnoozePolicy != null && snoozesPerformed < alarm.SnoozePolicy.Repeat)
        {
            yield return context.WaitForExternalEvent(
                name: nameof(ExternalEvent.Snooze),
                defaultValue: ExternalEvent.Timeout,
                timeout: alarm.Timeout,
                cancelToken: cts.Token);
        }
    }

    public async Task HandleExternalEventAsync(
        IDurableOrchestrationContext context, 
        ExternalEvent externalEvent, 
        Alarm alarm,
        OrchestrateAlarmInput input)
    {
        logger.LogInformation($"Alarm {alarm.Id}: {externalEvent}");
        if (externalEvent == ExternalEvent.Dismissed)
        {
            await mediator.Publish(new AlarmDismissedEvent()
            {
                AlarmId = alarm.Id
            });
        }
        else if (externalEvent == ExternalEvent.Snooze)
        {
            // TODO: Increase snoozes
            await mediator.Publish(new AlarmSnoozedEvent()
            {
                AlarmId = alarm.Id
            });
            context.ContinueAsNew(input, false);
        }
        else
        {
            await mediator.Publish(new AlarmTimedoutEvent()
            {
                AlarmId = alarm.Id
            });
        }
    }

}
