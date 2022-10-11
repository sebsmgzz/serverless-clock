namespace ServerlessTimers.Application.Functions.Clients;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using ServerlessTimers.Application.Services.Commands;

public class DeleteTimerFunction
{


    private readonly IMediator mediator;
    private readonly ILogger logger;

    public DeleteTimerFunction(
        IMediator mediator,
        ILogger<DeleteTimerFunction> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(DeleteTimerFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "delete" },
            Route = "timers/{id:guid}")] 
        HttpRequest request,
        Guid id)
    {
        try
        {

            // Execute command
            var alarmId = await mediator.Send(new DeleteTimerCommand()
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
