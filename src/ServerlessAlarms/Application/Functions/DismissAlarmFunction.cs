namespace ServerlessAlarm.Application.Functions;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using MediatR;
using Application.Services.Commands;
using ServerlessAlarm.Application.Models.EventsData;

public class DismissAlarmFunction
{

    private readonly IMediator mediator;
    private readonly ILogger logger;

    public DismissAlarmFunction(
        IMediator mediator,
        ILogger<DismissAlarmFunction> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(DismissAlarmFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "post" },
            Route = "alarms/{id:guid}")]
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
                eventName: nameof(ExternalEvent.Dismissed),
                eventData: ExternalEvent.Dismissed);

            // Execute command
            await mediator.Send(new DismissAlarmCommand()
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
