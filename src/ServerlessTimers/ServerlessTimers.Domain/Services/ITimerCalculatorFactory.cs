namespace ServerlessTimers.Domain.Services;

using ServerlessTimers.Domain.Aggregators.Timers;

public interface ITimerCalculatorFactory
{

    ITimerCalculator GetCalculator(ServerlessTimer timer);

}
