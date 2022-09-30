namespace ServerlessAlarm.Application.Services.Commands;

using ServerlessAlarm.Application.Functions;
using Domain.Aggregators.Alarms;
using MediatR;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading;
using System.Threading.Tasks;
using ServerlessAlarm.Application.Models.Inputs;
using System;

public class CreateAlarmCommand : IRequest<Guid>
{

    public Alarm Alarm { get; set; }

}
public class CreateAlarmCommandHandler : IRequestHandler<CreateAlarmCommand, Guid>
{

    private readonly IAlarmRepository alarmsRepository;

    public CreateAlarmCommandHandler(
        IAlarmRepository alarmsRepository)
    {
        this.alarmsRepository = alarmsRepository;
    }

    public async Task<Guid> Handle(
        CreateAlarmCommand request,
        CancellationToken cancellationToken)
    {

        // Insert alarm
        await alarmsRepository.AddAsync(request.Alarm);
        
        // Return the alarm
        return request.Alarm.Id;

    }

}
