namespace ServerlessTimers.Application.Functions.Events;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using ServerlessTimers.Application.Services.Commands;
using Microsoft.AspNetCore.Hosting;
using System.Web.Http;

public class StartTimerFunction
{

    private readonly IMediator mediator;
    private readonly ILogger logger;

    public StartTimerFunction(
        IMediator mediator,
        ILogger<StartTimerFunction> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(StartTimerFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "post" },
            Route = "timers/{id:guid}/events/start")] 
        HttpRequest request,
        Guid id)
    {
        try
        {

            // Execute command
            await mediator.Send(new StartTimerCommand()
            {
                TimerId = id
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
