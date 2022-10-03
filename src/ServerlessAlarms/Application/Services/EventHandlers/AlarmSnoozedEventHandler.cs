namespace ServerlessAlarms.Application.Services.EventHandlers;

using MediatR;
using ServerlessAlarms.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

public class AlarmSnoozedEventHandler : INotificationHandler<AlarmSnoozedEvent>
{

    public async Task Handle(
        AlarmSnoozedEvent notification,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

}
