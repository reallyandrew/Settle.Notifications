using Settle.Notifications.Core.Shared;

namespace Settle.Notifications.Core.ValueObjects;
public static class EmailErrors
{
    public static Error Empty => new("Email.Empty", "Email is empty");
    public static Error Invalid => new("Email.Invalid", "Not a valid email address");
}
