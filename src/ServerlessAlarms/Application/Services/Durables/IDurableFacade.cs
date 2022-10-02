namespace ServerlessAlarm.Application.Services.Durables;

using ServerlessAlarm.Domain.Aggregators.Alarms;
using System.Threading.Tasks;

public interface IDurableFacade
{
    
    Task ActivateAlarmAsync(Alarm alarm);

    Task DeactivateAlarmAsync(Alarm alarm);

    Task<bool> RestartAlarmAsync(Alarm alarm);

    Task SnoozeAlarmAsync(Alarm alarm);

    Task DismissAlarmAsync(Alarm alarm);

}
