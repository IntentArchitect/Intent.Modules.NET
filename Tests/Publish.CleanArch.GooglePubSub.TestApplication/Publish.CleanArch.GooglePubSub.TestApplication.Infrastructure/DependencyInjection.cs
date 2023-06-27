using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Publish.CleanArch.GooglePubSub.TestApplication.Application;
using Publish.CleanArch.GooglePubSub.TestApplication.Application.Common.Interfaces;
using Publish.CleanArch.GooglePubSub.TestApplication.Domain.Common.Interfaces;
using Publish.CleanArch.GooglePubSub.TestApplication.Infrastructure.Configuration;
using Publish.CleanArch.GooglePubSub.TestApplication.Infrastructure.Persistence;
using Publish.CleanArch.GooglePubSub.TestApplication.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Publish.CleanArch.GooglePubSub.TestApplication.Infrastructure
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
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.RegisterGoogleCloudPubSubServices(configuration);
            services.AddSubscribers();
            services.RegisterEventHandlers();
            services.RegisterTopicEvents();
            return services;
        }
    }
}