using System.Reflection;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Behaviours;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Validation;
using AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.Orders;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
                cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
                cfg.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddSingleton<OrderAddressDtoMapper>();
            services.AddSingleton<OrderCustomerDtoMapper>();
            services.AddSingleton<OrderDiscountDtoMapper>();
            services.AddSingleton<OrderDtoMapper>();
            services.AddSingleton<OrderOrderLineDtoMapper>();
            services.AddSingleton<OrderPaymentDtoMapper>();
            services.AddSingleton<OrderProductCategoryDtoMapper>();
            services.AddSingleton<OrderProductDtoMapper>();
            services.AddSingleton<OrderShipmentDtoMapper>();
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            return services;
        }
    }
}