using Settle.Notifications.Core;
using Settle.Notifications.Core.Shared;
using Settle.Notifications.Core.ValueObjects;
using Settle.Notifications.Templates;

namespace Settle.Notifications;
public interface IEmailMessageService
{
    /// <summary>
    /// Sends the email message
    /// </summary>
    /// <param name="email">The email message to send.</param>
    /// <returns></returns>
    Task<Result<string>> SendMessageAsync(EmailMessage email);
    /// <summary>
    /// Sends an email to the nominated recipient. This does not support templating of the email message. It will be sent as HTML, from the default sender, with no tag.
    /// </summary>
    /// <param name="recipient">The person to receive the message</param>
    /// <param name="subject">The subject line for the email</param>
    /// <param name="body">The body of the message.</param>
    /// <returns>Returns the body of the message as sent.</returns>
    Task<Result<string>> SendMessageAsync(Email recipient, string subject, string body);
    /// <summary>
    /// Sends an email to the nominated recipient. This does not support templating of the email message. It will be sent from the default sender.
    /// </summary>
    /// <param name="recipient">The person to receive the message</param>
    /// <param name="subject">The subject line for the email</param>
    /// <param name="body">The body of the message.</param>
    /// <param name="tag">The tag (category) for the message</param>
    /// <returns>Returns the body of the message as sent.</returns>
    Task<Result<string>> SendMessageAsync(Email recipient, string subject, string body, string tag);
    /// <summary>
    /// Sends an email to the nominated recipient. Supports Liquid templating in the body. Provide an ITemplateModel with the parameters to replace. It will be sent as HTML, from the default sender, with no tag.
    /// </summary>
    /// <param name="recipient">The person to receive the message</param>
    /// <param name="subject">The subject line for the email</param>
    /// <param name="body">The body of the message.</param>
    /// <param name="templateModel">The template model</param>
    /// <returns>Returns the body of the message as sent.</returns>
    Task<Result<string>> SendMessageWithTemplateAsync(Email recipient, string subject, string body, ITemplateModel templateModel);
    /// <summary>
    /// Sends an email to the nominated recipient. Supports Liquid templating in the body. Provide an ITemplateModel with the parameters to replace. It will be sent from the default sender.
    /// </summary>
    /// <param name="recipient">The person to receive the message</param>
    /// <param name="subject">The subject line for the email</param>
    /// <param name="body">The body of the message.</param>
    /// <param name="templateModel">The template model</param>
    /// <param name="isHtml">Indicates if the message is to be sent as HTML</param>
    /// <param name="tag">The tag (category) for the message</param>
    /// <returns>Returns the body of the message as sent.</returns>
    Task<Result<string>> SendMessageWithTemplateAsync(Email recipient, string subject, string body, ITemplateModel templateModel, string tag);
    /// <summary>
    ///  Sends an email to the nominated recipient. Supports Liquid templating in the body. Uses a baseTemplate to give an overall layout. Provide an ITemplateModel with the parameters to replace. It will be sent from the default sender with no Tag.
    /// </summary>
    /// <param name="recipient">The person to receive the message</param>
    /// <param name="subject">The subject line for the email</param>
    /// <param name="body">The body of the message.</param>
    /// <param name="baseTemplate">The base (outer) template for the email. The body is inserted into a {{ content }} tag in the base template.</param>
    /// <param name="templateModel">The template model</param>
    /// <returns></returns>
    Task<Result<string>> SendMessageWithBaseTemplateAsync(Email recipient, string subject, string body, string baseTemplate, ITemplateModel templateModel);
    /// <summary>
    /// Sends an email to the nominated recipient. Supports Liquid templating in the body. Uses a baseTemplate to give an overall layout. Provide an ITemplateModel with the parameters to replace. It will be sent from the default sender.
    /// </summary>
    /// <param name="recipient">The person to receive the message</param>
    /// <param name="subject">The subject line for the email</param>
    /// <param name="body">The body of the message.</param>
    /// <param name="baseTemplate">The base (outer) template for the email. The body is inserted into a {{ content }} tag in the base template.</param>
    /// <param name="templateModel">The template model</param>
    /// <param name="isHtml">Indicates if the message is to be sent as HTML</param>
    /// <param name="tag">The tag (category) for the message</param>
    /// <returns></returns>
    Task<Result<string>> SendMessageWithBaseTemplateAsync(Email recipient, string subject, string body, string baseTemplate, ITemplateModel templateModel, string tag);

}
