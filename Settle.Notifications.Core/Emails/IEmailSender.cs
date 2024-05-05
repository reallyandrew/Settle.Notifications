using Settle.Notifications.Core;
using Settle.Notifications.Core.Shared;

namespace Settle.Notifications.Emails;
public interface IEmailSender
{
    public Task<Result<MessageResponse>> SendAsync(EmailMessage message);
}
