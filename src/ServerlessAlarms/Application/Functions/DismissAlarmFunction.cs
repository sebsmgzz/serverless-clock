namespace ServerlessAlarm.Application.Functions;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using MediatR;
using Application.Services.Commands;
using ServerlessAlarm.Application.Models.EventsData;
using ServerlessAlarm.Application.Models.Dtos;
using System.Text.Json;
using ServerlessAlarm.Application.Services.Durable;
using ServerlessAlarm.Application.Services.Queries;

public class DismissAlarmFunction
{

    private readonly IDurableFacadeFactory durableFactory;
    private readonly IMediator mediator;
    private readonly ILogger logger;

    public DismissAlarmFunction(
        IDurableFacadeFactory durableFactory,
        IMediator mediator,
        ILogger<DismissAlarmFunction> logger)
    {
        this.durableFactory = durableFactory;
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(DismissAlarmFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "post" },
            Route = "events/dismiss")]
        HttpRequest request,
        [DurableClient]
        IDurableOrchestrationClient durableClient)
    {
        try
        {

            // Deserialize payload
            var dto = JsonSerializer.Deserialize<DismissAlarmDto>(request.Body);

            // Get alarm
            var alarm = await mediator.Send(new ReadAlarmCommand()
            {
                AlarmId = dto.AlarmId
            });

            // Raise dismiss event
            var durableFacade = durableFactory.GetFacade(durableClient);
            await durableFacade.DismissAlarmAsync(alarm);

            // Execute command
            await mediator.Send(new DismissAlarmCommand()
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
