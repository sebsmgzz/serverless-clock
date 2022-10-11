namespace ServerlessTimers.Application.Functions.Clients;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using ServerlessTimers.Application.Models.Dtos;
using ServerlessTimers.Application.Services.Commands;

public class CreateTimerFunction
{

    private readonly IMediator mediator;
    private readonly ILogger logger;

    public CreateTimerFunction(
        IMediator mediator,
        ILogger<CreateTimerFunction> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(CreateTimerFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "post" },
            Route = "timers")] HttpRequest request)
    {
        try
        {

            // Deserialize payload
            var dto = JsonSerializer.Deserialize<CreateTimerDto>(request.Body);
            var timer = dto.ToTimer();

            // Execute command
            var timerId = await mediator.Send(new CreateTimerCommand()
            {
                Timer = timer
            });

            // Return timers's id
            return new OkObjectResult(new
            {
                id = timerId
            });

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return new StatusCodeResult(500);
        }
    }

}
