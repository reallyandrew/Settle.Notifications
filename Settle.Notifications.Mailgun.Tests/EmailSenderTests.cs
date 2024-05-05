using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Settle.Notifications.Core;
using Settle.Notifications.Core.Exceptions;
using Settle.Notifications.Core.ValueObjects;
using System.Net;

namespace Settle.Notifications.Mailgun.Tests;

public class EmailSenderTests
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Dictionary<string, string?> _settings;
    private readonly MockHttpMessageHandler _httpMessageHandler;
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        public StringContent ResponseContent { get; set; } = new StringContent("{\"key\":\"value\"}");
        public string RequestEndpoint { get; set; } = "Your request endpoint";
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post && request.RequestUri!.ToString().Contains(RequestEndpoint))
            {
                // Simulate a response for POST request
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = ResponseContent
                };
                return await Task.FromResult(response);
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound); // Default to not found for other cases
        }
    }

    public EmailSenderTests()
    {
        _httpMessageHandler = new MockHttpMessageHandler();
        var _httpClient = new HttpClient(_httpMessageHandler);

        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(_httpClient);
        _settings = new Dictionary<string, string?>
        {
            { "Notifications:DefaultEmailSender", "example@example.com" },
            { "Notifications:TestMode:IsEnabled","true" },
            { "Notifications:TestMode:DefaultRecipient","test@example.com" },
            { "Notifications:TestMode:AllowedEmailDomains","bloggs.com" },
            { "Notifications:TestMode:AllowedRecipients","test@test.com" },
            { "Notifications:Mailgun:ApiKey","ValidApiKey" },
            { "Notifications:Mailgun:Domain","mg.test.com" },
            { "Notifications:Mailgun:UseTestModeHeader","true" },
            { "Notifications:Mailgun:Region","EU" }
        };
    }

    [Fact]
    public async Task SendMessage_Valid_ReturnsSuccess()
    {
        // Arrange
        _settings["Notifications:Mailgun:ApiKey"] = "ValidApiKey";
        _settings["Notifications:Mailgun:Domain"] = "domain";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        _httpMessageHandler.ResponseContent = new StringContent(@"{""id"": ""message-id"",""message"": ""Queued. Thank you.""}");
        _httpMessageHandler.RequestEndpoint = "https://api.eu.mailgun.net/v3/domain/messages";

        var emailSender = new EmailSender(_httpClientFactory, configuration);
        var recipient = Email.Create("test@test.com");
        var sender = Email.Create("sender@test.com");
        var emailMessage = EmailMessage.Create(recipient.Value, "Test message", "Message body", sender.Value);

        // Act
        var result = await emailSender.SendAsync(emailMessage);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }


    [Fact]
    public void Constructor_MissingPassword_ThrowsMissingConfigurationException()
    {
        // Arrange
        _settings["Notifications:Mailgun:ApiKey"] = string.Empty;
        _settings["Notifications:Mailgun:Domain"] = "mg.test.com";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();

        // Act
        var act = () => new EmailSender(_httpClientFactory, configuration);

        // Assert
        act.Should().ThrowExactly<MissingConfigurationException>();
    }
    [Fact]
    public void Constructor_MissingDomain_ThrowsMissingConfigurationException()
    {
        // Arrange
        _settings["Notifications:Mailgun:ApiKey"] = "ValidApiKey";
        _settings["Notifications:Mailgun:Domain"] = null;
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();

        // Act
        var act = () => new EmailSender(_httpClientFactory, configuration);

        // Assert
        act.Should().ThrowExactly<MissingConfigurationException>();
    }
}