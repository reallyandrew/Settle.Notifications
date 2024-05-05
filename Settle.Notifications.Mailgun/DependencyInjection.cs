using Microsoft.Extensions.DependencyInjection;
using Settle.Notifications.Core;

namespace Settle.Notifications.Mailgun;
public static class DependencyInjection
{
    public static IServiceCollection AddMailgunEmailSender(this IServiceCollection services)
    {
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddHttpClient();
        return services;
    }
}
