namespace ServerlessAlarm.Application.Models.Durable;

using Newtonsoft.Json;

public interface IDurableAlarm
{

    void Snooze();

    void Restart();

}
[JsonObject(MemberSerialization.OptIn)]
public class DurableAlarm : IDurableAlarm
{


    [JsonProperty("snoozes")]
    public int Snoozes { get; set; }

    public void Snooze()
    {
        Snoozes++;
    }

    public void Restart()
    {
        Snoozes = 0;
    }

}
