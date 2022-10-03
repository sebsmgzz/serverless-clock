namespace ServerlessAlarms.Application.Services.Commands;

using MediatR;
using System;

public class SnoozeAlarmCommand : IRequest
{

    public Guid AlarmId { get; set; }

}
