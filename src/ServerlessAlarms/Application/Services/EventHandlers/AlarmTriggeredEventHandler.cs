namespace ServerlessAlarms.Application.Services.EventHandlers;

using MediatR;
using ServerlessAlarms.Domain.Events;
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
