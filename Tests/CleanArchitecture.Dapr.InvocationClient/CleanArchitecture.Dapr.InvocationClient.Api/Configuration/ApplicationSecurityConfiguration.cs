using CleanArchitecture.Dapr.InvocationClient.Api.Services;
using CleanArchitecture.Dapr.InvocationClient.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.ApplicationSecurityConfiguration", Version = "1.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Api.Configuration
{
    public static class ApplicationSecurityConfiguration
    {
        public static IServiceCollection ConfigureApplicationSecurity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            return services;
        }
    }
}