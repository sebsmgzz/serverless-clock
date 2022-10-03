namespace ServerlessAlarms.Application.Models.Dtos;

using System;
using System.Text.Json.Serialization;

public class DismissAlarmDto
{

    [JsonPropertyName("alarmId")]
    public Guid AlarmId { get; set; }

}
