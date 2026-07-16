using System.Reflection;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scalar.NET10.Bearer.Application.Implementation;
using Scalar.NET10.Bearer.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Scalar.NET10.Bearer.Application
{
    public static class DependencyInjection
    {
        [IntentMerge]
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IWidgetService, WidgetService>();
            return services;
        }
    }
}
