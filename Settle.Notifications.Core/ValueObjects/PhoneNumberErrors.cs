using Settle.Notifications.Core.Shared;

namespace Settle.Notifications.Core.ValueObjects;
public static class PhoneNumberErrors
{
    public static Error Empty => new("Phone.Empty", "Phone number is empty");
    public static Error Invalid => new("Phone.Invalid", "Not a valid UK mobile number");
}
