namespace ServerlessTimers.Application.Services.Commands;

using MediatR;
using ServerlessTimers.Application.Exceptions;
using ServerlessTimers.Application.Services.Durables;
using ServerlessTimers.Domain.Aggregators.Timers;
using System;
using System.Threading;
using System.Threading.Tasks;

public class StartTimerCommand : IRequest
{

    public Guid TimerId { get; set; }

}
public class StartTimerCommandHandler : IRequestHandler<StartTimerCommand>
{

    private readonly ITimerRepository timerRepository;
    private readonly IDurableFacade durableFacade;

    public StartTimerCommandHandler(
        ITimerRepository timerRepository,
        IDurableFacade durableFacade)
    {
        this.timerRepository = timerRepository;
        this.durableFacade = durableFacade;
    }

    public async Task<Unit> Handle(
        StartTimerCommand request, 
        CancellationToken cancellationToken)
    {

        // Get the timer
        var timer = await timerRepository.FindByIdAsync(request.TimerId, cancellationToken) ??
            throw new TimerNotFoundException(request.TimerId);

        // Update the entity
        timer.Start();
        timerRepository.UpdateAsync(timer, cancellationToken).Wait(cancellationToken);

        // Start the durable timer
        await durableFacade.StartTimerAsync(timer, cancellationToken);

        // Return nothing
        return Unit.Value;

    }

}