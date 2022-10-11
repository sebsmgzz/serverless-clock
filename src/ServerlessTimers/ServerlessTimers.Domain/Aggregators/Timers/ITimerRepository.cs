namespace ServerlessTimers.Domain.Aggregators.Timers;

using ServerlessTimers.Domain.Seedwork;

public interface ITimerRepository : IRepository<ServerlessTimer, Guid>
{

    Task<ServerlessTimer?> FindByIdAsync(
        Guid timerId, 
        CancellationToken cancellationToken = default);

}