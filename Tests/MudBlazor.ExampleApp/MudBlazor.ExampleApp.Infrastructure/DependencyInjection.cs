using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.ExampleApp.Application.Common.Interfaces;
using MudBlazor.ExampleApp.Domain.Common.Interfaces;
using MudBlazor.ExampleApp.Domain.Repositories;
using MudBlazor.ExampleApp.Infrastructure.Persistence;
using MudBlazor.ExampleApp.Infrastructure.Repositories;
using MudBlazor.ExampleApp.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace MudBlazor.ExampleApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}