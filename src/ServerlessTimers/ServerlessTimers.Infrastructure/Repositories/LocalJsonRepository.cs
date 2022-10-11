namespace ServerlessTimers.Infrastructure.Repositories;

using Newtonsoft.Json;
using ServerlessTimers.Domain.Seedwork;
using System.Text;

public abstract class LocalJsonRepository<TEntity, TId> :
    LocalRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : struct
{

    protected override string GetEntityFileName(TId id)
    {
        return $"{typeof(TEntity).Name}-{id}.json";
    }

    protected override byte[] Serialize(TEntity entity)
    {
        var entityString = JsonConvert.SerializeObject(entity);
        return Encoding.UTF8.GetBytes(entityString);
    }

    protected override TEntity Deserialize(byte[] bytes)
    {
        var entityString = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<TEntity>(entityString);
    }

}
