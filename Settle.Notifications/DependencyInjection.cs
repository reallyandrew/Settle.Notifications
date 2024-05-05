using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Settle.Notifications;
public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsService(this IServiceCollection services)
    {
        services.AddScoped<IEmailMessageService, EmailMessageService>();
        return services;
    }
}
