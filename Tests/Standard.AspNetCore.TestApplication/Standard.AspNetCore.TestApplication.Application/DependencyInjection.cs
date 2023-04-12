using System.Reflection;
using AutoMapper;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.TestApplication.Application.Common.Validation;
using Standard.AspNetCore.TestApplication.Application.Implementation;
using Standard.AspNetCore.TestApplication.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IClassASService, ClassASService>();
            services.AddTransient<IHttpServiceAppliedService, HttpServiceAppliedService>();
            services.AddTransient<INonHttpServiceAppliedService, NonHttpServiceAppliedService>();
            return services;
        }
    }
}