namespace ServerlessAlarm.Application.Functions;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Text.Json;
using System;
using Application.Models.Dtos;
using Application.Services.Commands;
using MediatR;
using ServerlessAlarm.Application.Models.ExternalEvents;

public class SnoozeAlarmFunction
{

    private readonly IMediator mediator;
    private readonly ILogger logger;

    public SnoozeAlarmFunction(
        IMediator mediator,
        ILogger<SnoozeAlarmFunction> logger)
    {
        this.logger = logger;
        this.mediator = mediator;
    }

    [FunctionName(nameof(SnoozeAlarmFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "post" },
            Route = "alarms/{id:guid}/snoozes")]
        HttpRequest request,
        Guid id,
        [DurableClient]
        IDurableOrchestrationClient durableClient)
    {
        try
        {

            // Call durable external event
            await durableClient.RaiseEventAsync(
                instanceId: id.ToString(),
                eventName: OnAlarmTriggeredEvent.AlarmSnoozed.Name,
                eventData: OnAlarmTriggeredEvent.AlarmSnoozed);

            // Execute command
            await mediator.Send(new SnoozeAlarmCommand()
            {
                AlarmId = id
            });

            // Return nothing
            return new NoContentResult();

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return new StatusCodeResult(500);
        }
    }

}
