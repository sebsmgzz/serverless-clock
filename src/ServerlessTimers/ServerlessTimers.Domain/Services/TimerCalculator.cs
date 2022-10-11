namespace ServerlessTimers.Domain.Services;

using ServerlessTimers.Domain.Aggregators.Timers;
using System;

public class TimerCalculator : ITimerCalculator
{

    private readonly ServerlessTimer timer;

    public TimerCalculator(ServerlessTimer timer)
    {
        this.timer = timer!;
    }

    public TimeSpan CalculateElapsedTime()
    {
        var elapsedTime = TimeSpan.Zero;
        foreach (var run in timer.Runs)
        {
            elapsedTime += run.GetDuration();
        }
        return elapsedTime;
    }

    public TimeSpan CalculateRemainingTime()
    {
        return timer.Duration - CalculateElapsedTime();
    }

    public DateTime CalculateCompletionDate()
    {
        var remainingTime = CalculateRemainingTime();
        return DateTime.UtcNow + remainingTime;
    }

}
