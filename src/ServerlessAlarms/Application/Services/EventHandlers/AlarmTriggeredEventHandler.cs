namespace ServerlessAlarm.Application.Services.EventHandlers;

using MediatR;
using ServerlessAlarm.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

public class AlarmTriggeredEventHandler : INotificationHandler<AlarmTriggeredEvent>
{

    public async Task Handle(
        AlarmTriggeredEvent notification, 
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

}
public class AlarmDismissedEventHandler : INotificationHandler<AlarmDismissedEvent>
{

    public async Task Handle(
        AlarmDismissedEvent notification,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

}
public class AlarmSnoozedEventHandler : INotificationHandler<AlarmSnoozedEvent>
{

    public async Task Handle(
        AlarmSnoozedEvent notification,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

}
public class AlarmTimedOutEvetnHandler : INotificationHandler<AlarmTimedoutEvent>
{

    public async Task Handle(
        AlarmTimedoutEvent notification, 
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

}
