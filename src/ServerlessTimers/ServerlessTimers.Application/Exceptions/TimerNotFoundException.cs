namespace ServerlessTimers.Application.Exceptions;

using System;

public class TimerNotFoundException : Exception
{

    public TimerNotFoundException() : 
        base("Timer not found")
    {
    }

    public TimerNotFoundException(Guid id) : 
        base($"Timer {id} not found")
    {
    }

}
