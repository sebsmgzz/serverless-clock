namespace ServerlessAlarms.Application.Services.Queries;

using MediatR;
using ServerlessAlarms.Application.Exceptions;
using ServerlessAlarms.Domain.Aggregators.Alarms;
using System.Threading;
using System.Threading.Tasks;

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