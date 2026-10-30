using FluentValidation;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit;
using TestMediator.Publisher.Infrastructure.Dispatch.Behaviours;
using TestMediator.Publisher.Application.Common.Eventing;
using TestMediator.Publisher.Application.Common.Interfaces;
using TestMediator.Publisher.Application.Common.Models;
using TestMediator.Publisher.Application.Orders;
using TestMediator.Publisher.Application.Orders.CreateOrder;
using TestMediator.Publisher.Application.Orders.GetOrders;
using TestMediator.Publisher.Domain;
using TestMediator.Publisher.Domain.Common;
using TestMediator.Publisher.Domain.Events;
using TestMediator.Publisher.Infrastructure.Persistence;

namespace TestMediator.Publisher.IntegrationTests;

/// <summary>
/// Spike BehaviourChecks, ported: single pieces of the generated pipeline that need neither a broker nor a database.
/// </summary>
public class BehaviourChecks
{
    // Behaviours are Transient, but timing must stay per call even if an instance is reused.
    [Fact]
    public async Task PerformanceBehaviour_TimesEachCallSeparately_WhenAnInstanceIsReused()
    {
        var logger = new CountingLogger<PerformanceBehaviour<GetOrdersQuery, List<OrderDto>>>();
        var behaviour = new PerformanceBehaviour<GetOrdersQuery, List<OrderDto>>(
            logger, new NoCurrentUser(), new ConfigurationBuilder().Build());

        // Four 150 ms calls: each is well under the 500 ms "long running" threshold, while time
        // accumulated across calls (the defect) passes it on the fourth (~600 ms).
        for (var i = 0; i < 4; i++)
        {
            await behaviour.Handle(new GetOrdersQuery(), async (_, ct) =>
            {
                await Task.Delay(150, ct);
                return new List<OrderDto>();
            }, CancellationToken.None);
        }

        Assert.Equal(0, logger.Warnings);
    }

