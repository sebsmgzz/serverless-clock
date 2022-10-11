namespace ServerlessTimers.Infrastructure.Repositories;

using ServerlessTimers.Domain.Seedwork;

public abstract class LocalRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : struct
{

    protected abstract string GetEntityFileName(TId id);

    protected abstract void AssignNewId(TEntity entity);

    protected abstract byte[] Serialize(TEntity entity);

    protected abstract TEntity Deserialize(byte[] bytes);

    protected string GetRepoDirectory()
    {
        var tempDirPath = Path.GetTempPath();
        var project = nameof(ServerlessTimers);
        var repoName = GetType().Name;
        return Path.Join(tempDirPath, project, repoName);
    }

    public async Task<TEntity> AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
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
        await entityStream.WriteAsync(entityBytes, cancellationToken);

        // Return entity
        return await Task.FromResult(entity);

    }

    public async Task<TEntity> RemoveAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {

        // Get file location
        var repoDir = GetRepoDirectory();
        var entityFileName = GetEntityFileName(entity.Id);
        var entityFilePath = Path.Join(repoDir, entityFileName);

        // Delete file
        File.Delete(entityFilePath);

        // Return entity
        return await Task.FromResult(entity);

    }

    public async Task<TEntity> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {

        // Get file location
        var repoDir = GetRepoDirectory();
        var entityFileName = GetEntityFileName(entity.Id);
        var entityFilePath = Path.Join(repoDir, entityFileName);

        // Update entity
        var entityBytes = Serialize(entity);
        using var entityStream = File.OpenWrite(entityFilePath);
        await entityStream.WriteAsync(entityBytes, cancellationToken);

        // Return entity
        return await Task.FromResult(entity);

    }

}
