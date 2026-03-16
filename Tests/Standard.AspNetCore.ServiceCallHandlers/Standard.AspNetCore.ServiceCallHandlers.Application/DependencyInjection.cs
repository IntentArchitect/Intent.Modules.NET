using System.Reflection;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Standard.AspNetCore.ServiceCallHandlers.Application.Common.Validation;
using Standard.AspNetCore.ServiceCallHandlers.Application.Implementation;
using Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.Addresses;
using Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.AddressesServiceHandlers;
using Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.AddressServiceCustomHandlers;
using Standard.AspNetCore.ServiceCallHandlers.Application.Implementation.PeopleServiceHandlers;
using Standard.AspNetCore.ServiceCallHandlers.Application.Interfaces;
using Standard.AspNetCore.ServiceCallHandlers.Application.Interfaces.Addresses;

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
            services.AddTransient<CreateAddressSCH>();
            services.AddTransient<UpdateAddressSCH>();
            services.AddTransient<FindAddressByIdSCH>();
            services.AddTransient<FindAddressesSCH>();
            services.AddTransient<DeleteAddressSCH>();
            services.AddTransient<CreateAddressNoTokenSCH>();
            services.AddTransient<UpdateAddressSyncSCH>();
            services.AddTransient<CreatePersonSCH>();
            services.AddTransient<FindPersonByIdSCH>();
            services.AddTransient<FindPeopleSCH>();
            services.AddTransient<UpdatePersonSCH>();
            services.AddTransient<DeletePersonSCH>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IAddressesService, AddressesService>();
            services.AddTransient<IPeopleService, PeopleService>();
            services.AddTransient<IAddressServiceCustomService, AddressServiceCustomService>();
            return services;
        }
    }
}