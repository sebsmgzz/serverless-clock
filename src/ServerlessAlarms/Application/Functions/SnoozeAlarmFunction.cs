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
using ServerlessAlarm.Application.Services.Queries;

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
            Route = "events/snoozes")]
        HttpRequest request,
        [DurableClient]
        IDurableOrchestrationClient durableClient)
    {
        try
        {

            // Deserialize input
            var dto = JsonSerializer.Deserialize<SnoozeAlarmDto>(request.Body);

            // Execute command
            await mediator.Send(new SnoozeAlarmCommand()
            {
                AlarmId = dto.AlarmId
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
