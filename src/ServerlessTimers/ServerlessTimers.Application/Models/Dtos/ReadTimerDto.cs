namespace ServerlessTimers.Application.Models.Dtos;

using ServerlessTimers.Domain.Aggregators.Timers;
using System;
using System.Text.Json.Serialization;

public class ReadTimerDto
{

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("state")]
    public TimerStateDto State { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreateAt { get; set; }

    public static ReadTimerDto FromTimer(ServerlessTimer timer)
    {
        return new ReadTimerDto()
        {
            Id = timer.Id,
            Name = timer.Name,
            Description = timer.Description,
            State = TimerStateDto.FromTimerState(timer.State),
            CreateAt = timer.CreatedAt
        };
    }

}
