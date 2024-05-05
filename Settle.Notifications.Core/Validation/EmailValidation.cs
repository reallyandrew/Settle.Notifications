using System.Text.RegularExpressions;

namespace Settle.Notifications.Core.Validation;
public static class EmailValidation
{
    private static TimeSpan RegExTimeout { get; set; } = TimeSpan.FromMilliseconds(500);
    /// <summary>
    /// This method is for TestCase use only, it sets the Regex timeout to be 1µs to trigger the exception handling
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    internal static bool IsValidEmailTestRegEx(this string email)
    {
        RegExTimeout = TimeSpan.FromMicroseconds(1);
        return email.IsValidEmail();
    }
    /// <summary>
    /// This method is for TestCase use only, it ensures the Regex timeout is 500ms
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    internal static void ResetDefaultTimeout()
    {
        RegExTimeout = TimeSpan.FromMilliseconds(500);
    }
    /// <summary>
    /// Validates that the provided email address meets the mainstream requirements from RFC5322 and RFC 5321
    /// It disallows some technically valid, but unlikely to be used by real users forms of email address
    /// Once specific example is that including " in the email address will return false
    /// </summary>
    /// <param name="email">The email address to be tested</param>
    /// <returns>True/False</returns>
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        bool localPartResult;
        bool domainPartResult;
        try
        {
            email = NormaliseDomain(email);
            var atPosition = email.LastIndexOf('@');
            if (atPosition <0 )
            { return false; }
            localPartResult = CheckLocalPartRules(email[..atPosition]);
            domainPartResult = CheckDomainPartRules(email[(atPosition+1)..]);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
        return localPartResult && domainPartResult && Regex.IsMatch(email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
              RegexOptions.IgnoreCase, RegExTimeout);
    }

    private static bool CheckDomainPartRules(string v)
    {
        if (v.StartsWith('[')&& !v.EndsWith(']'))
        {  return false; }
        if (v.EndsWith(']') && !v.StartsWith('['))
        {  return false; }
        return true;
    }

    private static bool CheckLocalPartRules(string localPart)
    {
        if (string.IsNullOrWhiteSpace(localPart)) return false;
        if (localPart.StartsWith('.') || localPart.EndsWith('.'))
        {
            return false;
        }
        if (localPart.Contains(".."))
        {
            return false;
        }
        // Technically the local part of a domain can contain a ", but only in specific formats
        if (localPart.Contains('"'))
        {
            return false;
        }
        return true;
    }

    private static string NormaliseDomain(string email)
    {
        email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, RegExTimeout);

        static string DomainMapper(Match match)
        {
            // Use IdnMapping class to convert Unicode domain names.
            var idn = new System.Globalization.IdnMapping();

            // Pull out and process domain name (throws ArgumentException on invalid)
            var domainName = idn.GetAscii(match.Groups[2].Value);

            return match.Groups[1].Value + domainName;
        }

        return email;
    }
}
