namespace ServerlessAlarm.Application.Functions.Clients;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Models.Dtos;
using Application.Services.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ServerlessAlarm.Application.Exceptions;

public class UpdateAlarmFunction
{

    private readonly IMediator mediator;
    private readonly ILogger<UpdateAlarmFunction> logger;

    public UpdateAlarmFunction(
        IMediator mediator,
        ILogger<UpdateAlarmFunction> logger)
    {
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(UpdateAlarmFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "put" },
            Route = "alarms/{id:guid}")]
        HttpRequest request,
        Guid id)
    {
        try
        {

            // Deserialize input body
            var dto = JsonSerializer.Deserialize<UpdateAlarmDto>(request.Body);

            // Execute command
            await mediator.Send(new UpdateAlarmCommand()
            {
                AlarmId = id,
                AlarmName = dto.Name,
                AlarmRecurrence = dto.Recurrence,
                AlarmTimeout = dto.Timeout,
                SnoozeInterval = dto.SnoozePolicy?.Interval,
                SnoozeRepeat = dto.SnoozePolicy?.Repeat
            });

            // Return nothing
            return new NoContentResult();

        }
        catch (AlarmNotFoundException ex)
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
