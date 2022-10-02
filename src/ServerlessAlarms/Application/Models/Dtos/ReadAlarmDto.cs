namespace ServerlessAlarm.Application.Models.Dtos;

using Domain.Aggregators.Alarms;
using System;
using System.Text.Json.Serialization;

public class ReadAlarmDto
{

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("recurrence")]
    public string Recurrence { get; set; }

    [JsonPropertyName("timeout")]
    public TimeSpan Timeout { get; set; }

    [JsonPropertyName("snoozePolicy")]
    public SnoozePolicyDto SnoozePolicy { get; set; }

    public static ReadAlarmDto FromAlarm(Alarm alarm)
    {
        return new ReadAlarmDto()
        {
            Id = alarm.Id,
            Name = alarm.Name,
            Recurrence = alarm.Recurrence,
            Timeout = alarm.Timeout,
            SnoozePolicy = SnoozePolicyDto.FromSnoozePolicy(alarm.SnoozePolicy)
        };
    }

}
