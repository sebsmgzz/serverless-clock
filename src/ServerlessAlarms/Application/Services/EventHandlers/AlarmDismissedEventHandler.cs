namespace ServerlessAlarm.Application.Services.EventHandlers;

using MediatR;
using ServerlessAlarm.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

public class AlarmDismissedEventHandler : INotificationHandler<AlarmDismissedEvent>
{

    public async Task Handle(
        AlarmDismissedEvent notification,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

}
