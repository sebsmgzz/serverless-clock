namespace ServerlessAlarm.Application.Functions;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Domain.Aggregators.Alarms;
using Application.Models.Inputs;
using NCrontab;
using MediatR;
using ServerlessAlarm.Domain.Events;

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
            var input = context.GetInput<ScheduleAlarmInput>();
            var alarm = await alarmsRepository.FindByIdAsync(input.AlarmId);

            // Schedule the alarm
            var recurrence = CrontabSchedule.Parse(alarm.Recurrence);
            var nextOcurrence = recurrence.GetNextOccurrence(DateTime.UtcNow);
            logger.LogInformation($"Alarm {alarm.Id}: Scheduled to trigger at {nextOcurrence}");
            await context.CreateTimer(nextOcurrence, cts.Token);

            // Trigger alarm
            var instanceId = context.StartNewOrchestration(
                functionName: nameof(TriggerAlarmFunction),
                instanceId: alarm.Id.ToString(),
                input: new TriggerAlarmInput()
                {
                    AlarmId = alarm.Id,
                    Snoozes = 0
                });
            logger.LogInformation($"Alarm {alarm.Id}: Triggered with {instanceId}");
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
