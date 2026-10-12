using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PublisherCurrentUser = CqrsSpike.Publisher.Application.Common.Interfaces.ICurrentUser;
using PublisherCurrentUserService = CqrsSpike.Publisher.Application.Common.Interfaces.ICurrentUserService;
using PublisherDbContext = CqrsSpike.Publisher.Infrastructure.Persistence.ApplicationDbContext;
using PublisherMessageBus = CqrsSpike.Publisher.Application.Common.Eventing.IMessageBus;
using SubscriberCurrentUser = CqrsSpike.Subscriber.Application.Common.Interfaces.ICurrentUser;
using SubscriberCurrentUserService = CqrsSpike.Subscriber.Application.Common.Interfaces.ICurrentUserService;
using SubscriberDbContext = CqrsSpike.Subscriber.Infrastructure.Persistence.ApplicationDbContext;
using SubscriberMessageBus = CqrsSpike.Subscriber.Application.Common.Eventing.IMessageBus;

namespace CqrsSpike.Scenarios.Infrastructure;

/// <summary>
/// Each app's real composition (AddApplication + AddInfrastructure) in a plain service provider,
/// without a host or broker. Integration messages are discarded and the caller is anonymous.
/// Pass a SQL Server connection string to replace EF Core InMemory.
/// </summary>
internal static class InProcessApps
{
    private static readonly IConfiguration Configuration = new ConfigurationBuilder().Build();

    public static ServiceProvider Publisher(
        string? sqlServer = null,
        Action<IServiceCollection>? beforeApp = null,
        Action<IServiceCollection>? afterApp = null)
    {
        var services = Start(beforeApp);
        CqrsSpike.Publisher.Application.DependencyInjection.AddApplication(services, Configuration);
        CqrsSpike.Publisher.Infrastructure.DependencyInjection.AddInfrastructure(services, Configuration);
        if (sqlServer is not null)
        {
            SqlServerDatabase.Replace<PublisherDbContext>(services, sqlServer);
        }

        afterApp?.Invoke(services);
        services.AddScoped<PublisherCurrentUserService, AnonymousPublisherUser>();
        services.AddScoped<PublisherMessageBus, DiscardingPublisherBus>();
        return services.BuildServiceProvider();
    }

    public static ServiceProvider Subscriber(
        string? sqlServer = null,
        Action<IServiceCollection>? afterApp = null)
    {
        var services = Start(null);
        CqrsSpike.Subscriber.Application.DependencyInjection.AddApplication(services, Configuration);
        CqrsSpike.Subscriber.Infrastructure.DependencyInjection.AddInfrastructure(services, Configuration);
        if (sqlServer is not null)
        {
            SqlServerDatabase.Replace<SubscriberDbContext>(services, sqlServer);
        }

        afterApp?.Invoke(services);
        services.AddScoped<SubscriberCurrentUserService, AnonymousSubscriberUser>();
        services.AddScoped<SubscriberMessageBus, DiscardingSubscriberBus>();
        return services.BuildServiceProvider();
    }

    private static ServiceCollection Start(Action<IServiceCollection>? beforeApp)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Configuration);
        beforeApp?.Invoke(services);
        return services;
    }

    private sealed class AnonymousPublisherUser : PublisherCurrentUserService
    {
        public Task<PublisherCurrentUser?> GetAsync() => Task.FromResult<PublisherCurrentUser?>(null);
        public Task<bool> IsInRoleAsync(string role) => Task.FromResult(false);
        public Task<bool> AuthorizeAsync(string policy) => Task.FromResult(false);
    }

    private sealed class AnonymousSubscriberUser : SubscriberCurrentUserService
    {
        public Task<SubscriberCurrentUser?> GetAsync() => Task.FromResult<SubscriberCurrentUser?>(null);
        public Task<bool> IsInRoleAsync(string role) => Task.FromResult(false);
        public Task<bool> AuthorizeAsync(string policy) => Task.FromResult(false);
    }

    private sealed class DiscardingPublisherBus : PublisherMessageBus
    {
        public void Publish<TMessage>(TMessage message) where TMessage : class { }
        public void Send<TMessage>(TMessage message) where TMessage : class { }
        public void Send<TMessage>(TMessage message, Uri address) where TMessage : class { }
        public Task FlushAllAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class DiscardingSubscriberBus : SubscriberMessageBus
    {
        public void Publish<TMessage>(TMessage message) where TMessage : class { }
        public void Send<TMessage>(TMessage message) where TMessage : class { }
        public void Send<TMessage>(TMessage message, Uri address) where TMessage : class { }
        public Task FlushAllAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
