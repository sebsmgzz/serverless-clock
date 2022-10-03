namespace ServerlessAlarms.Application.Services.Commands;

using MediatR;
using System;

public class UpdateAlarmCommand : IRequest
{

    public Guid AlarmId { get; set; }

    public string AlarmName { get; set; }

    public string AlarmRecurrence { get; set; }

    public TimeSpan? AlarmTimeout { get; set; }

    public TimeSpan? SnoozeInterval { get; set; }

    public int? SnoozeRepeat { get; set; }

}
