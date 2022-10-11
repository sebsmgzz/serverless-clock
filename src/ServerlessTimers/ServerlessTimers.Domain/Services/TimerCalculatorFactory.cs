namespace ServerlessTimers.Domain.Services;

using ServerlessTimers.Domain.Aggregators.Timers;

public class TimerCalculatorFactory : ITimerCalculatorFactory
{

    public ITimerCalculator GetCalculator(ServerlessTimer timer)
    {
        return new TimerCalculator(timer);
    }

}
