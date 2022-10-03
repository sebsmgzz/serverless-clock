namespace ServerlessAlarms.Application.Services.Commands;

using Domain.Aggregators.Alarms;
using MediatR;
using ServerlessAlarms.Application.Services.Durables;
using System.Threading;
using System.Threading.Tasks;

public class DeleteAlarmCommandHandler : IRequestHandler<DeleteAlarmCommand>
{

    private readonly IDurableFacade durableFacade;
    private readonly IAlarmRepository alarmsRepository;

    public DeleteAlarmCommandHandler(
        IDurableFacade durableFacade,
        IAlarmRepository alarmsRepository)
    {
        this.durableFacade = durableFacade;
        this.alarmsRepository = alarmsRepository;
    }

    public async Task<Unit> Handle(
        DeleteAlarmCommand request,
        CancellationToken cancellationToken)
    {

        // Delete the alarm
        var alarm = await alarmsRepository.FindByIdAsync(request.AlarmId);
        await alarmsRepository.RemoveAsync(alarm);

        // Deactivate the durable orchestration
        await durableFacade.DeactivateAlarmAsync(alarm);

        // Return nothing
        return Unit.Value;

    }

}
