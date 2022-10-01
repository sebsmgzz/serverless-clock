namespace ServerlessAlarm.Application.Services.Commands;

using ServerlessAlarm.Domain.Events;
using MediatR;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading;
using System.Threading.Tasks;

public class DismissAlarmCommand : IRequest
{

    public Guid AlarmId { get; set; }

}
public class DismissAlarmCommandHandler : IRequestHandler<DismissAlarmCommand>
{

    private readonly IMediator mediator;

    public DismissAlarmCommandHandler(
        IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<Unit> Handle(
        DismissAlarmCommand request, 
        CancellationToken cancellationToken)
    {

        // Notify domain event
        await mediator.Publish(
            notification: new AlarmDismissedEvent()
            {
                AlarmId = request.AlarmId
            },
            cancellationToken: cancellationToken);

        // Return nothing
        return Unit.Value;

    }

}
