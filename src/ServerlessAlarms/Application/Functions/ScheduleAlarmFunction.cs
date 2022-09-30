namespace ServerlessAlarm.Application.Functions;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Domain.Aggregators.Alarms;
using Application.Models.Inputs;
using Application.Models.ExternalEvents;
using NCrontab;

public class ScheduleAlarmFunction
{

    private readonly IAlarmRepository alarmsRepository;
    private readonly ILogger logger;
    private readonly CancellationTokenSource cts;

    public ScheduleAlarmFunction(
        IAlarmRepository alarmsRepository,
        ILogger<ScheduleAlarmFunction> logger)
    {
        this.alarmsRepository = alarmsRepository;
        this.logger = logger;
        cts = new CancellationTokenSource();
    }

    [FunctionName(nameof(ScheduleAlarmFunction))]
    public async Task Run(
        [OrchestrationTrigger]
        IDurableOrchestrationContext context)
    {
        try
        {

            // Get input
            var input = context.GetInput<ScheduleAlarmInput>();
            var alarm = await alarmsRepository.FindByIdAsync(input.AlarmId);

            // Schedule the alarm
            var recurrence = CrontabSchedule.Parse(alarm.Recurrence);
            var nextOcurrence = recurrence.GetNextOccurrence(DateTime.UtcNow);
            logger.LogInformation($"Alarm {alarm.Id}: Scheduled to trigger at {nextOcurrence}");
            await context.CreateTimer(nextOcurrence, cts.Token);

            // Trigger alarm
            await context.CallSubOrchestratorAsync(
                functionName: nameof(TriggerAlarmFunction),
                input: new TriggerAlarmInput()
                {
                    AlarmId = alarm.Id,
                    Snoozes = 0
                });
            // TODO: Raise alarm triggered alert

            // Durable loop
            context.ContinueAsNew(input, false);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

}
public class TriggerAlarmFunction
{

    private readonly IAlarmRepository alarmsRepository;
    private readonly ILogger logger;
    private readonly CancellationTokenSource cts;

    public TriggerAlarmFunction(
        IAlarmRepository alarmsRepository,
        ILogger<TriggerAlarmFunction> logger)
    {
        this.alarmsRepository = alarmsRepository;
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
            }

            // Build list of awaitable external events
            var externalEventTasks = new List<Task<OnAlarmTriggeredEvent>>();
            if (alarm.SnoozePolicy != null && args.Snoozes < alarm.SnoozePolicy.Repeat)
            {
                var snoozeEventTask = context.WaitForExternalEvent<OnAlarmTriggeredEvent>(
                    name: OnAlarmTriggeredEvent.AlarmSnoozed.Name,
                    defaultValue: OnAlarmTriggeredEvent.AlarmTimedout,
                    timeout: alarm.Timeout,
                    cancelToken: cts.Token);
                externalEventTasks.Add(snoozeEventTask);
            }
            var dismissEventTask = context.WaitForExternalEvent<OnAlarmTriggeredEvent>(
                    name: OnAlarmTriggeredEvent.AlarmDismissed.Name,
                    defaultValue: OnAlarmTriggeredEvent.AlarmTimedout,
                    timeout: alarm.Timeout,
                    cancelToken: cts.Token);
            externalEventTasks.Add(dismissEventTask);

            // Await for any external event (or timeout)
            logger.LogInformation($"Alarm {alarm.Id}: Waiting for external events");
            var achievedTask = await Task.WhenAny(externalEventTasks);
            if (achievedTask.Result == OnAlarmTriggeredEvent.AlarmDismissed)
            {
                logger.LogInformation($"Alarm {args.AlarmId}: Dismissed");
            }
            else if (achievedTask.Result == OnAlarmTriggeredEvent.AlarmSnoozed)
            {
                // Recursive call trigger and increase counter
                logger.LogInformation($"Alarm {args.AlarmId}: Snoozed");
                args.Snoozes++;
                context.ContinueAsNew(args, false);
            }
            else
            {
                logger.LogInformation($"Alarm {args.AlarmId}: Timedout");
                // TODO: Raise alarm timedout alert
            }

        }
        catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

}
public class TriggerAlarmInput
{

    public Guid AlarmId { get; set; }

    public int Snoozes { get; set; }

}