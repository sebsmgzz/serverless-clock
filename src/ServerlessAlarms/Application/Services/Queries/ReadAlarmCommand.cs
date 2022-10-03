namespace ServerlessAlarms.Application.Services.Queries;

using MediatR;
using ServerlessAlarms.Domain.Aggregators.Alarms;
using System;

public class ReadAlarmCommand : IRequest<Alarm>
{

    public Guid AlarmId { get; set; }

}
