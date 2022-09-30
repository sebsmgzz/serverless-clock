namespace ServerlessAlarm.Infrastructure.Repositories;

using ServerlessAlarm.Domain.Aggregators.Alarms;
using System.Text.Json;

public class AlarmRepository : LocalRepository<Alarm, Guid>, IAlarmRepository
{

    protected override void AssignNewId(Alarm entity)
    {
        entity.Id = Guid.NewGuid();
    }

    protected override byte[] Serialize(Alarm entity)
    {
        return JsonSerializer.SerializeToUtf8Bytes(entity);
    }

    protected override Alarm Deserialize(byte[] bytes)
    {
        return JsonSerializer.Deserialize<Alarm>(bytes) ??
            throw new ArgumentNullException("Could not deserialize alarm");
    }

}
