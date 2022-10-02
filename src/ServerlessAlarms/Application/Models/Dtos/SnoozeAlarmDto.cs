namespace ServerlessAlarm.Application.Models.Dtos;

using System;
using System.Text.Json.Serialization;

public class SnoozeAlarmDto
{

    [JsonPropertyName("alarmId")]
    public Guid AlarmId { get; set; }

}
