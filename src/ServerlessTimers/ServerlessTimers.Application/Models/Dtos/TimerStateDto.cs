namespace ServerlessTimers.Application.Models.Dtos;

using ServerlessTimers.Domain.Aggregators.Timers;
using System.Text.Json.Serialization;

public class TimerStateDto
{

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    public static TimerStateDto FromTimerState(TimerState state)
    {
        return new TimerStateDto()
        {
            Id = state.Id,
            Name = state.Name
        };
    }

}