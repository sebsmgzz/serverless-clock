namespace ServerlessAlarm.Application.Services.Commands;

using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using ServerlessAlarm.Domain.Aggregators.Alarms;
using ServerlessAlarm.Application.Services.Durables;
using ServerlessAlarm.Application.Exceptions;

public class SnoozeAlarmCommand : IRequest
{

    public Guid AlarmId { get; set; }

}
public class ScheduleAlarmCommandHandler : IRequestHandler<SnoozeAlarmCommand>
{

    private readonly IAlarmRepository alarmRepository;
    private readonly IDurableFacade durableFacade;

    public ScheduleAlarmCommandHandler(
        IAlarmRepository alarmRepository,
        IDurableFacade durableFacade)
    {
        this.alarmRepository = alarmRepository;
        this.durableFacade = durableFacade;
    }

    public async Task<Unit> Handle(
        SnoozeAlarmCommand request, 
        CancellationToken cancellationToken)
    {

        // Get alarm
        var alarm = await alarmRepository.FindByIdAsync(request.AlarmId) ??
            throw new AlarmNotFoundException(request.AlarmId);

        // Raise the snooze alarm durable external event
        await durableFacade.SnoozeAlarmAsync(alarm);

        // Return nothing
        return Unit.Value;

    }

}
