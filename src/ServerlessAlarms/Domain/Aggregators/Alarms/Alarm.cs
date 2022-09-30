namespace ServerlessAlarm.Domain.Aggregators.Alarms;

using Domain.Seedwork;
using System;

public class Alarm : Entity<Guid>
{

    /// <summary>
    /// A human-friendly way to represent the alarm.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A CRON expression determining how ofthen the alarm should be triggered.
    /// </summary>
    public string Recurrence { get; set; }

    /// <summary>
    /// True if the alarm is meant to go off when its time comes, false otherwise.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// The duration of how long the dismissing/snozzing of the alarm
    /// will be available, after triggered, beforing timing out.
    /// After this time, the alarm snoozes itself if snoozing is available, else it dismisses itself.
    /// </summary>
    public TimeSpan Timeout { get; set; }

    /// <summary>
    /// The snoozing capabilities of the alarm.
    /// If null, the alarm cannot be snoozed.
    /// </summary>
    public SnoozePolicy SnoozePolicy { get; set; }

    public void ShallowCopy(Alarm otherAlarm)
    {
        otherAlarm.Name = this.Name;
        otherAlarm.Recurrence = this.Recurrence;
        otherAlarm.IsEnabled = this.IsEnabled;
        otherAlarm.Timeout = this.Timeout;
        this.SnoozePolicy.ShallowCopy(otherAlarm.SnoozePolicy);
    }

}
