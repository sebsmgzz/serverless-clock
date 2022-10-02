namespace ServerlessAlarm.Application.Services.EventHandlers;

using MediatR;
using ServerlessAlarm.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

public class AlarmTimedOutEvetnHandler : INotificationHandler<AlarmTimedoutEvent>
{

    public async Task Handle(
        AlarmTimedoutEvent notification, 
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

}
