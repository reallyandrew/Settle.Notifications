using System.Text.RegularExpressions;

namespace Settle.Notifications.Core.Validation;
public static class MobilePhoneValidation
{
    private static TimeSpan RegExTimeout { get; set; } = TimeSpan.FromMilliseconds(250);
    public static bool IsValidUkNumber(this string phoneNumber) 
        => PhoneNumberIsAMobile(phoneNumber) && PhoneNumberHasRightNumberOfDigits(phoneNumber);

    private static bool PhoneNumberHasRightNumberOfDigits(string phoneNumber)
    {
        var re = new Regex("^(?:(?:\\+447)|(?:07))[0-9]{9}$", RegexOptions.None, RegExTimeout);
        return re.IsMatch(phoneNumber);
    }

    private static bool PhoneNumberIsAMobile(string phoneNumber) => phoneNumber.StartsWith("07") || phoneNumber.StartsWith("+447");

}
