namespace ServerlessAlarms.Application.Services.Commands;

using Domain.Aggregators.Alarms;
using MediatR;
using System;

public class CreateAlarmCommand : IRequest<Guid>
{

    public Alarm Alarm { get; set; }

}
