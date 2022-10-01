namespace ServerlessAlarm.Domain.Events;

using MediatR;

public class AlarmSnoozedEvent : INotification
{

    public Guid AlarmId { get; set; }

}
