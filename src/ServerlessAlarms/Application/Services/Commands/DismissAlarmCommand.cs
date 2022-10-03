namespace ServerlessAlarms.Application.Services.Commands;

using MediatR;
using System;

public class DismissAlarmCommand : IRequest
{

    public Guid AlarmId { get; set; }

}
