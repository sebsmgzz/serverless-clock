namespace ServerlessAlarm.Application.Services.Commands;

using ServerlessAlarm.Domain.Aggregators.Alarms;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using ServerlessAlarm.Application.Exceptions;
using ServerlessAlarm.Application.Services.Durables;

public class UpdateAlarmCommand : IRequest
{

    public Guid AlarmId { get; set; }

    public string AlarmName { get; set; }

    public string AlarmRecurrence { get; set; }

    public TimeSpan? AlarmTimeout { get; set; }

    public TimeSpan? SnoozeInterval { get; set; }

    public int? SnoozeRepeat { get; set; }

}
public class UpdateAlarmCommandHandler : IRequestHandler<UpdateAlarmCommand>
{

    private readonly IAlarmRepository alarmsRepository;
    private readonly IDurableFacade durableFacade;

    public UpdateAlarmCommandHandler(
        IAlarmRepository alarmsRepository,
        IDurableFacade durableFacade)
    {
        this.alarmsRepository = alarmsRepository;
        this.durableFacade = durableFacade;
    }

    public async Task<Unit> Handle(
        UpdateAlarmCommand request,
        CancellationToken cancellationToken)
    {

        // Get the alarm
        var alarm = await alarmsRepository.FindByIdAsync(request.AlarmId) ??
            throw new AlarmNotFoundException(request.AlarmId);

        // Update alarm
        alarm.Name = request.AlarmName ?? alarm.Name;
        alarm.Recurrence = request.AlarmRecurrence ?? alarm.Recurrence;
        alarm.Timeout = request.AlarmTimeout ?? alarm.Timeout;
        alarm.SnoozePolicy.Interval = request.SnoozeInterval ?? alarm.SnoozePolicy.Interval;
        alarm.SnoozePolicy.Repeat = request.SnoozeRepeat ?? alarm.SnoozePolicy.Repeat;
        await alarmsRepository.UpdateAsync(alarm);

        // Restart durable function
        await durableFacade.RestartAlarmAsync(alarm);

        // Return nothing
        return Unit.Value;

    }

}
