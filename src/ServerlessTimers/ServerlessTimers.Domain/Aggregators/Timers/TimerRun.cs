namespace ServerlessTimers.Domain.Aggregators.Timers;

using ServerlessTimers.Domain.Seedwork;
using System.Collections.Generic;

public class TimerRun : ValueObject
{

    public DateTime BeganAt { get; }

    public DateTime? EndedAt { get; }

    public TimerRun(DateTime beganAt) : this(beganAt, null)
    {
    }

    public TimerRun(DateTime beganAt, DateTime? endedAt)
    {
        BeganAt = beganAt;
        EndedAt = endedAt;
    }

    protected override IEnumerable<object> GetComponents()
    {
        yield return BeganAt;
        yield return EndedAt;
    }

    public TimeSpan GetDuration()
    {
        if(EndedAt == null)
        {
            return TimeSpan.Zero;
        }
        return (DateTime)EndedAt - BeganAt;
    }

}
