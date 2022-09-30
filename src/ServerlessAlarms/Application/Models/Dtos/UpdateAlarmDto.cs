namespace ServerlessAlarm.Application.Models.Dtos;

using Domain.Aggregators.Alarms;
using System;
using System.Text.Json.Serialization;

public class UpdateAlarmDto
{

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("recurrence")]
    public string Recurrence { get; set; }

    [JsonPropertyName("timeout")]
    public TimeSpan? Timeout { get; set; }

    [JsonPropertyName("snoozePolicy")]
    public SnoozePolicyDto SnoozePolicy { get; set; }

    public Alarm ToAlarm(Alarm alarm)
    {
        alarm.Name = Name ?? alarm.Name;
        alarm.Recurrence = Recurrence ?? alarm.Recurrence;
        alarm.Timeout = Timeout ?? alarm.Timeout;
        alarm.SnoozePolicy = SnoozePolicy.ToSnoozePolicy() ?? alarm.SnoozePolicy;
        return alarm;
    }

}