namespace ServerlessAlarm.Application.Models.Durables;

using System;

public class OrchestrateAlarmInput
{

    public Guid AlarmId { get; set; }

    public int Snoozes { get; set; } = 0;

}
