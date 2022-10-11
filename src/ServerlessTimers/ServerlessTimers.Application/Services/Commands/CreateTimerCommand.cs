namespace ServerlessTimers.Application.Services.Commands;

using MediatR;
using ServerlessTimers.Application.Services.Durables;
using ServerlessTimers.Domain.Aggregators.Timers;
using System;
using System.Threading;
using System.Threading.Tasks;

public class CreateTimerCommand : IRequest<Guid>
{

    public ServerlessTimer Timer { get; set; }

}
public class CreateTimerCommandHandler : IRequestHandler<CreateTimerCommand, Guid>
{

    private readonly ITimerRepository timerRepository;
    private readonly IDurableFacade durableFacade;

    public CreateTimerCommandHandler(
        ITimerRepository timerRepository,
        IDurableFacade durableFacade)
    {
        this.timerRepository = timerRepository;
        this.durableFacade = durableFacade;
    }

    public async Task<Guid> Handle(
        CreateTimerCommand request, 
        CancellationToken cancellationToken)
    {

        // Create the timer
        var timer = await timerRepository.AddAsync(request.Timer, cancellationToken);

        // Start the timer
        if(timer.State == TimerState.Started)
        {

            // Update the entity
            timer.Start();
            timerRepository.UpdateAsync(timer, cancellationToken).Wait(cancellationToken);

            // Start the durable timer
            await durableFacade.StartTimerAsync(timer, cancellationToken);

        }

        // Return the timer's id
        return request.Timer.Id;

    }

}
