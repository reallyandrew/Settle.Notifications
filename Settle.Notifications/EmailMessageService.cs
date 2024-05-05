using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Settle.Notifications.Configuration;
using Settle.Notifications.Core;
using Settle.Notifications.Core.Exceptions;
using Settle.Notifications.Core.Shared;
using Settle.Notifications.Core.ValueObjects;
using Settle.Notifications.Templates;

namespace Settle.Notifications;
internal class EmailMessageService : IEmailMessageService
{
    private readonly NotificationsOptions _settings;
    private readonly Email _defaultSenderEmail;
    private readonly List<Email> _defaultTestRecipients;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<EmailMessageService> _logger;
    public EmailMessageService(IConfiguration configuration, IEmailSender emailSender, ILogger<EmailMessageService> logger)
    {
        _settings = new();
        _logger = logger;
        configuration.GetSection(NotificationsOptions.Notifications).Bind(_settings);
        _defaultSenderEmail = GetDefaultSenderEmail();
        _defaultTestRecipients = GetDefaultTestRecipients();
        _emailSender = emailSender;
        _logger.LogDebug("EmailMessageService instantiated");
    }

    private Email GetDefaultSenderEmail()
    {
        _logger.LogDebug("Getting default sender email");
        var senderEmailResult = Email.Create(_settings.DefaultEmailSender);
        if (senderEmailResult.IsFailure)
        {
            throw new InvalidConfigurationException($"Notifications:DefaultSenderEmail - {senderEmailResult.Error.Message}");
        }
        _logger.LogDebug("Default sender email created: {email}", senderEmailResult.Value);
        return senderEmailResult.Value;
    }
    private List<Email> GetDefaultTestRecipients()
    {
        _logger.LogDebug("Getting default test recipients");
        if (!_settings.TestMode.IsEnabled)
        {
            _logger.LogInformation("Notification service test mode is not enabled");
            return [];
        }
        if (string.IsNullOrWhiteSpace(_settings.TestMode.DefaultRecipient))
        {
            throw new MissingConfigurationException("A default test recipient must be provided in test mode");
        }
        var emails = _settings.TestMode.DefaultRecipient!.Split(';', StringSplitOptions.TrimEntries);
        var recipients = new List<Email>();
        foreach (var email in emails)
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
            {
                throw new InvalidConfigurationException(emailResult.Error.Message);
            }
            recipients.Add(emailResult.Value);
        }
        _logger.LogInformation("{recipientCount} test recipients found",recipients.Count);
        return recipients;
    }
    private async Task<Result<string>> SendMessageAsync(Result<EmailMessage> emailMessageResult)
    {
        if (emailMessageResult.IsFailure)
        {
            _logger.LogWarning("EmailMessage could not be created: {error}", emailMessageResult.Error.Message);
            return Result.Failure<string>(emailMessageResult.Error);
        }
        return await SendMessageAsync(emailMessageResult.Value);
    }
    public async Task<Result<string>> SendMessageAsync(EmailMessage email)
    {
        if (_settings.TestMode.IsEnabled)
        {
            email = ReplaceUnauthoriseRecipients(email);
        }
        _logger.LogDebug("Sending email message via provider");
        var result = await _emailSender.SendAsync(email);
        if (result.IsFailure)
        {
            _logger.LogWarning("Message sending failed: {error}", result.Error.Message);
            return Result.Failure<string>(result.Error);
        }
        return email.Body;
    }

    public async Task<Result<string>> SendMessageAsync(Email recipient, string subject, string body)
    {
        var emailMessage = EmailMessage.Create(recipient, subject, body, _defaultSenderEmail);
        return await SendMessageAsync(emailMessage);
    }

    public async Task<Result<string>> SendMessageAsync(Email recipient, string subject, string body, string tag)
    {
        var emailMessage = EmailMessage.Create(recipient, subject, body, _defaultSenderEmail,tag);
        return await SendMessageAsync(emailMessage);
    }

    public async Task<Result<string>> SendMessageWithTemplateAsync(Email recipient, string subject, string body, ITemplateModel templateModel)
    {
        var emailMessageResult = TemplatedEmailMessage.Create(recipient,subject,body,templateModel,_defaultSenderEmail);
        return await SendMessageAsync(emailMessageResult);
    }

    public async Task<Result<string>> SendMessageWithTemplateAsync(Email recipient, string subject, string body, ITemplateModel templateModel, string tag)
    {
        var emailMessageResult = TemplatedEmailMessage.Create(recipient, subject, body, templateModel, _defaultSenderEmail, tag);
        return await SendMessageAsync(emailMessageResult);
    }

    public async Task<Result<string>> SendMessageWithBaseTemplateAsync(Email recipient, string subject, string body, string baseTemplate, ITemplateModel templateModel)
    {
        var emailMessageResult = TemplatedEmailMessage.Create(recipient, subject, body, baseTemplate, templateModel, _defaultSenderEmail);
        return await SendMessageAsync(emailMessageResult);
    }

    public async Task<Result<string>> SendMessageWithBaseTemplateAsync(Email recipient, string subject, string body, string baseTemplate, ITemplateModel templateModel, string tag)
    {
        var emailMessageResult = TemplatedEmailMessage.Create(recipient, subject, body, baseTemplate, templateModel, _defaultSenderEmail,tag);
        return await SendMessageAsync(emailMessageResult);
    }

    private EmailMessage ReplaceUnauthoriseRecipients(EmailMessage email)
    {
        var toRecipients = ReplaceUnauthorisedRecipients(email.To.ToList());
        var ccRecipients = ReplaceUnauthorisedRecipients(email.Cc.ToList());
        var bccRecipients = ReplaceUnauthorisedRecipients(email.Bcc.ToList());
        var cleanEmail = EmailMessage.Create(toRecipients, email.Subject, email.Body, email.Sender, email.Tags, ccRecipients, bccRecipients);
        return cleanEmail;
    }

    private List<Email> ReplaceUnauthorisedRecipients(List<Email> emailRecipients)
    {
        var recipients = new List<Email>();
        foreach (var recipient in emailRecipients)
        {
            if (IsAllowedDomain(recipient.Value) || (IsAllowedEmail(recipient.Value)))
            {
                recipients.Add(recipient);
            }
        }
        if (recipients.Count != 0)
        {
            return recipients;
        }
        return _defaultTestRecipients;
    }

    private bool IsAllowedDomain(string email)
    {
        if (string.IsNullOrWhiteSpace(_settings.TestMode.AllowedEmailDomains))
        {
            return false;
        }
        var allowedDomains = _settings.TestMode.AllowedEmailDomains.Split(';', StringSplitOptions.TrimEntries);
        var matchedDomain = Array.Find(allowedDomains, d => email.EndsWith($"@{d}"));
        return matchedDomain != null;
    }

    private bool IsAllowedEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(_settings.TestMode.AllowedRecipients))
        {
            return false;
        }
        var allowedEmails = _settings.TestMode.AllowedRecipients.Split(';', StringSplitOptions.TrimEntries);
        var matchedEmail = Array.Find(allowedEmails, e => email == e);
        return matchedEmail != null;

    }
}
