using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Settle.Notifications;
using Settle.Notifications.Core.ValueObjects;
using Settle.Notifications.Emails;
using Settle.Notifications.Mailgun;

namespace TestApp;

internal class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<Serilog.ILogger>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailMessageService>();

            logger.Information("Application is started");
            var baseTemplate = "<html><head><title>Something</title></head><body>{{ Content }}</body></html>";
            var template = @"
<p>Here's my template</p>
<ul>
<li>{{ Name }}</li>
<li>{{ Description }}</li>
</ul>";
            var templateModel = new SampleTemplateModel("Andrew", "Example description");
            var recipient = Email.Create("recipient@test.com");
            var templateResult = await emailService.SendMessageWithTemplateAsync(recipient.Value, "Test message", template, templateModel);
            if (templateResult.IsSuccess)
            {
                logger.Information("Body created: {body}", templateResult.Value);
            }
            else
            {
                logger.Error("Failed: {error}", templateResult.Error);
            }

            var baseTemplateResult = await emailService.SendMessageWithBaseTemplateAsync(recipient.Value, "Test message", template, baseTemplate, templateModel);
            if (baseTemplateResult.IsSuccess)
            {
                logger.Information("Body created: {body}", baseTemplateResult.Value);
            }
            else
            {
                logger.Error("Failed: {error}", baseTemplateResult.Error);
            }
        }



        Console.ReadKey();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration))
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMailgunEmailSender();
                services.AddNotificationsService();
            });
    }
}
