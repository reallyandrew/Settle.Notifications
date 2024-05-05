using System.Diagnostics.CodeAnalysis;

namespace Settle.Notifications.Core.Primitives;
public abstract class ValueObject : IEqualityComparer<ValueObject>
{
    public abstract IEnumerable<object> GetAtomicValues();

    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && ValuesAreEqual(other);
    }
    
    public static bool operator ==(ValueObject? a, ValueObject? b)
    {
        if (a is null && b is null)
            return true;
        if (a is null || b is null)
            return false;
        return a.Equals(b);
    }
    public static bool operator !=(ValueObject? a, ValueObject? b) => !(a == b);
    

    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(
                default(int),
                HashCode.Combine);
    }

    private bool ValuesAreEqual(ValueObject other)
    {
        return GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    public bool Equals(ValueObject? x, ValueObject? y)
    {
        if (x == null || y == null)
        {
            return object.Equals(x, y);
        }
        return x.Equals(y);
    }

    public int GetHashCode([DisallowNull] ValueObject obj) => obj.GetHashCode();
}
