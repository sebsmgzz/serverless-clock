namespace ServerlessAlarms.Domain.Seedwork;

public abstract class ValueObject
{

    protected abstract IEnumerable<object> GetComponents();

    public bool Equals(ValueObject? valueObject)
    {
        return valueObject != null && 
            GetComponents().SequenceEqual(valueObject.GetComponents());
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueObject valueObject && Equals(valueObject);
    }

    public override int GetHashCode()
    {
        return GetComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return Equals(left, null) ? Equals(right, null) : left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }

}
