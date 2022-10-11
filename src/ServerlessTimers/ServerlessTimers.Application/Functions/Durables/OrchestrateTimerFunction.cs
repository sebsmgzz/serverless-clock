namespace ServerlessTimers.Application.Functions.Durables;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using ServerlessTimers.Application.Exceptions;
using ServerlessTimers.Application.Models.DurableEvents;
using ServerlessTimers.Application.Models.Durables;
using ServerlessTimers.Application.Services.Durables;
using ServerlessTimers.Domain.Aggregators.Timers;
using ServerlessTimers.Domain.Services;

public class OrchestrateTimerFunction
{

    private readonly ILogger logger;
    private readonly IDurableFacade durableFacade;
    private readonly ITimerRepository timerRepository;
    private readonly ITimerCalculatorFactory calculatorFactory;
    private readonly CancellationTokenSource cts;

    public OrchestrateTimerFunction(
        IDurableFacade durableFacade,
        ITimerRepository timerRepository,
        ITimerCalculatorFactory calculatorFactory,
        ILogger<OrchestrateTimerFunction> logger)
    {
        this.logger = logger;
        this.durableFacade = durableFacade;
        this.timerRepository = timerRepository;
        this.calculatorFactory = calculatorFactory;
        cts = new CancellationTokenSource();
    }

    [FunctionName(nameof(OrchestrateTimerFunction))]
    public async Task RunOrchestrator(
        [OrchestrationTrigger] 
        IDurableOrchestrationContext context)
    {
        try
        {

            // Get timer
            var input = context.GetInput<TimerOrchestratorInput>();
            var timer = await timerRepository.FindByIdAsync(input.TimerId) ??
                throw new TimerNotFoundException(input.TimerId);

            // Do not run orchestration if timer's shouldn't be running
            if(!timer.State.EqualRunningState())
            {
                logger.LogError($"Timer {timer.Id}: " +
                    $"Tried to be orchestrated but has {timer.State} state");
                throw new Exception();
            }

            // Calculate the completion date of the timer
            var calculator = calculatorFactory.GetCalculator(timer);
            var remainingTime = calculator.CalculateRemainingTime();
            logger.LogInformation($"Timer {timer.Id}: " +
                $"To complete in {remainingTime}");
            if (remainingTime <= TimeSpan.Zero)
            {
                logger.LogError($"Timer {timer.Id}: " +
                    $"Remaining time is negative");
                throw new Exception();
            }

            // Set external events
            var timerPausedEventTask = context.WaitForExternalEvent<DurableEvent>(
                name: nameof(TimerPausedDurableEvent),
                defaultValue: new TimerCompletedDurableEvent(),
                timeout: remainingTime,
                cancelToken: cts.Token);
            var timerStoppedEventTask = context.WaitForExternalEvent<DurableEvent>(
                name: nameof(TimerStoppedDurableEvent),
                defaultValue: new TimerCompletedDurableEvent(),
                timeout: remainingTime,
                cancelToken: cts.Token);
            
            // Await timer
            var durableEvent = await Task.WhenAny<DurableEvent>(
                timerPausedEventTask, timerStoppedEventTask);
            cts.Cancel();
            
            // Handle events
            if(durableEvent.Result is TimerCompletedDurableEvent)
            {
                logger.LogInformation($"Timer {timer.Id}: Completed");
            }
            else if (durableEvent.Result is TimerStoppedDurableEvent)
            {
                logger.LogInformation($"Timer {timer.Id}: Stopped");
            }
            else if (durableEvent.Result is TimerPausedDurableEvent pausedEvent)
            {
                logger.LogInformation($"Timer {timer.Id}: Paused ({pausedEvent.Reason})");
            }

        }
        catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

}
