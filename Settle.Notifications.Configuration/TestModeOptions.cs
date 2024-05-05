namespace Settle.Notifications.Configuration;
public sealed class TestModeOptions
{
    public bool IsEnabled { get; set; }
    public string? DefaultRecipient { get; set; }
    public string? AllowedEmailDomains { get; set; }
    public string? AllowedRecipients { get; set; }
}
