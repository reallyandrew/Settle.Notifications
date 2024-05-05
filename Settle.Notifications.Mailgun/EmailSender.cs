using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Settle.Notifications.Configuration;
using Settle.Notifications.Core;
using Settle.Notifications.Core.Exceptions;
using Settle.Notifications.Core.Shared;
using Settle.Notifications.Core.ValueObjects;
using Settle.Notifications.Emails;
using System.Net;
using System.Text;

namespace Settle.Notifications.Mailgun;

internal sealed class EmailSender : IEmailSender
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _credentials;
    private readonly bool _isTestMode;
    private readonly bool _useTestModeHeader;
    private readonly string _domain;
    private readonly string _baseUrl;

    public EmailSender(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        NotificationsOptions settings = new();
        configuration.GetSection(NotificationsOptions.Notifications).Bind(settings);
        if (settings.Mailgun is null)
        {
            throw new MissingConfigurationException("Mailgun is not configured");
        }
        if (!string.IsNullOrWhiteSpace(settings.Mailgun.ApiKey))
        {
            _credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{settings.Mailgun.ApiKey}"));
        }
        else
        {
            throw new MissingConfigurationException("API key is not set");
        }
        _isTestMode = settings.TestMode.IsEnabled;
        _useTestModeHeader = settings.Mailgun.UseTestModeHeader;
        _domain = settings.Mailgun.Domain?? throw new MissingConfigurationException("Domain is not set");
        _baseUrl = settings.Mailgun.Region == MailgunRegion.EU ? "https://api.eu.mailgun.net/v3/" : "https://api.mailgun.net/v3/";
    }

    public async Task<Result<MessageResponse>> SendAsync(EmailMessage message)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Basic {_credentials}");
        MultipartFormDataContent data = CreateMessageData(message);
        try
        {
            var request = await client.PostAsync($"{_baseUrl}{_domain}/messages", data);
            var response = await request.Content.ReadAsStringAsync();
            if (request.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure<MessageResponse>(new Error("Mailgun.Unauthorized", "You are not authorized to send via Mailgun. Check your API key. Check your region"));
            }
            var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(response);
            if (request.IsSuccessStatusCode)
            {
                return messageResponse;
            }
            return Result.Failure<MessageResponse>(new Error("Mailgun.SendingError", $"Mailgun message response was: {messageResponse?.Message}"));
        }
        catch (Exception ex)
        {
            return Result.Failure<MessageResponse>(new Error("Mailgun.FailedSend", $"Error sending email message to mailgun: {ex.Message}"));
        }
    }

    private MultipartFormDataContent CreateMessageData(EmailMessage message)
    {
        MultipartFormDataContent data = new()
        {
            { new StringContent(message.Sender.Value), "from" },
            { new StringContent(GetRecipients(message.To)), "to" },
            { new StringContent(message.Subject), "subject" },
            { new StringContent(message.Body), "html" }
        };
        var tags = message.Tags.ToList();
        if (tags.Count > 0)
        {
            // Mailgun doesn't support multiple tags per message so only add one
            data.Add(new StringContent(tags[0]), "o:tag");
        }
        if (_isTestMode && _useTestModeHeader)
        {
            data.Add(new StringContent("yes"), "o:testmode");
        }

        return data;
    }
    private static string GetRecipients(IEnumerable<Email> recipients)
    {
        var recipientEmails = recipients.Select(r => r.Value).ToArray();
        return string.Join(',', recipientEmails);

    }
}
