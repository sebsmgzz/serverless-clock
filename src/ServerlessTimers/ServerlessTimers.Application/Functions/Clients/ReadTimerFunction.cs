namespace ServerlessTimers.Application.Functions.Clients;

using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using ServerlessTimers.Application.Models.Dtos;
using ServerlessTimers.Application.Services.Queries;

public class ReadTimerFunction
{


    private readonly IMediator mediator;
    private readonly ILogger logger;

    public ReadTimerFunction(
        IMediator mediator,
        ILogger<ReadTimerFunction> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(ReadTimerFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "get" },
            Route = "timers/{id:guid}")] 
        HttpRequest request,
        Guid id)
    {
        try
        {

            // Execute query
            var timer = await mediator.Send(new ReadTimerQuery()
            {
                TimerId = id
            });

            // Return timers's dto
            return new OkObjectResult(ReadTimerDto.FromTimer(timer));

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return new StatusCodeResult(500);
        }
    }

}
