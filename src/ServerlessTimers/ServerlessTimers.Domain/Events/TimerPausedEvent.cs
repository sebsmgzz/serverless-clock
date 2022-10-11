namespace ServerlessTimers.Domain.Events;

using MediatR;

public class TimerPausedEvent : INotification
{

    public Guid TimerId { get; set; }

    public TimeSpan Duration { get; set; }

    public DateTime CurrentTime { get; set; }
    
    public TimeSpan ElapsedTime { get; set; }
    
}
