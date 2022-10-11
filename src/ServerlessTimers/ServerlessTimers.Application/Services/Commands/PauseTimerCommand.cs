namespace ServerlessTimers.Application.Services.Commands;

using MediatR;
using ServerlessTimers.Application.Exceptions;
using ServerlessTimers.Application.Services.Durables;
using ServerlessTimers.Domain.Aggregators.Timers;
using System;
using System.Threading;
using System.Threading.Tasks;

public class PauseTimerCommand : IRequest
{

    public Guid TimerId { get; set; }

}
public class PauseTimerCommandHandler : IRequestHandler<PauseTimerCommand>
{

    private readonly ITimerRepository timerRepository;
    private readonly IDurableFacade durableFacade;

    public PauseTimerCommandHandler(
        ITimerRepository timerRepository,
        IDurableFacade durableFacade)
    {
        this.timerRepository = timerRepository;
        this.durableFacade = durableFacade;
    }

    public async Task<Unit> Handle(
        PauseTimerCommand request, 
        CancellationToken cancellationToken)
    {

        // Get the timer
        var timer = await timerRepository.FindByIdAsync(request.TimerId, cancellationToken) ??
            throw new TimerNotFoundException(request.TimerId);

        // Update the entity
        timer.Pause();
        timerRepository.UpdateAsync(timer, cancellationToken).Wait(cancellationToken);

        // Pause the durable timer
        await durableFacade.PauseTimerAsync(timer, cancellationToken);

        // Return nothing
        return Unit.Value;

    }

}