    // Async rules share the scoped DbContext, which rejects a second concurrent operation.
    [Fact]
    public async Task ValidationBehaviour_RunsValidatorsOneAtATime()
    {
        var probe = new ConcurrencyProbe();
        var behaviour = new ValidationBehaviour<CreateOrderCommand, Guid>(
            [new AsyncRuleValidator(probe), new AsyncRuleValidator(probe)]);

        await behaviour.Handle(new CreateOrderCommand("A", 1m), (_, _) => ValueTask.FromResult(Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(2, probe.Calls);
        Assert.Equal(1, probe.MaxInFlight);
    }

    [Fact]
    public async Task ValidationBehaviour_SkipsValidators_ForARequestThatBypassesValidation()
    {
        var probe = new ConcurrencyProbe();
        var behaviour = new ValidationBehaviour<BypassedCommand, Guid>([new BypassedValidator(probe)]);

        await behaviour.Handle(new BypassedCommand(), (_, _) => ValueTask.FromResult(Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(0, probe.Calls);
    }

    [Fact]
    public void DomainEventNotificationFactory_Throws_WhenAnEventHasNoNotification()
    {
        Assert.Throws<InvalidOperationException>(() => DomainEventNotificationFactory.Create(new UnmappedDomainEvent()));
    }

    private sealed class ConcurrencyProbe
    {
        private int _inFlight;
        private int _maxInFlight;
        private int _calls;

        public int MaxInFlight => _maxInFlight;
        public int Calls => _calls;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _calls);
            var inFlight = Interlocked.Increment(ref _inFlight);
            int seen;
            while (inFlight > (seen = _maxInFlight) && Interlocked.CompareExchange(ref _maxInFlight, inFlight, seen) != seen)
            {
            }

            await Task.Delay(50, cancellationToken);
            Interlocked.Decrement(ref _inFlight);
        }
    }

    private sealed class AsyncRuleValidator : AbstractValidator<CreateOrderCommand>
    {
        public AsyncRuleValidator(ConcurrencyProbe probe)
        {
            RuleFor(x => x.CustomerName).MustAsync(async (_, ct) =>
            {
                await probe.RunAsync(ct);
                return true;
            });
        }
    }

    private sealed record BypassedCommand : ICommand<Guid>, IBypassValidation;

    private sealed class BypassedValidator : AbstractValidator<BypassedCommand>
    {
        public BypassedValidator(ConcurrencyProbe probe)
        {
            RuleFor(x => x).MustAsync(async (_, ct) =>
            {
                await probe.RunAsync(ct);
                return false;
            });
        }
    }

    private sealed class UnmappedDomainEvent : DomainEvent
    {
    }

    private sealed class NoCurrentUser : ICurrentUserService
    {
        public Task<ICurrentUser?> GetAsync() => Task.FromResult<ICurrentUser?>(null);
        public Task<bool> IsInRoleAsync(string role) => Task.FromResult(false);
        public Task<bool> AuthorizeAsync(string policy) => Task.FromResult(false);
    }

    private sealed class CountingLogger<T> : ILogger<T>
    {
        public int Warnings { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Warning)
            {
                Warnings++;
            }
        }
    }
}

/// <summary>
/// Spike DomainEventChecks, ported: domain-event dispatch through the Publisher's real Application and Infrastructure
/// composition in a plain service provider, on EF Core InMemory and without a broker. The generated notification wrapper
/// resolves INotificationHandler from DI on every publish, so each test adds its own handlers beside the sample's
/// OrderPlacedDomainEventHandler. Every test handler is genuinely async: a synchronous throw escapes before any
/// publisher's error handling runs.
/// </summary>
public class DomainEventChecks
{
    [Fact]
    public async Task DomainEvent_AllHandlersRunOneAtATime_WhenAnEventHasMoreThanOne()
    {
        var log = new HandlerLog();
        await using var provider = InProcessPublisher.Build(afterApp: services =>
        {
            services.AddSingleton(log);
            services.AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, FirstProbeHandler>();
            services.AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, SecondProbeHandler>();
        });

        var before = await AuditCountAsync(provider);
        await SendAsync(provider, new CreateOrderCommand("Multi", 1m));

        Assert.Equal(["first", "second"], log.Entries);
        Assert.Equal(1, log.MaxInFlight);
        Assert.Equal(before + 1, await AuditCountAsync(provider));
    }

    [Fact]
    public async Task DomainEvent_RaisedByAHandler_IsDispatchedInTheSameSave()
    {
        var log = new HandlerLog();
        await using var provider = InProcessPublisher.Build(afterApp: services =>
            services.AddSingleton(log).AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, CascadingHandler>());

        // One SaveChanges, no pipeline: only the dispatch loop's re-scan can reach the cascaded event.
        await using (var scope = provider.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Orders.Add(new Order("Cascade", 1m));
            await db.SaveChangesAsync();
        }

        // The cascaded event reached both handlers, so the sample handler audited twice.
        Assert.Equal(["raised", "skipped"], log.Entries);
        Assert.Equal(2, await AuditCountAsync(provider));
    }

    [Fact]
    public async Task DomainEvent_HandlerFailure_StopsLaterHandlersAndTheSave()
    {
        var log = new HandlerLog();
        // Registered before the sample's handler, so it runs first; the probe runs last.
        await using var provider = InProcessPublisher.Build(
            beforeApp: services => services.AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, FailingHandler>(),
            afterApp: services => services.AddSingleton(log).AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, FirstProbeHandler>());

        // The handler's own exception reaches the caller, not an AggregateException.
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => SendAsync(provider, new CreateOrderCommand("Fail", 1m)));
        Assert.Equal(FailingHandler.Message, exception.Message);

        Assert.Empty(log.Entries);
        Assert.Equal(0, await AuditCountAsync(provider));
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.False(await db.Orders.AnyAsync());
    }

    private static async Task SendAsync(IServiceProvider provider, CreateOrderCommand command)
    {
        await using var scope = provider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<ISender>().Send(command);
    }

    private static async Task<int> AuditCountAsync(IServiceProvider provider)
    {
        await using var scope = provider.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().AuditEntries.CountAsync();
    }

