namespace ServerlessTimers.Domain.Events;

using MediatR;

public class TimerCompletedEvent : INotification
{

    public Guid TimerId { get; set; }

}