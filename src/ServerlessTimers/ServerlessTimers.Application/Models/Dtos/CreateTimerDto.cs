namespace ServerlessTimers.Application.Models.Dtos;

using ServerlessTimers.Domain.Aggregators.Timers;
using System;
using System.Text.Json.Serialization;

public class CreateTimerDto
{

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("duration")]
    public TimeSpan Duration { get; set; }

    public ServerlessTimer ToTimer()
    {
        return new ServerlessTimer()
        {
            Name = Name,
            Description = Description,
            Duration = Duration,
            CreatedAt = DateTime.UtcNow
        };
    }

}
