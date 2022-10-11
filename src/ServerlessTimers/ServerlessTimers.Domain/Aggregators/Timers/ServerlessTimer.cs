namespace ServerlessTimers.Domain.Aggregators.Timers;

using ServerlessTimers.Domain.Seedwork;

public class ServerlessTimer : Entity<Guid>
{

    public TimerRunList Runs { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public TimerState State { get; set; }

    public TimeSpan Duration { get; set; }

    public DateTime CreatedAt { get; set; }

    public ServerlessTimer() : this(new TimerRunList())
    {
    }

    public ServerlessTimer(TimerRunList runs)
    {
        State = TimerState.Created;
        Runs = runs;
    }

    private bool TrySetState(TimerState state, bool safe = true)
    {
        if(State.CanChangeTo(state))
        {
            State = state;
            return true;
        }
        return safe ? false : throw new InvalidOperationException();
    }

    public void Start()
    {
        TrySetState(TimerState.Started, false);
        Runs.BeginRun();
    }

    public void Pause()
    {
        TrySetState(TimerState.Paused, false);
        Runs.EndRun();
    }

    public void Resume()
    {
        TrySetState(TimerState.Resumed, false);
        Runs.BeginRun();
    }

    public void Stop()
    {
        TrySetState(TimerState.Stopped, false);
        Runs.EndRun();
    }

    public void Complete()
    {
        TrySetState(TimerState.Completed, false);
        Runs.EndRun();
    }

}
