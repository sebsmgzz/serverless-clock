namespace ServerlessAlarm.Application.Models.ExternalEvents;

public sealed class OnAlarmTriggeredEvent
{

    public static OnAlarmTriggeredEvent AlarmDismissed => new(0, nameof(AlarmDismissed));
    
    public static OnAlarmTriggeredEvent AlarmSnoozed => new(1, nameof(AlarmSnoozed));

    public static OnAlarmTriggeredEvent AlarmTimedout => new(2, nameof(AlarmTimedout));

    public int Index { get; }

    public string Name { get; }

    private OnAlarmTriggeredEvent(int key, string name)
    {
        Index = key;
        Name = name;
    }

    public bool Equals(OnAlarmTriggeredEvent otherEvent)
    {
        return otherEvent.Index.Equals(Index);
    }

    public override bool Equals(object obj)
    {
        return obj is OnAlarmTriggeredEvent alarmEvent && Equals(alarmEvent);
    }

    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }

    public static bool operator ==(OnAlarmTriggeredEvent left, OnAlarmTriggeredEvent right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(OnAlarmTriggeredEvent left, OnAlarmTriggeredEvent right)
    {

        return !Equals(left, right);
    }

}
