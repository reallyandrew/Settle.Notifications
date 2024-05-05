using Settle.Notifications.Core.Primitives;
using Settle.Notifications.Core.Shared;
using Settle.Notifications.Core.Validation;
using System.ComponentModel;

namespace Settle.Notifications.Core.ValueObjects;
[DefaultProperty("Value")]
public sealed class Email : ValueObject
{
    public const int MaxLength = 100;
    private Email(string value)
    {
        Value = value.ToLowerInvariant();
    }
    public string Value { get; }
    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrEmpty(email))
        {            
            return Result.Failure<Email>(EmailErrors.Empty);
        }
        if (!email.IsValidEmail())
        {
            return Result.Failure<Email>(EmailErrors.Invalid);
        }
        return new Email(email);
    }
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
