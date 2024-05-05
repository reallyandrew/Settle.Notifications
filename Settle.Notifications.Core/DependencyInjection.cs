using Microsoft.Extensions.DependencyInjection;
using Settle.Notifications.Emails;

namespace Settle.Notifications;
public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsService(this IServiceCollection services)
    {
        services.AddScoped<IEmailMessageService, EmailMessageService>();
        return services;
    }
}
