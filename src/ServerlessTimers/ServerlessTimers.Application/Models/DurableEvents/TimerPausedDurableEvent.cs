namespace ServerlessTimers.Application.Models.DurableEvents;

public class TimerPausedDurableEvent : DurableEvent
{

    public string Reason { get; set; }

}
