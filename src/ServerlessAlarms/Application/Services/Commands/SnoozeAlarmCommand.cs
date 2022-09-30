namespace ServerlessAlarm.Application.Services.Commands;

using ServerlessAlarm.Application.Models.ExternalEvents;
using ServerlessAlarm.Domain.Events;
using MediatR;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading;
using System.Threading.Tasks;

public class SnoozeAlarmCommand : IRequest
{

    public Guid AlarmId { get; set; }

}
public class ScheduleAlarmCommandHandler : IRequestHandler<SnoozeAlarmCommand>
{

    private readonly IMediator mediator;

    public ScheduleAlarmCommandHandler(
        IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task<Unit> Handle(
        SnoozeAlarmCommand request, 
        CancellationToken cancellationToken)
    {

        // Notify domain event
        await mediator.Publish(
            notification: new AlarmSnoozedEvent()
            {
                AlarmId = request.AlarmId
            },
            cancellationToken: cancellationToken);

        // Return nothing
        return Unit.Value;

    }

}
