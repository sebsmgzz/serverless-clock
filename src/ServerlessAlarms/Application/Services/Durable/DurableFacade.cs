﻿namespace ServerlessAlarm.Application.Services.Durable;

using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using ServerlessAlarm.Application.Functions;
using ServerlessAlarm.Application.Models.EventsData;
using ServerlessAlarm.Application.Models.Inputs;
using ServerlessAlarm.Domain.Aggregators.Alarms;
using System.Linq;
using System.Threading.Tasks;

public class DurableFacade : IDurableFacade
{

    private readonly IDurableOrchestrationClient client;
    private readonly ILogger logger;

    public DurableFacade(
        IDurableOrchestrationClient client,
        ILogger<IDurableFacade> logger)
    {
        this.client = client;
        this.logger = logger;
    }

    public async Task ActivateAlarmAsync(Alarm alarm)
    {
        logger.LogInformation($"Alarm {alarm.Id}: Activated with durable {alarm.Id}");
        await client.StartNewAsync(
            orchestratorFunctionName: nameof(OrchestrateAlarmFunction),
            instanceId: alarm.Id.ToString(),
            input: new ScheduleAlarmInput()
            {
                AlarmId = alarm.Id,
            });
    }

    public async Task DeactivateAlarmAsync(Alarm alarm)
    {
        logger.LogInformation($"Alarm {alarm.Id}: Deactivated");
        await client.PurgeInstanceHistoryAsync(alarm.Id.ToString());
        await client.TerminateAsync(
            instanceId: alarm.Id.ToString(),
            reason: "Alarm deleted");
    }

    public async Task<bool> RestartAlarmAsync(Alarm alarm)
    {
        var queryResult = await client.ListInstancesAsync(
            new OrchestrationStatusQueryCondition(), default);
        var durable = queryResult.DurableOrchestrationState.FirstOrDefault(
            predicate: durable => durable.InstanceId == alarm.Id.ToString(),
            defaultValue: null);
        if (durable == null)
        {
            return false;
        }
        await client.TerminateAsync(
            instanceId: alarm.Id.ToString(),
            reason: "Updated");
        await client.RestartAsync(
            instanceId: alarm.Id.ToString(),
            restartWithNewInstanceId: false);
        return true;
    }

    public async Task SnoozeAlarmAsync(Alarm alarm)
    {
        logger.LogInformation($"Alarm {alarm.Id}: Snoozed");
        await client.RaiseEventAsync(
            instanceId: alarm.Id.ToString(),
            eventName: nameof(ExternalEvent.Snooze),
            eventData: ExternalEvent.Snooze);
    }

    public async Task DismissAlarmAsync(Alarm alarm)
    {
        logger.LogInformation($"Alarm {alarm.Id}: Dismissed");
        await client.RaiseEventAsync(
            instanceId: alarm.Id.ToString(),
            eventName: nameof(ExternalEvent.Dismissed),
            eventData: ExternalEvent.Dismissed);
    }

}
