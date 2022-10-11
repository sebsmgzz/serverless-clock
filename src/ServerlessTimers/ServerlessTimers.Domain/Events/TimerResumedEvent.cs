namespace ServerlessTimers.Domain.Events;

using MediatR;

public class TimerResumedEvent : INotification
{

    public Guid TimerId { get; set; }

    public TimeSpan Duration { get; set; }
    
    public DateTime CurrentTime { get; set; }
    
    public TimeSpan RemainingTime { get; set; }

}
