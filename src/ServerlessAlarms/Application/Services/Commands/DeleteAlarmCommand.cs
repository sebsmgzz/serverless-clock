namespace ServerlessAlarms.Application.Services.Commands;

using MediatR;
using System;

public class DeleteAlarmCommand : IRequest
{

    public Guid AlarmId { get; set; }

}
