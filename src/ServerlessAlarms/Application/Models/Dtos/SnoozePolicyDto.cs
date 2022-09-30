namespace ServerlessAlarm.Application.Models.Dtos;

using Domain.Aggregators.Alarms;
using System;
using System.Text.Json.Serialization;

public class SnoozePolicyDto
{

    [JsonPropertyName("interval")]
    public TimeSpan Interval { get; set; }

    [JsonPropertyName("repeat")]
    public int Repeat { get; set; }

    public SnoozePolicy ToSnoozePolicy()
    {
        return new SnoozePolicy()
        {
            Interval = Interval,
            Repeat = Repeat
        };
    }

}
