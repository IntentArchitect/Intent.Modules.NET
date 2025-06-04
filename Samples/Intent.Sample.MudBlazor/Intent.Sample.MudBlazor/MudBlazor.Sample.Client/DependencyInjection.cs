using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Client.Common.Validation;
using MudBlazor.Sample.Client.HttpClients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.DependencyInjection", Version = "1.0")]

namespace MudBlazor.Sample.Client
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