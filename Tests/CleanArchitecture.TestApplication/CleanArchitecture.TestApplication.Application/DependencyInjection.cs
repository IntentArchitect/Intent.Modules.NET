using System.Reflection;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using CleanArchitecture.TestApplication.Application.Common.Validation;
using CleanArchitecture.TestApplication.Application.Implementation.Comments;
using CleanArchitecture.TestApplication.Application.Implementation.ServiceDispatch;
using CleanArchitecture.TestApplication.Application.Interfaces.Comments;
using CleanArchitecture.TestApplication.Application.Interfaces.ServiceDispatch;
using CleanArchitecture.TestApplication.Domain.Services;
using CleanArchitecture.TestApplication.Domain.Services.Async;
using CleanArchitecture.TestApplication.Domain.Services.DDD;
using CleanArchitecture.TestApplication.Domain.Services.DefaultDiagram;
using CleanArchitecture.TestApplication.Domain.Services.DomainServices;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application
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
            services.AddTransient<ITestDomainService, TestDomainService>();
            services.AddTransient<ICommentTestService, CommentTestService>();
            services.AddTransient<IServiceDispatchService, ServiceDispatchService>();
            return services;
        }
    }
}