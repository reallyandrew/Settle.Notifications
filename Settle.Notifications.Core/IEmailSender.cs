using Settle.Notifications.Core.Shared;

namespace Settle.Notifications.Core;
public interface IEmailSender
{
    public Task<Result<MessageResponse>> SendAsync(EmailMessage message);
}
