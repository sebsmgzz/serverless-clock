namespace ServerlessTimers.Application.Services.Commands;

using MediatR;
using ServerlessTimers.Application.Exceptions;
using ServerlessTimers.Domain.Aggregators.Timers;
using System;
using System.Threading;
using System.Threading.Tasks;

public class DeleteTimerCommand : IRequest
{

    public Guid TimerId { get; set; }

}
public class DeleteTimerCommandHandler : IRequestHandler<DeleteTimerCommand>
{

    private readonly ITimerRepository timerRepository;

    public DeleteTimerCommandHandler(ITimerRepository timerRepository)
    {
        this.timerRepository = timerRepository;
    }

    public async Task<Unit> Handle(
        DeleteTimerCommand request, 
        CancellationToken cancellationToken)
    {

        // Get the timer
        var timer = await timerRepository.FindByIdAsync(request.TimerId, cancellationToken) ??
            throw new TimerNotFoundException(request.TimerId);

        // Delete the timer
        await timerRepository.RemoveAsync(timer, cancellationToken);

        // Return noting
        return Unit.Value;

    }

}

