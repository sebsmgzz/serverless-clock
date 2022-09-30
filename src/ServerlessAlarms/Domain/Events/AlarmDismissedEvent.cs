namespace ServerlessAlarm.Domain.Events;

using MediatR;

public class AlarmDismissedEvent : INotification
{

    public Guid AlarmId { get; set; }

}
