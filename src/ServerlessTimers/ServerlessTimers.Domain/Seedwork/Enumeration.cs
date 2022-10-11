namespace ServerlessTimers.Domain.Seedwork;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;

public abstract class Enumeration<TKey, TValue> : IComparable 
    where TKey : struct, IComparable
{

    public TKey Id { get; private set; }

    public TValue Name { get; private set; }

    protected Enumeration(TKey id, TValue name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString()
    {
        return $"({Id}, {Name})";
    }

    public bool Equals(Enumeration<TKey, TValue>? enumeration)
    {
        return enumeration != null && 
            GetType().Equals(enumeration.GetType()) && 
            Id.Equals(enumeration.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Enumeration<TKey, TValue> enumeration && Equals(enumeration);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public int CompareTo(object? other)
    {
        if(other is Enumeration<TKey, TValue> otherEnumeration)
        {
            return Id.CompareTo(otherEnumeration.Id);
        }
        var thisTypeName = GetType().Name;
        var otherTypeName = other?.GetType().Name ?? "null";
        throw new ArgumentException(
            $"Type {otherTypeName} cannot be compared with {thisTypeName}");
    }

    public static bool operator ==(
        Enumeration<TKey, TValue> left, 
        Enumeration<TKey, TValue> right)
    {
        return Equals(left, null) ? Equals(right, null) : left.Equals(right);
    }

    public static bool operator !=(
        Enumeration<TKey, TValue> left,
        Enumeration<TKey, TValue> right)
    {
        return !(left == right);
    }

}
