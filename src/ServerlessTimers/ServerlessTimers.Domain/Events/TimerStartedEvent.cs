namespace ServerlessTimers.Domain.Events;

using MediatR;

public class TimerStartedEvent : INotification
{

    public Guid TimerId { get; set; }

}
