using Settle.Notifications.Core.Primitives;
using Settle.Notifications.Core.Shared;
using Settle.Notifications.Core.Validation;
using System.ComponentModel;

namespace Settle.Notifications.Core.ValueObjects;
[DefaultProperty("Value")]
public sealed class UKMobilePhoneNumber : ValueObject
{
    public const int MaxLength = 13;
    private UKMobilePhoneNumber(string value)
    {
        Value = value;
    }
    public string Value { get; }
    public static Result<UKMobilePhoneNumber> Create(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return Result.Failure<UKMobilePhoneNumber>(PhoneNumberErrors.Empty);
        }
        if (!phoneNumber.IsValidUkNumber())
        {
            return Result.Failure<UKMobilePhoneNumber>(PhoneNumberErrors.Invalid);
        }
        return new UKMobilePhoneNumber(phoneNumber);
    }
    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
