using CqrsSpike.Publisher.Application.Common.Models;
using CqrsSpike.Publisher.Application.Orders.CreateOrder;
using CqrsSpike.Publisher.Domain.Entities;
using CqrsSpike.Publisher.Domain.Events;
using CqrsSpike.Publisher.Infrastructure.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CqrsSpike.Scenarios.Infrastructure;
using Xunit;

namespace CqrsSpike.Scenarios;

/// <summary>
/// Domain-event dispatch through the Publisher's real composition (Application + Infrastructure),
/// in-process and without a broker. The generated notification wrapper resolves
/// <c>INotificationHandler&lt;T&gt;</c> from DI on every publish, so these tests add their own
/// handlers beside the sample's <c>OrderPlacedDomainEventHandler</c>. Every test handler is
/// genuinely async: a synchronous throw escapes before any publisher's error handling runs.
/// </summary>
public class DomainEventChecks
{
    [Fact]
    public async Task DomainEvent_AllHandlersRunOneAtATime_WhenAnEventHasMoreThanOne()
    {
        var log = new HandlerLog();
        await using var provider = InProcessApps.Publisher(afterApp: services =>
        {
            services.AddSingleton(log);
            services.AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, FirstProbeHandler>();
            services.AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, SecondProbeHandler>();
        });
        var customer = $"Multi-{Guid.NewGuid():N}";

        await SendAsync(provider, new CreateOrderCommand(customer, 1m));

        Assert.Equal(["first", "second"], log.Entries);
        Assert.Equal(1, log.MaxInFlight);
        Assert.Equal(1, await AuditCountAsync(provider, customer));
    }

    [Fact]
    public async Task DomainEvent_RaisedByAHandler_IsDispatchedInTheSameSave()
    {
        var log = new HandlerLog();
        await using var provider = InProcessApps.Publisher(afterApp: services =>
            services.AddSingleton(log).AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, CascadingHandler>());
        var customer = $"Cascade-{Guid.NewGuid():N}";

        // One SaveChanges, no pipeline: only the dispatch loop's re-scan can reach the cascaded event.
        await using (var scope = provider.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Orders.Add(new Order(customer, 1m));
            await db.SaveChangesAsync();
        }

        // The cascaded event reached both handlers, so the sample handler audited twice.
        Assert.Equal(["raised", "skipped"], log.Entries);
        Assert.Equal(2, await AuditCountAsync(provider, customer));
    }

    [Fact]
    public async Task DomainEvent_HandlerFailure_StopsLaterHandlersAndTheSave()
    {
        var log = new HandlerLog();
        // Registered before the sample's handler, so it runs first; the probe runs last.
        await using var provider = InProcessApps.Publisher(
            beforeApp: services => services.AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, FailingHandler>(),
            afterApp: services => services.AddSingleton(log).AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, FirstProbeHandler>());
        var customer = $"Fail-{Guid.NewGuid():N}";

        // The handler's own exception reaches the caller (D22), not an AggregateException.
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => SendAsync(provider, new CreateOrderCommand(customer, 1m)));
        Assert.Equal(FailingHandler.Message, exception.Message);

        Assert.Empty(log.Entries);
        Assert.Equal(0, await AuditCountAsync(provider, customer));
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.False(await db.Orders.AnyAsync(o => o.CustomerName == customer));
    }

    private static async Task SendAsync(IServiceProvider provider, CreateOrderCommand command)
    {
        await using var scope = provider.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<ISender>().Send(command);
    }

    private static async Task<int> AuditCountAsync(IServiceProvider provider, string customer)
    {
        await using var scope = provider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await db.AuditEntries.CountAsync(a => a.Description == $"Order placed for {customer}");
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

    private sealed class CascadingHandler(HandlerLog log) : INotificationHandler<OrderPlacedDomainEventNotification>
    {
        public async ValueTask Handle(OrderPlacedDomainEventNotification notification, CancellationToken cancellationToken)
        {
            // Raise a second event on the tracked Order once, decided from the entity's own state.
            var order = notification.DomainEvent.Order;
            if (order.DomainEvents.Count == 1)
            {
                order.DomainEvents.Add(new OrderPlacedDomainEvent(order));
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
