using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.ServiceCallHandlers.Application.Common.Validation;
using Standard.AspNetCore.ServiceCallHandlers.Application.Implementation;
using Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.PeopleServiceHandlers;
using Standard.AspNetCore.ServiceCallHandlers.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), lifetime: ServiceLifetime.Transient);
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IValidatorProvider, ValidatorProvider>();
            services.AddTransient<CreatePersonSCH>();
            services.AddTransient<FindPersonByIdSCH>();
            services.AddTransient<FindPeopleSCH>();
            services.AddTransient<UpdatePersonSCH>();
            services.AddTransient<DeletePersonSCH>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IPeopleService, PeopleService>();
            return services;
        }
    }
}