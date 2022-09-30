namespace ServerlessAlarm.Application.Functions;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Models.Dtos;
using Application.Services.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

public class UpdateAlarmFunction
{

    private readonly IMediator mediator;
    private readonly ILogger<UpdateAlarmFunction> logger;

    public UpdateAlarmFunction(
        ILogger<UpdateAlarmFunction> logger)
    {
        this.logger = logger;
    }

    [FunctionName(nameof(UpdateAlarmFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "put" },
            Route = "alarms/{id:guid}")]
        HttpRequest request,
        Guid id,
        [DurableClient]
        IDurableOrchestrationClient durableClient)
    {
        try
        {

            // Deserialize payload
            var dto = JsonSerializer.Deserialize<UpdateAlarmDto>(request.Body);

            // Execute command
            await mediator.Send(new UpdateAlarmCommand()
            {
                AlarmId = id,
                AlarmName = dto.Name,
                AlarmRecurrence = dto.Recurrence,
                AlarmTimeout = dto.Timeout,
                SnoozeInterval = dto.SnoozePolicy.Interval,
                SnoozeRepeat = dto.SnoozePolicy.Repeat
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
