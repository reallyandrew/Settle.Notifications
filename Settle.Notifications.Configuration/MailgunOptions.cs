namespace Settle.Notifications.Configuration;
public class MailgunOptions
{
    public string? ApiKey { get; set; }
    public string? Domain { get; set; }
    public bool UseTestModeHeader { get; set; }
    public MailgunRegion Region { get; set; } = MailgunRegion.EU;
}
