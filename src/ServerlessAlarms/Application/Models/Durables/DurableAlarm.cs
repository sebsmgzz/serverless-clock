namespace ServerlessAlarm.Application.Models.Durables;

using Newtonsoft.Json;

public interface ISnoozing
{

    void Snooze();

    void Restart();

}
[JsonObject(MemberSerialization.OptIn)]
public class Snoozing : ISnoozing
{

    [JsonProperty("count")]
    public int Count { get; set; }

    public void Snooze()
    {
        Count++;
    }

    public void Restart()
    {
        Count = 0;
    }

}
