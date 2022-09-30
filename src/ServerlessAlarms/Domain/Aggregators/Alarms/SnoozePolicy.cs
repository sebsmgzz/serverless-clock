namespace ServerlessAlarm.Domain.Aggregators.Alarms;

using Domain.Seedwork;
using System;
using System.Collections.Generic;

// TODO: ValueObjects should be immutable
public class SnoozePolicy : ValueObject
{

    private int repeat = 3;

    /// <summary>
    /// The interval of the snoozing.
    /// </summary>
    public TimeSpan Interval { get; set; }

    /// <summary>
    /// The amount of times the snoozing can be repeating.
    /// When set to -1, snoozing can be repeated infinte times.
    /// </summary>
    public int Repeat
    {
        get => repeat;
        set
        {
            if (value == -1 || 0 < value)
            {
                repeat = 0;
            }
        }
    }

    public void ShallowCopy(SnoozePolicy otherPolicy)
    {
        otherPolicy.Interval = this.Interval;
        otherPolicy.Repeat = this.Repeat;
    }

    protected override IEnumerable<object> GetComponents()
    {
        yield return Interval;
        yield return Repeat;
    }

}
