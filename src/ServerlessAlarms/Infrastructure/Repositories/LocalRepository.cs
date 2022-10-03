namespace ServerlessAlarms.Infrastructure.Repositories;

using ServerlessAlarms.Domain.Seedwork;
using System.Text;

public abstract class LocalRepository<TEntity, TId> 
    where TEntity : Entity<TId> 
    where TId : struct
{

    protected string GetRepoDirectory()
    {
        var tempDirPath = Path.GetTempPath();
        var project = nameof(ServerlessAlarms);
        var repoName = GetType().Name;
        return Path.Join(tempDirPath, project, repoName);
    }

    protected string GetEntityFileName(TId id)
    {
        return $"{typeof(TEntity).Name}-{id}.json";
    }

    public async Task AddAsync(TEntity entity)
    {

        // Ensure consistency and get location
        AssignNewId(entity);
        var repoDir = GetRepoDirectory();
        var entityFileName = GetEntityFileName(entity.Id);
        var entityFilePath = Path.Join(repoDir, entityFileName);
        Directory.CreateDirectory(repoDir);

        // Write entity
        var entityBytes = Serialize(entity);
        using var entityStream = File.Open(entityFilePath, FileMode.OpenOrCreate);
        entityStream.Write(entityBytes);
        await Task.CompletedTask;

    }

    public async Task RemoveAsync(TEntity entity)
    {

        // Get location
        var repoDir = GetRepoDirectory();
        var entityFileName = GetEntityFileName(entity.Id);
        var entityFilePath = Path.Join(repoDir, entityFileName);

        // Delete entity
        File.Delete(entityFilePath);
        await Task.CompletedTask;

    }

    public async Task UpdateAsync(TEntity entity)
    {

        // Get location
        var repoDir = GetRepoDirectory();
        var entityFileName = GetEntityFileName(entity.Id);
        var entityFilePath = Path.Join(repoDir, entityFileName);

        // Update entity
        var entityBytes = Serialize(entity);
        using var entityStream = File.OpenWrite(entityFilePath);
        await entityStream.WriteAsync(entityBytes);

    }

    public async Task<TEntity?> FindByIdAsync(TId id)
    {

        // Get location
        var repoDir = GetRepoDirectory();
        var entityFileName = GetEntityFileName(id);
        var entityFilePath = Path.Join(repoDir, entityFileName);

        // Be consistent
        if (!File.Exists(entityFilePath))
        {
            return null;
        }

        // Read entity
        var entityBytes = File.ReadAllBytes(entityFilePath);
        var entity = Deserialize(entityBytes);
        return await Task.FromResult(entity);

    }

    protected abstract void AssignNewId(TEntity entity);

    protected abstract byte[] Serialize(TEntity entity);

    protected abstract TEntity Deserialize(byte[] bytes);

}
