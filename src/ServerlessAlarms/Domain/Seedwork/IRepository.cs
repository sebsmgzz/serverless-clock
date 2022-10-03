namespace ServerlessAlarms.Domain.Seedwork;

public interface IRepository<TEntity, TId> 
    where TEntity : Entity<TId> 
    where TId : struct
{

    Task AddAsync(TEntity entity);

    Task RemoveAsync(TEntity entity);

    Task UpdateAsync(TEntity entity);

}