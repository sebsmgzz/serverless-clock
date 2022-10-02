namespace ServerlessAlarm.Application.Functions;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MediatR;
using Application.Services.Commands;
using ServerlessAlarm.Application.Services.Durable;
using ServerlessAlarm.Application.Services.Queries;

public class DeleteAlarmFunction
{

    private readonly IDurableFacadeFactory durableFactory;
    private readonly IMediator mediator;
    private readonly ILogger<DeleteAlarmFunction> logger;

    public DeleteAlarmFunction(
        IDurableFacadeFactory durableFactory,
        IMediator mediator,
        ILogger<DeleteAlarmFunction> logger)
    {
        this.durableFactory = durableFactory;
        this.mediator = mediator;
        this.logger = logger;
    }

    [FunctionName(nameof(DeleteAlarmFunction))]
    public async Task<IActionResult> Run(
        [HttpTrigger(
            authLevel: AuthorizationLevel.Function,
            methods: new string[] { "delete" },
            Route = "alarms/{id:guid}")]
        HttpRequest request,
        Guid id,
        [DurableClient]
        IDurableOrchestrationClient durableClient)
    {
        try
        {

            // Get alarm
            var alarm = await mediator.Send(new ReadAlarmCommand()
            {
                AlarmId = id
            });

            // Terminate durable function
            var durableFacade = durableFactory.GetFacade(durableClient);
            await durableFacade.DeactivateAlarmAsync(alarm);

            // Delete alarm
            await mediator.Send(new DeleteAlarmCommand()
            {
                AlarmId = id
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