    private sealed class HandlerLog
    {
        private int _inFlight;
        private int _maxInFlight;

        public List<string> Entries { get; } = [];
        public int MaxInFlight => _maxInFlight;

        public async Task RecordAsync(string entry, CancellationToken cancellationToken)
        {
            var inFlight = Interlocked.Increment(ref _inFlight);
            int seen;
            while (inFlight > (seen = _maxInFlight) && Interlocked.CompareExchange(ref _maxInFlight, inFlight, seen) != seen)
            {
            }

            await Task.Delay(30, cancellationToken);
            lock (Entries)
            {
                Entries.Add(entry);
            }

            Interlocked.Decrement(ref _inFlight);
        }
    }

    private sealed class FirstProbeHandler(HandlerLog log) : INotificationHandler<OrderPlacedDomainEventNotification>
    {
        public async ValueTask Handle(OrderPlacedDomainEventNotification notification, CancellationToken cancellationToken) =>
            await log.RecordAsync("first", cancellationToken);
    }

    private sealed class SecondProbeHandler(HandlerLog log) : INotificationHandler<OrderPlacedDomainEventNotification>
    {
        public async ValueTask Handle(OrderPlacedDomainEventNotification notification, CancellationToken cancellationToken) =>
            await log.RecordAsync("second", cancellationToken);
    }

    private sealed class CascadingHandler(HandlerLog log, ApplicationDbContext db) : INotificationHandler<OrderPlacedDomainEventNotification>
    {
        public async ValueTask Handle(OrderPlacedDomainEventNotification notification, CancellationToken cancellationToken)
        {
            // Raise a second event on the tracked Order once, decided from the entity's own state.
            var order = db.ChangeTracker.Entries<Order>().Single().Entity;
            if (order.DomainEvents.Count == 1)
            {
                order.DomainEvents.Add(new OrderPlacedDomainEvent());
                await log.RecordAsync("raised", cancellationToken);
            }
            else
            {
                await log.RecordAsync("skipped", cancellationToken);
            }
        }
    }

    private sealed class FailingHandler : INotificationHandler<OrderPlacedDomainEventNotification>
    {
        public const string Message = "Domain-event handler failed.";

        public async ValueTask Handle(OrderPlacedDomainEventNotification notification, CancellationToken cancellationToken)
        {
            await Task.Yield();
            throw new InvalidOperationException(Message);
        }
    }
}

/// <summary>
/// The Publisher's real composition (AddApplication + AddInfrastructure) in a plain service provider, with no host or
/// broker: EF Core InMemory replaces SQL Server, integration messages are discarded and the caller is anonymous.
/// </summary>
internal static class InProcessPublisher
{
    public static ServiceProvider Build(Action<IServiceCollection>? beforeApp = null, Action<IServiceCollection>? afterApp = null)
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        beforeApp?.Invoke(services);
        TestMediator.Publisher.Application.DependencyInjection.AddApplication(services, configuration);
        TestMediator.Publisher.Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);

        // EF Core 9+ accumulates option configurations per context, so the SQL Server one must go too.
        var database = $"in-process-{Guid.NewGuid():N}";
        services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
        services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(database).UseLazyLoadingProxies());

        afterApp?.Invoke(services);
        services.AddScoped<ICurrentUserService, AnonymousUser>();
        services.AddScoped<IMessageBus, DiscardingBus>();
        return services.BuildServiceProvider();
    }

    private sealed class AnonymousUser : ICurrentUserService
    {
        public Task<ICurrentUser?> GetAsync() => Task.FromResult<ICurrentUser?>(null);
        public Task<bool> IsInRoleAsync(string role) => Task.FromResult(false);
        public Task<bool> AuthorizeAsync(string policy) => Task.FromResult(false);
    }

    private sealed class DiscardingBus : IMessageBus
    {
        public void Publish<TMessage>(TMessage message) where TMessage : class { }
        public void Send<TMessage>(TMessage message) where TMessage : class { }
        public void Send<TMessage>(TMessage message, Uri address) where TMessage : class { }
        public Task FlushAllAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
