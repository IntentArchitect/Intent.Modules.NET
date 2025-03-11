using System.Reflection;
using AspNetControllers.SecuredByDefault.Application.Implementation;
using AspNetControllers.SecuredByDefault.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AspNetControllers.SecuredByDefault.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITestService, TestService>();
            return services;
        }
    }
}