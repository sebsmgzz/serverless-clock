namespace ServerlessAlarm.Application.Models.Dtos;

using Domain.Aggregators.Alarms;
using System;
using System.Text.Json.Serialization;

public class CreateAlarmDto
{

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("recurrence")]
    public string Recurrence { get; set; }

    [JsonPropertyName("timeout")]
    public TimeSpan Timeout { get; set; }

    [JsonPropertyName("snoozePolicy")]
    public SnoozePolicyDto SnoozePolicy { get; set; }

    public Alarm ToAlarm(Guid id = default)
    {
        return new Alarm()
        {
            Id = id,
            Name = Name,
            Recurrence = Recurrence,
            Timeout = Timeout,
            IsEnabled = true,
            SnoozePolicy = SnoozePolicy.ToSnoozePolicy()
        };
    }

}
