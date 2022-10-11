namespace ServerlessTimers.Domain.Events;

using MediatR;

public class TimerStoppedEvent : INotification
{

    public Guid TimerId { get; set; }

    public TimeSpan RemainingTime { get; set; }

}
