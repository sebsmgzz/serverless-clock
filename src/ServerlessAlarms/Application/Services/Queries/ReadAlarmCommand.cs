namespace ServerlessAlarm.Application.Services.Queries;

using MediatR;
using ServerlessAlarm.Application.Exceptions;
using ServerlessAlarm.Domain.Aggregators.Alarms;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ReadAlarmCommand : IRequest<Alarm>
{

    public Guid AlarmId { get; set; }

}
public class ReadAlarmCommandHandler : IRequestHandler<ReadAlarmCommand, Alarm>
{

    private readonly IAlarmRepository alarmRepository;

    public ReadAlarmCommandHandler(IAlarmRepository alarmRepository)
    {
        this.alarmRepository = alarmRepository;
    }

    public async Task<Alarm> Handle(
        ReadAlarmCommand request, 
        CancellationToken cancellationToken)
    {

        return await alarmRepository.FindByIdAsync(request.AlarmId) ??
            throw new AlarmNotFoundException(request.AlarmId);

    }

}