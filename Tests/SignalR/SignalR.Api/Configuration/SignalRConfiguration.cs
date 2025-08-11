using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SignalR.Api.Hubs;
using SignalR.Api.Hubs.Services;
using SignalR.Application.Interfaces;
using SignalR.Application.Interfaces.Hubs;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.SignalR.SignalRConfiguration", Version = "1.0")]

namespace SignalR.Api.Configuration
{
    public static class SignalRConfiguration
    {
        public static IServiceCollection ConfigureSignalR(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSignalR();
            services.RegisterServices(configuration);
            return services;
        }

        private static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<INotificationsHub, NotificationsHubService>();
        }

        public static IEndpointRouteBuilder MapHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<NotificationsHub>("/hub/notifications");
            return endpoints;
        }
    }
}