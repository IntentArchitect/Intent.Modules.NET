using System.Reflection;
using FluentValidation;
using GrpcServer.Application.Common.Behaviours;
using GrpcServer.Application.Common.Validation;
using GrpcServer.Application.Implementation;
using GrpcServer.Application.Implementation.TypeTestingServices;
using GrpcServer.Application.Interfaces;
using GrpcServer.Application.Interfaces.TypeTestingServices;
using GrpcServer.Domain.Services;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace GrpcServer.Application
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
                cfg.AddOpenBehavior(typeof(EventBusPublishBehaviour<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehaviour<,>));
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<ITagsService, TagsService>();
            services.AddTransient<IForBinaryService, ForBinaryService>();
            services.AddTransient<IForBoolService, ForBoolService>();
            services.AddTransient<IForByteService, ForByteService>();
            services.AddTransient<IForCharService, ForCharService>();
            services.AddTransient<IForDateOnlyService, ForDateOnlyService>();
            services.AddTransient<IForDateTimeService, ForDateTimeService>();
            services.AddTransient<IForDateTimeOffsetService, ForDateTimeOffsetService>();
            services.AddTransient<IForDecimalService, ForDecimalService>();
            services.AddTransient<IForDictionaryService, ForDictionaryService>();
            services.AddTransient<IForDoubleService, ForDoubleService>();
            services.AddTransient<IForEnumService, ForEnumService>();
            services.AddTransient<IForFloatService, ForFloatService>();
            services.AddTransient<IForGuidService, ForGuidService>();
            services.AddTransient<IForIntService, ForIntService>();
            services.AddTransient<IForLongService, ForLongService>();
            services.AddTransient<IForObjectService, ForObjectService>();
            services.AddTransient<IForPagedResultService, ForPagedResultService>();
            services.AddTransient<IForShortService, ForShortService>();
            services.AddTransient<IForStringService, ForStringService>();
            services.AddTransient<IForTimeSpanService, ForTimeSpanService>();
            return services;
        }
    }
}