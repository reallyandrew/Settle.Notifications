using Settle.Notifications.Core.Shared;
using Settle.Notifications.Core.ValueObjects;
using Settle.Notifications.Emails;

namespace Settle.Notifications.Templates;
public class TemplatedEmailMessage : EmailMessage
{
    private TemplatedEmailMessage(Email sender) : base(sender)
    {

    }

    public static Result<EmailMessage> Create(Email to, string subject, string body, ITemplateModel templateModel, Email from, string? tag = null)
    {
        var processedBodyResult = FluidTemplate.ParseTemplate(body, templateModel);
        if (processedBodyResult.IsFailure)
        {
            return Result.Failure<EmailMessage>(processedBodyResult.Error);
        }
        var email = Create(to, subject, processedBodyResult.Value, from, tag);
        return email;
    }
    public static Result<EmailMessage> Create(Email to, string subject, string body, string baseTemplate, ITemplateModel templateModel, Email from, string? tag = null)
    {
        var processedBodyResult = FluidTemplate.ParseTemplate(body, templateModel);
        if (processedBodyResult.IsFailure)
        {
            return Result.Failure<EmailMessage>(processedBodyResult.Error);
        }
        var baseTemplateModel = new BaseTemplateModel(processedBodyResult.Value);
        var emailBody = FluidTemplate.ParseTemplate(baseTemplate, baseTemplateModel);
        if (emailBody.IsFailure)
        {
            return Result.Failure<EmailMessage>(emailBody.Error);
        }
        var email = Create(to, subject, emailBody.Value, from, tag);
        return email;
    }
}
