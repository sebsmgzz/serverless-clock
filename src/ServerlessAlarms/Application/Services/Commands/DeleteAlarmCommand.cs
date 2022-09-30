namespace ServerlessAlarm.Application.Services.Commands;

using Domain.Aggregators.Alarms;
using MediatR;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Threading;
using System.Threading.Tasks;

public class DeleteAlarmCommand : IRequest
{

    public Guid AlarmId { get; set; }

}
public class DeleteAlarmCommandHandler : IRequestHandler<DeleteAlarmCommand>
{

    private readonly IAlarmRepository alarmsRepository;

    public DeleteAlarmCommandHandler(
        IAlarmRepository alarmsRepository)
    {
        this.alarmsRepository = alarmsRepository;
    }

    public async Task<Unit> Handle(
        DeleteAlarmCommand request, 
        CancellationToken cancellationToken)
    {

        // Delete the alarm
        var alarm = await alarmsRepository.FindByIdAsync(request.AlarmId);
        await alarmsRepository.RemoveAsync(alarm);

        // Return nothing
        return Unit.Value;

    }

}
