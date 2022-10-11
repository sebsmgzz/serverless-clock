namespace ServerlessTimers.Application.Services.Commands;

using MediatR;
using ServerlessTimers.Application.Exceptions;
using ServerlessTimers.Application.Services.Durables;
using ServerlessTimers.Domain.Aggregators.Timers;
using System;
using System.Threading;
using System.Threading.Tasks;

public class StopTimerCommand : IRequest
{

    public Guid TimerId { get; set; }

}
public class StopTimerCommandHandler : IRequestHandler<StopTimerCommand>
{

    private readonly ITimerRepository timerRepository;
    private readonly IDurableFacade durableFacade;

    public StopTimerCommandHandler(
        ITimerRepository timerRepository,
        IDurableFacade durableFacade)
    {
        this.timerRepository = timerRepository;
        this.durableFacade = durableFacade;
    }

    public async Task<Unit> Handle(
        StopTimerCommand request, 
        CancellationToken cancellationToken)
    {

        // Get the timer
        var timer = await timerRepository.FindByIdAsync(request.TimerId, cancellationToken) ??
            throw new TimerNotFoundException(request.TimerId);

        // Stop the timer
        timer.Stop();
        timerRepository.UpdateAsync(timer, cancellationToken).Wait(cancellationToken);

        // Stop the durable timer
        await durableFacade.StopTimerAsync(timer, cancellationToken);

        // Return nothing
        return Unit.Value;

    }

}
