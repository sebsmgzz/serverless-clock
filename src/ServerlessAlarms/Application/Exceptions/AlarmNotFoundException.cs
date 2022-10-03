namespace ServerlessAlarms.Application.Exceptions;

using System;

public class AlarmNotFoundException : Exception
{

    public AlarmNotFoundException() : base("Alarm not found")
    {
    }

    public AlarmNotFoundException(Guid id) : base($"Alarm {id} not found")
    {
    }

}
