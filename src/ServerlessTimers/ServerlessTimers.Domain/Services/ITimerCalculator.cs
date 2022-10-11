namespace ServerlessTimers.Domain.Services;

using System;

public interface ITimerCalculator
{
    
    TimeSpan CalculateElapsedTime();

    TimeSpan CalculateRemainingTime();

    DateTime CalculateCompletionDate();

}
