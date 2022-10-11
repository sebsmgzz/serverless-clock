namespace ServerlessTimers.Application.Exceptions;

using ServerlessTimers.Domain.Aggregators.Timers;
using System;

public class InvalidStateChangeException : InvalidOperationException
{

    public InvalidStateChangeException() : 
        base("Cannot change the timer's state")
    {
    }

    public InvalidStateChangeException(TimerState desiredState) :
        base($"Cannot change the timer's state to {desiredState}")
    {
    }

    public InvalidStateChangeException(TimerState currentState, TimerState desiredState) :
        base($"Cannot change the timer's state from {currentState} to {desiredState}")
    {
    }

}