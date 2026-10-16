using Mediator;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using TestMediator.Outbox.Application.Common.Behaviours;
using TestMediator.Outbox.Application.Orders.AcceptOrder;
using TestMediator.Outbox.Eventing.Messages;
using TestMediator.Outbox.Infrastructure.Persistence;

namespace TestMediator.Outbox.IntegrationTests;

/// <summary>With the Entity Framework outbox the bus flush is not a pipeline item: SaveChanges releases the outbox.</summary>
public class PipelineShapeChecks
{
    [Fact]
    public void EfOutbox_PipelineHasNoFlushItem()
    {
        var services = new ServiceCollection();
        TestMediator.Outbox.Infrastructure.DependencyInjection.AddInfrastructure(services, new ConfigurationBuilder().Build());

        var pipeline = services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<AcceptOrderCommand, Unit>))
            .Select(d => d.ImplementationType!.GetGenericTypeDefinition())
            .ToArray();

        Assert.Equal(new[] { typeof(ValidationBehaviour<,>), typeof(UnitOfWorkBehaviour<,>) }, pipeline);
        Assert.DoesNotContain(typeof(MessageBusPublishBehaviour<,>), pipeline);
    }
}

[Collection(FixtureCollection.Name)]
public class OutboxScenarios
{
    private readonly Fixture _fixture;
    private readonly ITestOutputHelper _output;

    public OutboxScenarios(Fixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    private async Task<Observation> PublishOrderPlacedAndObserveAsync(Guid orderId, string? customerName, TimeSpan wait)
    {
        var bus = _fixture.App.Services.GetRequiredService<IBus>();
        var accepted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var acceptedCount = 0;
        var rowExistedOnArrival = false;

        var observer = bus.ConnectReceiveEndpoint($"test-accepted-{Guid.NewGuid():N}", endpoint =>
            endpoint.Handler<OrderAcceptedEvent>(async context =>
            {
                if (context.Message.OrderId != orderId)
                {
                    return;
                }

                using var scope = _fixture.App.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                rowExistedOnArrival = await db.Orders.AnyAsync(o => o.Id == orderId);
                Interlocked.Increment(ref acceptedCount);
                accepted.TrySetResult(true);
            }));
        await observer.Ready;

        var mark = _fixture.Recorder.Mark();
        try
        {
            await bus.Publish(new OrderPlacedEvent { OrderId = orderId, CustomerName = customerName! });
            await Task.WhenAny(accepted.Task, Task.Delay(wait));
            // Give a duplicate delivery time to show up.
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        finally
        {
            await observer.StopAsync();
        }

        var entries = _fixture.Recorder.Since(mark);
        _output.WriteLine(string.Join(" -> ", entries));

        using var verifyScope = _fixture.App.Services.CreateScope();
        var rowCount = await verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>()
            .Orders.CountAsync(o => o.Id == orderId);
        return new Observation(acceptedCount, rowExistedOnArrival, rowCount, entries);
    }

    private sealed record Observation(int AcceptedCount, bool RowExistedOnArrival, int RowCount, IReadOnlyList<string> Entries);

    [Fact]
    public async Task ConsumerThatInvokesACommand_TakesEfExistingTransactionBranch_AndDeliversExactlyOnceAfterCommit()
    {
        var orderId = Guid.NewGuid();

        var result = await PublishOrderPlacedAndObserveAsync(orderId, "Grace", TimeSpan.FromSeconds(20));

        // The outbox transaction is already open when the nested command runs, so the unit-of-work
        // behaviour must skip its own TransactionScope.
        Assert.Contains("hasDbTransaction=True", result.Entries);
        Assert.DoesNotContain("hasDbTransaction=False", result.Entries);
        Assert.DoesNotContain("save:ambientScope=True", result.Entries);

        // The pipeline has no flush item (see PipelineShapeChecks). The message is released by the flush that
        // DbContext.SaveChanges issues into the outbox, which can only happen after the handler queued it.
        var publishAt = result.Entries.ToList().IndexOf("publish:OrderAcceptedEvent");
        Assert.True(publishAt >= 0, string.Join(" -> ", result.Entries));
        var firstSaveAfterPublish = result.Entries.Select((e, i) => (e, i)).First(x => x.i > publishAt && x.e.StartsWith("save:")).i;
        Assert.Equal("flush", result.Entries[firstSaveAfterPublish + 1]);

        // Delivered exactly once, and only after the order row had committed.
        Assert.Equal(1, result.AcceptedCount);
        Assert.True(result.RowExistedOnArrival, "OrderAcceptedEvent reached the broker before the order was committed.");
        Assert.Equal(1, result.RowCount);
    }

    [Fact]
    public async Task ConsumerThatFailsAtSaveChanges_PublishesNothingAndPersistsNothing()
    {
        var orderId = Guid.NewGuid();
        var first = await PublishOrderPlacedAndObserveAsync(orderId, "Heidi", TimeSpan.FromSeconds(20));
        Assert.Equal(1, first.AcceptedCount);

        // Redelivering the same order id inserts a duplicate primary key: SaveChanges fails inside the outbox transaction
        // after the handler has already queued its publish.
        var second = await PublishOrderPlacedAndObserveAsync(orderId, "Heidi", TimeSpan.FromSeconds(6));

        Assert.Contains("publish:OrderAcceptedEvent", second.Entries);
        Assert.Equal(0, second.AcceptedCount);
        Assert.Equal(1, second.RowCount);
    }
}
