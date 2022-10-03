namespace ServerlessAlarms.Application.Services.Commands;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ServerlessAlarms.Domain.Aggregators.Alarms;
using ServerlessAlarms.Application.Services.Durables;
using ServerlessAlarms.Application.Exceptions;

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
