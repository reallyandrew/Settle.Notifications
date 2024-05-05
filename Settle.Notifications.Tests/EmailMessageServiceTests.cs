using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Settle.Notifications.Core;
using Settle.Notifications.Core.Exceptions;
using Settle.Notifications.Core.Shared;
using Settle.Notifications.Core.ValueObjects;
using Settle.Notifications.Emails;
using TestApp;

namespace Settle.Notifications.Tests;

public class EmailMessageServiceTests
{
    private readonly IEmailSender _emailSender;
    private readonly Dictionary<string, string?> _settings;
    private readonly ILogger<EmailMessageService> _logger;
    public EmailMessageServiceTests()
    {
        _settings = new Dictionary<string, string?>
        {
            { "Notifications:DefaultEmailSender", "example@example.com" },
            { "Notifications:TestMode:IsEnabled","true" },
            { "Notifications:TestMode:DefaultRecipient","test@example.com" },
            { "Notifications:TestMode:AllowedEmailDomains","bloggs.com" },
            { "Notifications:TestMode:AllowedRecipients","test@test.com" }
        };

        _emailSender = Substitute.For<IEmailSender>();
        _logger = Substitute.For<ILogger<EmailMessageService>>();
    }

    [Fact]
    public void Constructor_MissingDefaultSender_ThrowsInvalidConfigurationException()
    {
        // Arrange
        _settings["Notifications:DefaultEmailSender"] = string.Empty;
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();


        // Act
        var act = () => new EmailMessageService(configuration, _emailSender, _logger);

        // Assert
        act.Should().ThrowExactly<InvalidConfigurationException>();
    }
    [Fact]
    public void Constructor_DefaultTestRecipientMissing_ThrowsMissingConfigurationException()
    {
        // Arrange
        _settings["Notifications:TestMode:DefaultRecipient"] = string.Empty;
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();

        // Act
        var act = () => new EmailMessageService(configuration, _emailSender, _logger);

        // Assert
        act.Should().ThrowExactly<MissingConfigurationException>();
    }
    [Fact]
    public void Constructor_DefaultTestRecipientInvalid_ThrowsInvalidConfigurationException()
    {
        // Arrange
        _settings["Notifications:TestMode:DefaultRecipient"] = "test_example.com";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();

        // Act
        var act = () => new EmailMessageService(configuration, _emailSender, _logger);

        // Assert
        act.Should().ThrowExactly<InvalidConfigurationException>();
    }

