namespace ServerlessAlarm.Domain.Aggregators;

using ServerlessAlarm.Domain.Seedwork;

public class AlarmReaction : Enumeration<int, string>
{

    public AlarmReaction Dismiss => new(0, nameof(Dismiss));

    public AlarmReaction Snooze => new (1, nameof(Snooze));

    public AlarmReaction Timeout => new (2, nameof(Timeout));

    private AlarmReaction(int id, string name) : base(id, name)
    {
    }

}
