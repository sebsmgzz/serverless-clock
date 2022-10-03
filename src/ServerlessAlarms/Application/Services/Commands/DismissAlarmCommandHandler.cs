namespace ServerlessAlarms.Application.Services.Commands;

using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ServerlessAlarms.Application.Services.Durables;
using ServerlessAlarms.Domain.Aggregators.Alarms;
using ServerlessAlarms.Application.Exceptions;

public class DismissAlarmCommandHandler : IRequestHandler<DismissAlarmCommand>
{

    private readonly IAlarmRepository alarmRepository;
    private readonly IDurableFacade durableFacade;
    private readonly IMediator mediator;

    public DismissAlarmCommandHandler(
        IAlarmRepository alarmRepository,
        IDurableFacade durableFacade,
        IMediator mediator)
    {
        this.alarmRepository = alarmRepository;
        this.durableFacade = durableFacade;
        this.mediator = mediator;
    }

    public async Task<Unit> Handle(
        DismissAlarmCommand request,
        CancellationToken cancellationToken)
    {

        // Get alarm
        var alarm = await alarmRepository.FindByIdAsync(request.AlarmId) ??
            throw new AlarmNotFoundException(request.AlarmId);

        // Raise the dismiss alarm durable external event
        await durableFacade.DismissAlarmAsync(alarm);

        // Return nothing
        return Unit.Value;

    }

}
