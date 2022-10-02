namespace ServerlessAlarm.Application.Functions;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Application.Models.Dtos;
using MediatR;
using Application.Services.Commands;
using ServerlessAlarm.Application.Models.Inputs;
using ServerlessAlarm.Application.Services.Durable;

public class CreateAlarmFunction
{

    private readonly IDurableFacadeFactory durableFactory;
    private readonly IMediator mediator;
    private readonly ILogger logger;

    public CreateAlarmFunction(
        IDurableFacadeFactory durableFactory,
        IMediator mediator,
        ILogger<CreateAlarmFunction> logger)
    {
        this.durableFactory = durableFactory;
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(CreateAlarmFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "post" },
            Route = "alarms")] HttpRequest request,
        [DurableClient] IDurableOrchestrationClient durableClient)
    {
        try
        {

            // Deserialize payload
            var dto = JsonSerializer.Deserialize<CreateAlarmDto>(request.Body);
            var alarm = dto.ToAlarm();

            // Execute command
            var alarmId = await mediator.Send(new CreateAlarmCommand()
            {
                Alarm = alarm
            });

            // Call the alarm scheduler function
            var durableFacade = durableFactory.GetFacade(durableClient);
            await durableFacade.ActivateAlarmAsync(alarm);

            // Return alarm's id
            return new OkObjectResult(new 
            { 
                id = alarmId
            });

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return new StatusCodeResult(500);
        }
    }

}
