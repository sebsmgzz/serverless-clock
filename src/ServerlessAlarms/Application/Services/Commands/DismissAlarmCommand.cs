namespace ServerlessAlarm.Application.Services.Commands;

using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using ServerlessAlarm.Application.Services.Durables;
using ServerlessAlarm.Domain.Aggregators.Alarms;
using ServerlessAlarm.Application.Exceptions;

public class DismissAlarmCommand : IRequest
{

    public Guid AlarmId { get; set; }

}
public class DismissAlarmCommandHandler : IRequestHandler<DismissAlarmCommand>
{

    private readonly IAlarmRepository alarmRepository;
    private readonly IDurableFacade durableFacade;
    private readonly IMediator mediator;

    public DismissAlarmCommandHandler(
        IAlarmRepository alarmRepository,
        IDurableFacade durableFacade,
        IMediator mediator)
    {
        this.alarmRepository = alarmRepository;
        this.durableFacade = durableFacade;
        this.mediator = mediator;
    }

    public async Task<Unit> Handle(
        DismissAlarmCommand request, 
        CancellationToken cancellationToken)
    {

        // Get alarm
        var alarm = await alarmRepository.FindByIdAsync(request.AlarmId) ??
            throw new AlarmNotFoundException(request.AlarmId);

        // Raise the dismiss alarm durable external event
        await durableFacade.DismissAlarmAsync(alarm);

        // Return nothing
        return Unit.Value;

    }

}