    [Fact]
    public async Task SendEmailAsync_RecipientReplacedInTestMode()
    {
        // Arrange
        _settings["Notifications:TestMode:AllowedEmailDomains"] = string.Empty;
        _settings["Notifications:TestMode:AllowedRecipients"] = string.Empty;
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("fake@fake.com");
        const string body = "Test body";
        EmailMessage? capturedMessage = null;
        _emailSender.When(e => e.SendAsync(Arg.Do<EmailMessage>(msg => capturedMessage = msg)))
            .Do(call => call.Arg<EmailMessage>());
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Success(new MessageResponse("Message sent", "123456"))));
        var emailResult = Email.Create("test@example.com");
        var recipientList = new List<Email> { emailResult.Value };
        // Act
        var result = await service.SendMessageAsync(recipientResult.Value, "Test subject", body);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(body);
        capturedMessage.Should().NotBeNull();
        capturedMessage!.To.Should().BeEquivalentTo(recipientList);
    }
    [Fact]
    public async Task SendEmailAsync_RecipientNotReplacedWhenDomainAllowed()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("test@test.com");
        const string body = "Test body";
        EmailMessage? capturedMessage = null;
        _emailSender.When(e => e.SendAsync(Arg.Do<EmailMessage>(msg => capturedMessage = msg)))
            .Do(call => call.Arg<EmailMessage>());
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Success(new MessageResponse("Message sent", "123456"))));

        var recipientList = new List<Email> { recipientResult.Value };
        // Act
        var result = await service.SendMessageAsync(recipientResult.Value, "Test subject", body);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(body);
        capturedMessage.Should().NotBeNull();
        capturedMessage!.To.Should().BeEquivalentTo(recipientList);
    }
    [Fact]
    public async Task SendEmailAsync_RecipientNotReplacedWhenEmailAllowed()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("joe@bloggs.com");
        const string body = "Test body";
        EmailMessage? capturedMessage = null;
        _emailSender.When(e => e.SendAsync(Arg.Do<EmailMessage>(msg => capturedMessage = msg)))
            .Do(call => call.Arg<EmailMessage>());
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Success(new MessageResponse("Message sent", "123456"))));

        var recipientList = new List<Email> { recipientResult.Value };
        // Act
        var result = await service.SendMessageAsync(recipientResult.Value, "Test subject", body);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(body);
        capturedMessage.Should().NotBeNull();
        capturedMessage!.To.Should().BeEquivalentTo(recipientList);
    }
    [Fact]
    public async Task SendEmailAsync_RecipientNotReplacedWhenNotInTestMode()
    {
        // Arrange
        _settings["Notifications:TestMode:IsEnabled"] = "false";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("fake@fake.com");
        const string body = "Test body";
        EmailMessage? capturedMessage = null;
        _emailSender.When(e => e.SendAsync(Arg.Do<EmailMessage>(msg => capturedMessage = msg)))
            .Do(call => call.Arg<EmailMessage>());
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Success(new MessageResponse("Message sent", "123456"))));

        var recipientList = new List<Email> { recipientResult.Value };
        // Act
        var result = await service.SendMessageAsync(recipientResult.Value, "Test subject", body);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(body);
        capturedMessage.Should().NotBeNull();
        capturedMessage!.To.Should().BeEquivalentTo(recipientList);
    }
    [Fact]
    public async Task SendEmailAsync_SendAsyncReturnsFailed_ReturnsFailedResult()
    {
        // Arrange
        _settings["Notifications:TestMode:IsEnabled"] = "false";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("fake@fake.com");
        const string body = "Test body";
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Failure<MessageResponse>("Message failed")));

        // Act
        var result = await service.SendMessageAsync(recipientResult.Value, "Test subject", body);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("Message failed");
    }
    [Fact]
    public async Task SendEmailAsync_WithTag_SendAsync_ReturnsSuccess()
    {
        // Arrange
        _settings["Notifications:TestMode:IsEnabled"] = "false";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("fake@fake.com");
        const string body = "Test body";
        const string tag = "tag";
        EmailMessage? capturedMessage = null;
        _emailSender.When(e => e.SendAsync(Arg.Do<EmailMessage>(msg => capturedMessage = msg)))
            .Do(call => call.Arg<EmailMessage>());
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Success(new MessageResponse("Message sent", "123456"))));

        // Act
        var result = await service.SendMessageAsync(recipientResult.Value, "Test subject", body, tag);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedMessage.Should().NotBeNull();
        capturedMessage!.Tags.Should().HaveCount(1);
    }
    [Fact]
    public async Task SendMessageWithTemplateAsync_SendAsync_ReturnsSuccess()
    {
        // Arrange
        _settings["Notifications:TestMode:IsEnabled"] = "false";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("fake@fake.com");
        var model = new SampleTemplateModel("User", "Description");
        const string body = "User: {{ Name }}, Description: {{ Description }}";
        EmailMessage? capturedMessage = null;
        _emailSender.When(e => e.SendAsync(Arg.Do<EmailMessage>(msg => capturedMessage = msg)))
            .Do(call => call.Arg<EmailMessage>());
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Success(new MessageResponse("Message sent", "123456"))));

        // Act
        var result = await service.SendMessageWithTemplateAsync(recipientResult.Value, "Test subject", body, model);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("User: User, Description: Description");
        capturedMessage.Should().NotBeNull();
    }
    [Fact]
    public async Task SendMessageWithTemplateAsync_WithTag_SendAsync_ReturnsSuccess()
    {
        // Arrange
        _settings["Notifications:TestMode:IsEnabled"] = "false";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("fake@fake.com");
        var model = new SampleTemplateModel("User", "Description");
        const string body = "User: {{ Name }}, Description: {{ Description }}";
        const string tag = "tag";
        EmailMessage? capturedMessage = null;
        _emailSender.When(e => e.SendAsync(Arg.Do<EmailMessage>(msg => capturedMessage = msg)))
            .Do(call => call.Arg<EmailMessage>());
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Success(new MessageResponse("Message sent", "123456"))));

        // Act
        var result = await service.SendMessageWithTemplateAsync(recipientResult.Value, "Test subject", body, model, tag);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedMessage.Should().NotBeNull();
        capturedMessage!.Tags.Should().HaveCount(1);
        result.Value.Should().Be("User: User, Description: Description");
    }
    [Fact]
    public async Task SendMessageWithTemplateAsync_SendAsync_TemplateErrorReturnsFailure()
    {
        // Arrange
        _settings["Notifications:TestMode:IsEnabled"] = "false";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("fake@fake.com");
        var model = new SampleTemplateModel("User", "Description");
        const string body = "{% if Name %}User: {{ Name }}, Description: {{ Description }}";
        EmailMessage? capturedMessage = null;
        _emailSender.When(e => e.SendAsync(Arg.Do<EmailMessage>(msg => capturedMessage = msg)))
            .Do(call => call.Arg<EmailMessage>());
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Success(new MessageResponse("Message sent", "123456"))));

        // Act
        var result = await service.SendMessageWithTemplateAsync(recipientResult.Value, "Test subject", body, model);

        // Assert
        result.IsFailure.Should().BeTrue();
        capturedMessage.Should().BeNull();
    }
    [Fact]
    public async Task SendMessageWithBaseTemplateAsync_SendAsync_ReturnsSuccess()
    {
        // Arrange
        _settings["Notifications:TestMode:IsEnabled"] = "false";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("fake@fake.com");
        var model = new SampleTemplateModel("User", "Description");
        const string baseTemplate = "<div>{{Content}}</div>";
        const string body = "User: {{ Name }}, Description: {{ Description }}";
        EmailMessage? capturedMessage = null;
        _emailSender.When(e => e.SendAsync(Arg.Do<EmailMessage>(msg => capturedMessage = msg)))
            .Do(call => call.Arg<EmailMessage>());
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Success(new MessageResponse("Message sent", "123456"))));

        // Act
        var result = await service.SendMessageWithBaseTemplateAsync(recipientResult.Value, "Test subject", body,baseTemplate, model);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("<div>User: User, Description: Description</div>");
        capturedMessage.Should().NotBeNull();
    }
    [Fact]
    public async Task SendMessageWithBaseTemplateAsync_WithTag_SendAsync_ReturnsSuccess()
    {
        // Arrange
        _settings["Notifications:TestMode:IsEnabled"] = "false";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_settings)
            .Build();
        var service = new EmailMessageService(configuration, _emailSender, _logger);
        var recipientResult = Email.Create("fake@fake.com");
        var model = new SampleTemplateModel("User", "Description");
        const string baseTemplate = "<div>{{Content}}</div>";
        const string body = "User: {{ Name }}, Description: {{ Description }}";
        const string tag = "tag";
        EmailMessage? capturedMessage = null;
        _emailSender.When(e => e.SendAsync(Arg.Do<EmailMessage>(msg => capturedMessage = msg)))
            .Do(call => call.Arg<EmailMessage>());
        _emailSender.SendAsync(Arg.Any<EmailMessage>()).Returns(Task.FromResult(Result.Success(new MessageResponse("Message sent", "123456"))));

        // Act
        var result = await service.SendMessageWithBaseTemplateAsync(recipientResult.Value, "Test subject", body,baseTemplate, model, tag);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedMessage.Should().NotBeNull();
        capturedMessage!.Tags.Should().HaveCount(1); 
        result.Value.Should().Be("<div>User: User, Description: Description</div>");
    }
}