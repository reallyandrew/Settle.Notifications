namespace Settle.Notifications.Configuration;
public class MailgunOptions
{
    public string ApiKey { get; set; } = null!;
    public string Domain { get; set; } = null!;
    public bool UseTestModeHeader { get; set; }
    public MailgunRegion Region { get; set; } = MailgunRegion.EU;
}
