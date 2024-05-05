# Settle.Notifications

Send notifications via Email or SMS (coming soon).

## Send an email

```csharp
// Inject IEmailMessageService via DI
internal sealed class MyEmailService
{
    private readonly IEmailMessageService _emailService;

    public MyEmailService(IEmailMessageService emailService)
    {
    _emailService=emailService;
    }

    public string SendTestEmail(){
        var emailResult = Email.Create("test@example.com");
        var subject = "Test email";
        var body = "This is a test email message";

        var result = _emailService.SendMessageAsync(emailResult.Value, subject, body);
    }
}
```

## Send a templated email

This option allows you to define a template and the service will replace matching values for you. This uses the Liquid templating engine to process the file.

```csharp
internal sealed class MyEmailService
{
    public string SendTemplatedTestEmail(){
        var emailResult = Email.Create("test@example.com");
        var subject = "Test email";
        var body = @"
                <p>Here's my template</p>
                <ul>
                <li>{{ Name }}</li>
                <li>{{ Description }}</li>
                </ul>";

        var templateModel = new SampleTemplateModel("Test User","Test Description");

        var result = _emailService.SendMessageWithTemplateAsync(emailResult.Value, subject, body, templateModel);
    }
}

public sealed record SampleTemplateModel(string Name, string Description):ITemplateModel
{
}
```

## Send a templated email with a base template

