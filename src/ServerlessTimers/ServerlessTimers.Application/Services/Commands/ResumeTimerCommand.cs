namespace ServerlessTimers.Application.Services.Commands;

using MediatR;
using ServerlessTimers.Application.Exceptions;
using ServerlessTimers.Application.Services.Durables;
using ServerlessTimers.Domain.Aggregators.Timers;
using System;
using System.Threading.Tasks;
using System.Threading;

public class ResumeTimerCommand : IRequest
{

    public Guid TimerId { get; set; }

}

public class ResumeTimerCommandHandler : IRequestHandler<ResumeTimerCommand>
{

    private readonly ITimerRepository timerRepository;
    private readonly IDurableFacade durableFacade;

    public ResumeTimerCommandHandler(
        ITimerRepository timerRepository,
        IDurableFacade durableFacade)
    {
        this.timerRepository = timerRepository;
        this.durableFacade = durableFacade;
    }

    public async Task<Unit> Handle(
        ResumeTimerCommand request,
        CancellationToken cancellationToken)
    {

        // Get the timer
        var timer = await timerRepository.FindByIdAsync(request.TimerId, cancellationToken) ??
            throw new TimerNotFoundException(request.TimerId);

        // Resume the timer
        timer.Resume();
        timerRepository.UpdateAsync(timer, cancellationToken).Wait(cancellationToken);

        // Resume the durable timer
        await durableFacade.ResumeTimerAsync(timer, cancellationToken);

        // Return nothing
        return Unit.Value;

    }

}
