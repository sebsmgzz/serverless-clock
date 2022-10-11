namespace ServerlessTimers.Domain.Aggregators.Timers;

using ServerlessTimers.Domain.Seedwork;

public class TimerState : Enumeration<int, string>
{

    public static TimerState Created => new(0, nameof(Created));

    public static TimerState Started => new(1, nameof(Started));

    public static TimerState Paused => new(2, nameof(Paused));

    public static TimerState Resumed => new(2, nameof(Resumed));

    public static TimerState Completed => new(3, nameof(Completed));

    public static TimerState Stopped => new(4, nameof(Stopped));

    private TimerState(int id, string name) : base(id, name)
    {
    }

    public bool EqualRunningState()
    {
        return this == Started || this == Resumed;
    }

    public bool CanChangeTo(TimerState state)
    {
        if (this == Created)
        {
            return state == Started;
        }
        else if (this == Started)
        {
            return state == Paused ||
                   state == Completed ||
                   state == Stopped;
        }
        else if (this == Paused)
        {
            return state == Resumed || 
                   state == Stopped;
        }
        else if (this == Resumed)
        {
            return state == Paused ||
                   state == Completed ||
                   state == Stopped;
        }
        else if (this == Completed)
        {
            return false;
        }
        else if (this == Stopped)
        {
            return false;
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    public static TimerState FromId(int id)
    {
        var states = new List<TimerState>
        {
            Created,
            Started,
            Paused,
            Resumed,
            Stopped,
            Completed
        };
        return states.First(s => s.Id == id);
    }

}
