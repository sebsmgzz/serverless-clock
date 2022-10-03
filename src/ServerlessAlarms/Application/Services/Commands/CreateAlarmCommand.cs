namespace ServerlessAlarm.Application.Services.Commands;

using Domain.Aggregators.Alarms;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using ServerlessAlarm.Application.Services.Durables;

public class CreateAlarmCommand : IRequest<Guid>
{

    public Alarm Alarm { get; set; }

}
public class CreateAlarmCommandHandler : IRequestHandler<CreateAlarmCommand, Guid>
{

    private readonly IDurableFacade durableFacade;
    private readonly IAlarmRepository alarmsRepository;

    public CreateAlarmCommandHandler(
        IDurableFacade durableFacade,
        IAlarmRepository alarmsRepository)
    {
        this.durableFacade = durableFacade;
        this.alarmsRepository = alarmsRepository;
    }

    public async Task<Guid> Handle(
        CreateAlarmCommand request,
        CancellationToken cancellationToken)
    {

        // Insert the alarm
        await alarmsRepository.AddAsync(request.Alarm);

        // Activate the durable orchestration
        await durableFacade.ActivateAlarmAsync(request.Alarm);

        // Return the alarm
        return request.Alarm.Id;

    }

}
