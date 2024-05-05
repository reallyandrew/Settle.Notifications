using Settle.Notifications.Core;
using Settle.Notifications.Core.ValueObjects;

namespace Settle.Notifications.Emails;
public class EmailMessage : Message<Email>
{
    protected EmailMessage(Email sender) : base(sender)
    {
    }
    public IEnumerable<Email> Cc { get; private set; } = new List<Email>();
    public IEnumerable<Email> Bcc { get; private set; } = new List<Email>();
    public string Subject { get; private set; } = string.Empty;
    public IEnumerable<string> Tags { get; private set; } = new List<string>();

    public static EmailMessage Create(Email to, string subject, string body, Email from, string? tag = null)
    {
        return Create(new List<Email> { to }, subject, body, from, tag == null ? null : new List<string> { tag });
    }
    public static EmailMessage Create(IEnumerable<Email> to, string subject, string body, Email from, IEnumerable<string>? tags = null, IEnumerable<Email>? CcRecipients = null, IEnumerable<Email>? BccRecipients = null)
    {
        var email = new EmailMessage(from)
        {
            To = to,
            Subject = subject,
            Body = body,
        };
        if (tags != null)
        {
            email.Tags = tags;
        }
        if (CcRecipients != null)
        {
            email.Cc = CcRecipients;
        }
        if (BccRecipients != null)
        {
            email.Bcc = BccRecipients;
        }
        return email;
    }
}
