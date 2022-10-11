namespace ServerlessTimers.Application.Services.Durables;

using ServerlessTimers.Domain.Aggregators.Timers;
using System.Threading;
using System.Threading.Tasks;

public interface IDurableFacade
{
    
    Task StartTimerAsync(
        ServerlessTimer timer,
        CancellationToken cancellationToken = default);

    Task StopTimerAsync(
        ServerlessTimer timer,
        CancellationToken cancellationToken = default);

    Task PauseTimerAsync(
        ServerlessTimer timer,
        CancellationToken cancellationToken = default);

    Task ResumeTimerAsync(
        ServerlessTimer timer,
        CancellationToken cancellationToken = default);

}
