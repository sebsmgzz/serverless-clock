namespace ServerlessAlarm.Application.Services.Commands;

using ServerlessAlarm.Domain.Aggregators.Alarms;
using MediatR;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading;
using System.Threading.Tasks;

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
    private readonly IDurableOrchestrationClient durableClient;

    public UpdateAlarmCommandHandler(
        IAlarmRepository alarmsRepository,
        IDurableOrchestrationClient durableClient)
    {
        this.alarmsRepository = alarmsRepository;
        this.durableClient = durableClient;
    }

    public async Task<Unit> Handle(
        UpdateAlarmCommand request,
        CancellationToken cancellationToken)
    {

        // Update alarm
        var alarm = await alarmsRepository.FindByIdAsync(request.AlarmId);
        alarm.Name = request.AlarmName ?? alarm.Name;
        alarm.Recurrence = request.AlarmRecurrence ?? alarm.Recurrence;
        alarm.Timeout = request.AlarmTimeout ?? alarm.Timeout;
        alarm.SnoozePolicy.Interval = request.SnoozeInterval ?? alarm.SnoozePolicy.Interval;
        alarm.SnoozePolicy.Repeat = request.SnoozeRepeat ?? alarm.SnoozePolicy.Repeat;

        // Restart durable function
        await durableClient.RestartAsync(
            instanceId: alarm.Id.ToString(),
            restartWithNewInstanceId: false);
        
        // Return nothing
        return Unit.Value;

    }

}
