using System.Reflection;
using AutoMapper;
using Entities.PrivateSetters.MongoDb.Application.Implementation;
using Entities.PrivateSetters.MongoDb.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTransient<IInvoiceService, InvoiceService>();
            services.AddTransient<ITagService, TagService>();
            return services;
        }
    }
}