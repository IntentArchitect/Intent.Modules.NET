using System.Reflection;
using FluentValidation;
using FluentValidationTest.Blazor.Client.Common.Validation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.DependencyInjection", Version = "1.0")]

namespace FluentValidationTest.Blazor.Client
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddHttpClients(configuration);
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            return services;
        }
    }
}