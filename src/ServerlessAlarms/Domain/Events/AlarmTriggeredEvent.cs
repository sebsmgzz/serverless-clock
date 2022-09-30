namespace ServerlessAlarm.Domain.Events;

using MediatR;

public class AlarmTriggeredEvent : INotification
{

    public Guid AlarmId { get; set; }

}
