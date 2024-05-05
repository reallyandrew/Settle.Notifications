namespace Settle.Notifications.Configuration;
public sealed class NotificationsOptions
{
    public const string Notifications = "Notifications";
    public string DefaultEmailSender { get; set; } = string.Empty;
    public TestModeOptions TestMode { get; set; } = new();
    public MailgunOptions? Mailgun {  get; set; }
}
