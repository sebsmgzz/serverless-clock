namespace ServerlessTimers.Domain.Seedwork;

public interface IRepository<TEntity, TId> 
    where TEntity : Entity<TId> 
    where TId : struct
{

    Task<TEntity> AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default);

    Task<TEntity> RemoveAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default);

    Task<TEntity> UpdateAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default);

}