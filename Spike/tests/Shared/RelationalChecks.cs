using CqrsSpike.Publisher.Application.Common.Models;
using CqrsSpike.Publisher.Application.Orders.CreateOrder;
using CqrsSpike.Scenarios.Infrastructure;
using CqrsSpike.Subscriber.Application.OrderRecords.RecordOrder;
using Mediator;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using PublisherDbContext = CqrsSpike.Publisher.Infrastructure.Persistence.ApplicationDbContext;
using SubscriberDbContext = CqrsSpike.Subscriber.Infrastructure.Persistence.ApplicationDbContext;

namespace CqrsSpike.Scenarios;

/// <summary>
/// Claims EF Core InMemory cannot prove, because it ignores transactions and unique indexes. Each
/// app's real composition runs in-process against the fixture's SQL Server databases.
/// </summary>
[Collection(SpikeCollection.Name)]
public class RelationalChecks
{
    private readonly SpikeFixture _fixture;

    public RelationalChecks(SpikeFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DomainEventHandlerFailure_RollsBackANestedCommandsSave()
    {
        // Registered after the sample's OrderPlacedDomainEventHandler, whose nested
        // CreateAuditEntryCommand saves the Order and the AuditEntry before this handler fails.
        var probe = new RollbackProbe();
        await using var provider = InProcessApps.Publisher(_fixture.PublisherDatabase, afterApp: services => services
            .AddSingleton(probe)
            .AddTransient<INotificationHandler<OrderPlacedDomainEventNotification>, FailAfterNestedSaveHandler>());
        var customer = $"Rollback-{Guid.NewGuid():N}";

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await using var scope = provider.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<ISender>().Send(new CreateOrderCommand(customer, 1m));
        });

        Assert.True(probe.NestedSaveWasVisible, "The nested command's save did not reach the database.");
        await using var check = provider.CreateAsyncScope();
        var db = check.ServiceProvider.GetRequiredService<PublisherDbContext>();
        Assert.False(await db.Orders.AnyAsync(o => o.CustomerName == customer));
        Assert.False(await db.AuditEntries.AnyAsync(a => a.Description == $"Order placed for {customer}"));
    }

    [Fact]
    public async Task ConcurrentRedelivery_LeavesOneOrderRecord()
    {
        await using var provider = InProcessApps.Subscriber(_fixture.SubscriberDatabase);
        var orderId = Guid.NewGuid();

        // Eight concurrent deliveries of the same OrderCreatedEvent race the handler's
        // check-then-insert; only the unique OrderId index stops a second row.
        var outcomes = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ => Task.Run(async () =>
        {
            try
            {
                await using var scope = provider.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<ISender>().Send(new RecordOrderCommand(orderId, "Ann", 1m));
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        })));

        Assert.All(outcomes.OfType<Exception>(), ex =>
        {
            var update = Assert.IsType<DbUpdateException>(ex);
            var sql = Assert.IsType<SqlException>(update.InnerException);
            Assert.Contains(sql.Number, new[] { 2601, 2627 });
        });
        await using var check = provider.CreateAsyncScope();
        var db = check.ServiceProvider.GetRequiredService<SubscriberDbContext>();
        Assert.Equal(1, await db.OrderRecords.CountAsync(r => r.OrderId == orderId));
    }

    private sealed class RollbackProbe
    {
        public bool NestedSaveWasVisible { get; set; }
    }

    private sealed class FailAfterNestedSaveHandler(PublisherDbContext db, RollbackProbe probe)
        : INotificationHandler<OrderPlacedDomainEventNotification>
    {
        public async ValueTask Handle(OrderPlacedDomainEventNotification notification, CancellationToken cancellationToken)
        {
            var description = $"Order placed for {notification.DomainEvent.Order.CustomerName}";
            probe.NestedSaveWasVisible = await db.AuditEntries.AnyAsync(a => a.Description == description, cancellationToken);
            throw new InvalidOperationException("Domain-event handler failed after a nested save.");
        }
    }
}
