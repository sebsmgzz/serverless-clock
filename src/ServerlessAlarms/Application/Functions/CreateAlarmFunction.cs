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

public class CreateAlarmFunction
{

    private readonly IMediator mediator;
    private readonly ILogger<CreateAlarmFunction> logger;

    public CreateAlarmFunction(
        IMediator mediator,
        ILogger<CreateAlarmFunction> logger)
    {
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
            var instanceId = await durableClient.StartNewAsync(
                orchestratorFunctionName: nameof(ScheduleAlarmFunction),
                input: new ScheduleAlarmInput()
                {
                    AlarmId = alarmId,
                });
            logger.LogInformation($"Alarm {alarmId}: Scheduled with {instanceId}");

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
