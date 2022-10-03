namespace ServerlessAlarms.Domain.Aggregators.Alarms;

using Domain.Seedwork;
using System;

public interface IAlarmRepository : IRepository<Alarm, Guid>
{

    Task<Alarm?> FindByIdAsync(Guid alarmId);

}
