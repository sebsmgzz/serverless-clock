namespace ServerlessTimers.Application.Functions.Events;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ServerlessTimers.Application.Models.Dtos;
using ServerlessTimers.Application.Services.Commands;
using System.Threading.Tasks;
using System;
using System.Text.Json;

public class StopTimerFunction
{

    private readonly IMediator mediator;
    private readonly ILogger logger;

    public StopTimerFunction(
        IMediator mediator,
        ILogger<StopTimerFunction> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(StopTimerFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "post" },
            Route = "timers/{id:guid}/events/stop")] 
        HttpRequest request,
        Guid id)
    {
        try
        {

            // Execute command
            await mediator.Send(new StopTimerCommand()
            {
                TimerId = id
            }); ;

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
