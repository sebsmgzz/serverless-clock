namespace ServerlessAlarm.Domain.Seedwork;

using System.Security.Cryptography;

public abstract class Entity<TId> where TId : struct
{

    public TId Id { get; set; }

    public bool IsTransient()
    {
        return Id.Equals(default(TId));
    }

    public bool Equals(Entity<TId>? entity)
    {
        return entity != null && Id.Equals(entity.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Equals(entity);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return Equals(left, null) ? Equals(right, null) : left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }

}
