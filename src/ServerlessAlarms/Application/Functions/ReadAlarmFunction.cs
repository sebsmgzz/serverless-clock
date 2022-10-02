namespace ServerlessAlarm.Application.Functions;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Application.Models.Dtos;
using MediatR;
using ServerlessAlarm.Application.Exceptions;
using ServerlessAlarm.Application.Services.Queries;

public class ReadAlarmFunction
{

    private readonly IMediator mediator;
    private readonly ILogger logger;

    public ReadAlarmFunction(
        IMediator mediator,
        ILogger<ReadAlarmFunction> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(ReadAlarmFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "get" },
            Route = "alarms/{id}")] 
        HttpRequest request,
        Guid id)
    {
        try
        {

            // Execute command
            var alarm = await mediator.Send(new ReadAlarmCommand()
            {
                AlarmId = id
            });

            // Return alarm's id
            return new OkObjectResult(ReadAlarmDto.FromAlarm(alarm));

        }
        catch(AlarmNotFoundException ex)
        {
            return new NotFoundObjectResult(new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return new StatusCodeResult(500);
        }
    }

}
