namespace ServerlessTimers.Domain.Aggregators.Timers;

using ServerlessTimers.Domain.Seedwork;
using System.Collections;
using System.Collections.Generic;

public class TimerRunList : ValueObject, IReadOnlyList<TimerRun>
{

    private readonly List<TimerRun> runs;

    public TimerRun this[int index]
    {
        get => runs[index];
    }

    public int Count
    {
        get => runs.Count;
    }

    public TimerRunList() : this(new List<TimerRun>())
    {
    }

    public TimerRunList(IEnumerable<TimerRun> timerRuns)
    {
        runs = new List<TimerRun>(timerRuns);
    }

    public void BeginRun()
    {
        runs.Add(new TimerRun(DateTime.UtcNow));
    }

    public void EndRun()
    {
        var lastRun = runs.Last();
        runs.RemoveAt(runs.Count - 1);
        runs.Add(new TimerRun(lastRun.BeganAt, DateTime.UtcNow));
    }

    public IEnumerator<TimerRun> GetEnumerator()
    {
        return runs.GetEnumerator();
    }

    protected override IEnumerable<object> GetComponents()
    {
        return runs;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return runs.GetEnumerator();
    }

    public static implicit operator TimerRunList(List<TimerRun> enumerator)
    {
        return new TimerRunList(enumerator);
    }

}
