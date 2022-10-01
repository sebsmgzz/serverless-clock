namespace ServerlessAlarm.Application.Models.Inputs;

using System;

public class TriggerAlarmInput
{

    public Guid AlarmId { get; set; }

    public int Snoozes { get; set; }

}