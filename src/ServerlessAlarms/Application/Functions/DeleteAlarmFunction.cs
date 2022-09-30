namespace ServerlessAlarm.Application.Functions;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Application.Models.Dtos;
using MediatR;
using Application.Services.Commands;

public class DeleteAlarmFunction
{

    private readonly IMediator mediator;
    private readonly ILogger<DeleteAlarmFunction> logger;

    public DeleteAlarmFunction(
        IMediator mediator,
        ILogger<DeleteAlarmFunction> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(DeleteAlarmFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "delete" },
            Route = "alarms/{id:guid}")]
        HttpRequest request,
        Guid id,
        [DurableClient]
        IDurableOrchestrationClient durableClient)
    {
        try
        {

            // Terminate durable function
            await durableClient.PurgeInstanceHistoryAsync(id.ToString());
            await durableClient.TerminateAsync(
                instanceId: id.ToString(),
                reason: "Alarm deleted");

            // Execute command
            await mediator.Send(new DeleteAlarmCommand()
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
