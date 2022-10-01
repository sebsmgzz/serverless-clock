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
using ServerlessAlarm.Domain.Aggregators.Alarms;

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
                SnoozeInterval = dto.SnoozePolicy?.Interval,
                SnoozeRepeat = dto.SnoozePolicy?.Repeat
            });

            // Restart durable function
            var queryResult = await durableClient.ListInstancesAsync(
                new OrchestrationStatusQueryCondition(), default);
            var durable = queryResult.DurableOrchestrationState.FirstOrDefault(
                predicate: durable => durable.InstanceId == id.ToString(),
                defaultValue: null);
            if(durable != null)
            {
                await durableClient.TerminateAsync(
                    instanceId: id.ToString(),
                    reason: "Updated");
                await durableClient.RestartAsync(
                    instanceId: id.ToString(),
                    restartWithNewInstanceId: false);
            }

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
