namespace ServerlessTimers.Application.Services.Durables;

using MediatR;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using ServerlessTimers.Application.Functions.Durables;
using ServerlessTimers.Application.Models.DurableEvents;
using ServerlessTimers.Application.Models.Durables;
using ServerlessTimers.Domain.Aggregators.Timers;
using ServerlessTimers.Domain.Events;
using ServerlessTimers.Domain.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class DurableFacade : IDurableFacade
{

    private readonly IDurableClient client;
    private readonly IMediator mediator;
    private readonly ITimerCalculatorFactory calculatorFactory;
    private readonly ILogger logger;

    public DurableFacade(
        IDurableClient client,
        IMediator mediator,
        ITimerCalculatorFactory calculatorFactory,
        ILogger<IDurableFacade> logger)
    {
        this.client = client;
        this.mediator = mediator;
        this.calculatorFactory = calculatorFactory;
        this.logger = logger;
    }

    public async Task StartTimerAsync(
        ServerlessTimer timer,
        CancellationToken cancellationToken = default)
    {

        // Start a new orchestration
        // TODO: What if the orchestration is already started?
        // NOTE: Started orchestrators is using the same id (results in an exception)
        client.StartNewAsync(
            orchestratorFunctionName: nameof(OrchestrateTimerFunction),
            instanceId: timer.Id.ToString(),
            input: new TimerOrchestratorInput()
            {
                TimerId = timer.Id
            })
            .Wait(cancellationToken);

        // Publish the event
        await mediator.Publish(
            cancellationToken: cancellationToken,
            notification: new TimerStartedEvent()
            {
                TimerId = timer.Id
            });

    }

    public async Task StopTimerAsync(
        ServerlessTimer timer,
        CancellationToken cancellationToken = default)
    {

        // Check if the associated orchestrator is
        // running and can receive external events
        var query = new OrchestrationStatusQueryCondition()
        {
            InstanceIdPrefix = timer.Id.ToString()
        };
        var instances = await client.ListInstancesAsync(query, cancellationToken);
        var status = instances.DurableOrchestrationState.FirstOrDefault();
        var orchestratorCanRecieveExternalEvents = status?.RuntimeStatus
            .Equals(OrchestrationRuntimeStatus.Running) ?? false;

        // If orchestrator is not running, drop the call since
        if (!orchestratorCanRecieveExternalEvents)
        {
            return;
        }

        // Raise the external durable event
        client.RaiseEventAsync(
            instanceId: timer.Id.ToString(),
            eventName: nameof(TimerStoppedDurableEvent),
            eventData: new TimerStoppedDurableEvent())
            .Wait(cancellationToken);

        // Publish the event
        var calculator = calculatorFactory.GetCalculator(timer);
        await mediator.Publish(
            cancellationToken: cancellationToken,
            notification: new TimerStoppedEvent()
            {
                TimerId = timer.Id,
                RemainingTime = calculator.CalculateRemainingTime()
            });

    }

    public async Task PauseTimerAsync(
        ServerlessTimer timer, 
        CancellationToken cancellationToken = default)
    {

        // Check if the associated orchestrator is
        // running and can receive external events
        var query = new OrchestrationStatusQueryCondition()
        {
            InstanceIdPrefix = timer.Id.ToString()
        };
        var instances = await client.ListInstancesAsync(query, cancellationToken);
        var status = instances.DurableOrchestrationState.FirstOrDefault();
        var orchestratorCanRecieveExternalEvents = status?.RuntimeStatus
            .Equals(OrchestrationRuntimeStatus.Running) ?? false;

        // If orchestrator is not running, drop the call since
        if(!orchestratorCanRecieveExternalEvents)
        {
            return;
        }

        // Raise the external durable event
        client.RaiseEventAsync(
            instanceId: timer.Id.ToString(),
            eventName: nameof(TimerPausedDurableEvent),
            eventData: new TimerPausedDurableEvent()
            {
                Reason = nameof(PauseTimerAsync)
            })
            .Wait(cancellationToken);

        // Publish the domain event
        var calculator = calculatorFactory.GetCalculator(timer);
        await mediator.Publish(
            cancellationToken: cancellationToken,
            notification: new TimerPausedEvent()
            {
                TimerId = timer.Id,
                Duration = timer.Duration,
                CurrentTime = DateTime.UtcNow,
                ElapsedTime = calculator.CalculateElapsedTime()
            });

    }

    public async Task ResumeTimerAsync(
        ServerlessTimer timer,
        CancellationToken cancellationToken = default)
    {

        // Re-start orchestration
        // TODO: What if the orchestrator is not terminated?
        // NOTE: Not-terminated orchestrators cannot be restarted (results in an exception)
        client.RestartAsync(
            instanceId: timer.Id.ToString(),
            restartWithNewInstanceId: false)
            .Wait(cancellationToken);

        // Publish the domain event
        var calculator = calculatorFactory.GetCalculator(timer);
        await mediator.Publish(
            cancellationToken: cancellationToken,
            notification: new TimerResumedEvent()
            {
                TimerId = timer.Id,
                Duration = timer.Duration,
                CurrentTime = DateTime.UtcNow,
                RemainingTime = calculator.CalculateRemainingTime(),
            });

    }

}
