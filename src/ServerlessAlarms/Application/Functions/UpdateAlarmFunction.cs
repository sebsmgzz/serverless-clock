namespace ServerlessAlarm.Application.Functions;

using System;
using System.Linq;
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
using ServerlessAlarm.Application.Exceptions;
using ServerlessAlarm.Application.Services.Durable;
using ServerlessAlarm.Application.Services.Queries;

public class UpdateAlarmFunction
{

    private readonly IDurableFacadeFactory durableFactory;
    private readonly IMediator mediator;
    private readonly ILogger<UpdateAlarmFunction> logger;

    public UpdateAlarmFunction(
        IDurableFacadeFactory durableFactory,
        IMediator mediator,
        ILogger<UpdateAlarmFunction> logger)
    {
        this.durableFactory = durableFactory;
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
        Guid id,
        [DurableClient]
        IDurableOrchestrationClient durableClient)
    {
        try
        {

            // Get alarm
            var dto = JsonSerializer.Deserialize<UpdateAlarmDto>(request.Body);
            var alarm = await mediator.Send(new ReadAlarmCommand()
            {
                AlarmId = id
            });

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

            // Restart durable function
            var durableFacade = durableFactory.GetFacade(durableClient);
            await durableFacade.RestartAlarmAsync(alarm);

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
