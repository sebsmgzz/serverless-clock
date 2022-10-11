namespace ServerlessTimers.Application.Services.Queries;

using MediatR;
using ServerlessTimers.Application.Exceptions;
using ServerlessTimers.Domain.Aggregators.Timers;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ReadTimerQuery : IRequest<ServerlessTimer>
{

    public Guid TimerId { get; set; }

}
public class ReadTimerQueryHandler : IRequestHandler<ReadTimerQuery, ServerlessTimer>
{

    private readonly ITimerRepository timerRepository;

    public ReadTimerQueryHandler(ITimerRepository timerRepository)
    {
        this.timerRepository = timerRepository;
    }

    public async Task<ServerlessTimer> Handle(
        ReadTimerQuery request, 
        CancellationToken cancellationToken)
    {

        // Get the timer
        return await timerRepository.FindByIdAsync(request.TimerId, cancellationToken) ??
            throw new TimerNotFoundException(request.TimerId);

    }

}
