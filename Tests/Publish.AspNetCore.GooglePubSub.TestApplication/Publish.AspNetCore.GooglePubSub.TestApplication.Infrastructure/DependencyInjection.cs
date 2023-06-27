using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publish.AspNetCore.GooglePubSub.TestApplication.Application;
using Publish.AspNetCore.GooglePubSub.TestApplication.Domain.Common.Interfaces;
using Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure.Configuration;
using Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Infrastructure
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
            services.RegisterGoogleCloudPubSubServices(configuration);
            services.AddSubscribers();
            services.RegisterEventHandlers();
            services.RegisterTopicEvents();
            return services;
        }
    }
}