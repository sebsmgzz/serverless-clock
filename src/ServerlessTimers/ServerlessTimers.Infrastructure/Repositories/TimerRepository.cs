namespace ServerlessTimers.Infrastructure.Repositories;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerlessTimers.Domain.Aggregators.Timers;
using ServerlessTimers.Domain.Seedwork;
using System.Text;

public class TimerRepository : LocalJsonRepository<ServerlessTimer, Guid>, ITimerRepository
{

    protected override void AssignNewId(ServerlessTimer timer)
    {
        timer.Id = Guid.NewGuid();
    }

    protected override ServerlessTimer Deserialize(byte[] bytes)
    {
        var timerString = Encoding.UTF8.GetString(bytes);
        var jObject = JObject.Parse(timerString);
        var jArray = (JArray)jObject[nameof(ServerlessTimer.Runs)];
        var runs = new List<TimerRun>();
        foreach(var jToken in jArray)
        {
            var beganAt = jToken[nameof(TimerRun.BeganAt)].Value<DateTime>();
            var endedAt = jToken[nameof(TimerRun.EndedAt)].Value<DateTime?>();
            runs.Add(new TimerRun(beganAt, endedAt));
        }
        return new ServerlessTimer(new TimerRunList(runs))
        {
            Id = Guid.Parse(jObject[nameof(ServerlessTimer.Id)].Value<string>()),
            Name = jObject[nameof(ServerlessTimer.Name)].Value<string>(),
            Description = jObject[nameof(ServerlessTimer.Description)].Value<string>(),
            CreatedAt = jObject[nameof(ServerlessTimer.CreatedAt)].Value<DateTime>(),
            Duration = TimeSpan.Parse(jObject[nameof(ServerlessTimer.Duration)].Value<string>()),
            State = TimerState.FromId(jObject[nameof(ServerlessTimer.State)][nameof(TimerState.Id)].Value<int>())
        };
    }

    public async Task<ServerlessTimer?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {

        // Get location
        var repoDir = GetRepoDirectory();
        var entityFileName = GetEntityFileName(id);
        var entityFilePath = Path.Join(repoDir, entityFileName);

        // Ensure consistency
        if (!File.Exists(entityFilePath))
        {
            return null;
        }

        // Read entity
        var entityBytes = await File.ReadAllBytesAsync(entityFilePath, cancellationToken);
        var entity = Deserialize(entityBytes);
        return entity;

    }

}