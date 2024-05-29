using System.Reflection;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Behaviours;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using CleanArchitecture.Comprehensive.Application.Implementation.Comments;
using CleanArchitecture.Comprehensive.Application.Implementation.ServiceDispatch;
using CleanArchitecture.Comprehensive.Application.Interfaces.Comments;
using CleanArchitecture.Comprehensive.Application.Interfaces.ServiceDispatch;
using CleanArchitecture.Comprehensive.Domain.Services.Async;
using CleanArchitecture.Comprehensive.Domain.Services.DDD;
using CleanArchitecture.Comprehensive.Domain.Services.DefaultDiagram;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application
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
            services.AddTransient<IAsyncableDomainService, AsyncableDomainService>();
            services.AddTransient<IAccountingDomainService, AccountingDomainService>();
            services.AddTransient<IDataContractDomainService, DataContractDomainService>();
            services.AddTransient<IDomainServiceWithDefault, DomainServiceWithDefault>();
            services.AddTransient<ICommentTestService, CommentTestService>();
            services.AddTransient<IServiceDispatchService, ServiceDispatchService>();
            return services;
        }
    }
}