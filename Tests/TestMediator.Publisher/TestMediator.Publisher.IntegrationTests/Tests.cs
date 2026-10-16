using System.Net;
using System.Net.Http.Json;
using Mediator;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using TestMediator.Publisher.Application.Common.Behaviours;
using TestMediator.Publisher.Application.Common.Eventing;
using TestMediator.Publisher.Application.Orders;
using TestMediator.Publisher.Application.Orders.CreateOrder;
using TestMediator.Publisher.Application.Orders.GetOrders;

namespace TestMediator.Publisher.IntegrationTests;

/// <summary>
/// Pins the pipeline the Mediator source generator builds from the literal options.PipelineBehaviors array.
/// A dropped entry or a non-literal array still builds clean and silently removes behaviours; only the
/// generated registrations show it. No containers needed.
/// </summary>
public class PipelineShapeChecks
{
    private static ServiceCollection Services()
    {
        var services = new ServiceCollection();
        TestMediator.Publisher.Infrastructure.DependencyInjection.AddInfrastructure(services, new ConfigurationBuilder().Build());
        return services;
    }

    [Fact]
    public void CommandsGetAllSevenBehaviours_AndQueriesSkipUnitOfWork()
    {
        var services = Services();

        Assert.Equal(
            new[]
            {
                typeof(LoggingBehaviour<,>),
                typeof(UnhandledExceptionBehaviour<,>),
                typeof(PerformanceBehaviour<,>),
                typeof(AuthorizationBehaviour<,>),
                typeof(MessageBusPublishBehaviour<,>),
                typeof(ValidationBehaviour<,>),
                typeof(UnitOfWorkBehaviour<,>)
            },
            PipelineOf<CreateOrderCommand, Guid>(services));

        Assert.Equal(
            new[]
            {
                typeof(LoggingBehaviour<,>),
                typeof(UnhandledExceptionBehaviour<,>),
                typeof(PerformanceBehaviour<,>),
                typeof(AuthorizationBehaviour<,>),
                typeof(MessageBusPublishBehaviour<,>),
                typeof(ValidationBehaviour<,>)
            },
            PipelineOf<GetOrdersQuery, List<OrderDto>>(services));
    }

    [Fact]
    public void NotificationsUseTheStopOnFirstFailurePublisher()
    {
        using var provider = Services().BuildServiceProvider();
        using var scope = provider.CreateScope();
        Assert.IsType<TestMediator.Publisher.Infrastructure.Configuration.SequentialNotificationPublisher>(
            scope.ServiceProvider.GetRequiredService<INotificationPublisher>());
    }

    [Fact]
    public void HandlersAndBehavioursAreTransient()
    {
        var registrations = Services()
            .Where(d => d.ServiceType == typeof(ICommandHandler<CreateOrderCommand, Guid>)
                || d.ServiceType == typeof(IPipelineBehavior<CreateOrderCommand, Guid>))
            .ToList();

        Assert.Equal(8, registrations.Count);
        Assert.All(registrations, d => Assert.Equal(ServiceLifetime.Transient, d.Lifetime));
    }

    private static Type[] PipelineOf<TMessage, TResponse>(IServiceCollection services)
        where TMessage : notnull, IMessage =>
        services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<TMessage, TResponse>))
            .Select(d => d.ImplementationType!.GetGenericTypeDefinition())
            .ToArray();
}

[Collection(FixtureCollection.Name)]
public class PublisherScenarios
{
    private readonly Fixture _fixture;
    private readonly ITestOutputHelper _output;

    public PublisherScenarios(Fixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    private HttpClient Client(bool authenticated = false)
    {
        var client = _fixture.Publisher.CreateClient();
        if (authenticated)
        {
            client.DefaultRequestHeaders.Add("X-Test-User", "tester");
        }

        return client;
    }

    private static string TraceOf(HttpResponseMessage response) =>
        response.Headers.GetValues(TraceHeaderStartupFilter.HeaderName).Single();

    [Fact]
    public async Task Crud_CommandsAndQueries_RoundTripThroughDispatcher()
    {
        var client = Client(authenticated: true);

        var created = await client.PostAsJsonAsync("api/orders", new { customerName = "Ada", total = 12.5m });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var id = await created.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, id);

        var fetched = await client.GetFromJsonAsync<OrderDto>($"api/orders/{id}");
        Assert.NotNull(fetched);
        Assert.Equal("Ada", fetched!.CustomerName);
        Assert.Equal(12.5m, fetched.Total);

        var shipped = await client.PutAsync($"api/orders/{id}/ship", content: null);
        Assert.Equal(HttpStatusCode.NoContent, shipped.StatusCode);
        Assert.Equal(TestMediator.Publisher.Domain.OrderStatus.Shipped, (await client.GetFromJsonAsync<OrderDto>($"api/orders/{id}"))!.Status);

        var all = await client.GetFromJsonAsync<List<OrderDto>>("api/orders");
        Assert.Contains(all!, o => o.Id == id);

        Assert.Equal(HttpStatusCode.OK, (await client.DeleteAsync($"api/orders/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"api/orders/{id}")).StatusCode);
    }

    [Fact]
    public async Task SecuredQuery_RejectsAnonymousCaller()
    {
        var response = await Client().GetAsync($"api/orders/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // Spike S2: ValidationBehaviour runs before the handler, so nothing is buffered, committed or flushed.
    [Fact]
    public async Task InvalidCommand_Returns400_AndNothingIsBufferedCommittedOrFlushed()
    {
        var response = await Client(authenticated: true).PostAsJsonAsync("api/orders", new { customerName = (string?)null, total = 1m });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var entries = _fixture.Recorder.ForTrace(TraceOf(response)).Select(e => e.What).ToList();
        var trace = string.Join(" -> ", entries);
        _output.WriteLine(trace);

        Assert.DoesNotContain(entries, e => e.StartsWith("publish:") || e.StartsWith("send:"));
        Assert.DoesNotContain("commit", entries);
        Assert.DoesNotContain(entries, e => e.StartsWith("flush:") && e != "flush:0");
    }

    // Spike S3b: the AuthorizationBehaviour guards ISender dispatch too, not only the HTTP endpoint.
    [Fact]
    public async Task SecuredCommandSentStraightThroughISender_IsRejectedForAnAnonymousPrincipal()
    {
        using var scope = _fixture.Publisher.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        // An unauthenticated principal is not a user: UnauthorizedAccessException (ForbiddenAccessException is the wrong-role case).
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await sender.Send(new TestMediator.Publisher.Application.Orders.DeleteOrder.DeleteOrderCommand(Guid.NewGuid())));
    }

    // Spike S8: a query passes the bus flush item but never reaches the unit of work, so nothing is committed.
    [Fact]
    public async Task Query_NeverCommits_AndFlushesNothing()
    {
        var response = await Client(authenticated: true).GetAsync("api/orders");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var entries = _fixture.Recorder.ForTrace(TraceOf(response)).Select(e => e.What).ToList();
        var trace = string.Join(" -> ", entries);
        _output.WriteLine(trace);

        Assert.DoesNotContain("commit", entries);
        Assert.DoesNotContain(entries, e => e.StartsWith("publish:") || e.StartsWith("send:"));
        Assert.DoesNotContain(entries, e => e.StartsWith("flush:") && e != "flush:0");
    }

    private async Task<int> CountAsync(Func<TestMediator.Publisher.Infrastructure.Persistence.ApplicationDbContext, Task<int>> count)
    {
        using var scope = _fixture.Publisher.Services.CreateScope();
        return await count(scope.ServiceProvider.GetRequiredService<TestMediator.Publisher.Infrastructure.Persistence.ApplicationDbContext>());
    }

    // Spike S4: the Order constructor raises a domain event; its handler sends a command that persists an audit entry.
    [Fact]
    public async Task DomainEvent_RaisedByTheEntityConstructor_RunsItsHandler_AndTheNestedCommandPersistsAnAuditEntry()
    {
        var before = await CountAsync(db => db.AuditEntries.CountAsync());

        var response = await Client().PostAsJsonAsync("api/orders", new { customerName = "Domain", total = 1m });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(before + 1, await CountAsync(db => db.AuditEntries.CountAsync()));
    }

    // Spike S7 with a domain event: the nested command commits inside the outer save, but only the outermost dispatch flushes.
    [Fact]
    public async Task DomainEventNestedCommit_DoesNotFlush_OnlyTheOutermostDispatchFlushesAfterTheFinalCommit()
    {
        var response = await Client().PostAsJsonAsync("api/orders", new { customerName = "Nested", total = 1m });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var entries = _fixture.Recorder.ForTrace(TraceOf(response)).Select(e => e.What).ToList();
        var trace = string.Join(" -> ", entries);
        _output.WriteLine(trace);

        Assert.Equal(2, entries.Count(e => e == "commit"));
        Assert.True(entries.Count(e => e.StartsWith("flush:")) == 1, $"Only the outermost dispatch may flush: {trace}");
        Assert.Equal("flush:1", entries[^1]);
        Assert.True(entries.IndexOf("commit") < entries.IndexOf("publish:OrderCreatedEvent"), $"The nested command commits before the order message is buffered: {trace}");
        Assert.True(entries.LastIndexOf("commit") < entries.IndexOf("flush:1"), $"The flush follows the final commit: {trace}");
    }

    // A domain event handler that fails after its nested command saved: both the order and the audit entry roll back, nothing is flushed.
    [Fact]
    public async Task DomainEventHandlerThatFailsAfterTheNestedCommand_RollsBackBothRows_AndFlushesNothing()
    {
        var orders = await CountAsync(db => db.Orders.CountAsync());
        var audits = await CountAsync(db => db.AuditEntries.CountAsync());
        TestMediator.Publisher.Application.EventHandlers.DomainEventHandlers.OrderPlacedDomainEventHandler.FailAfterAudit = true;
        try
        {
            var response = await Client().PostAsJsonAsync("api/orders", new { customerName = "Rollback", total = 1m });
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var entries = _fixture.Recorder.ForTrace(TraceOf(response)).Select(e => e.What).ToList();
            Assert.DoesNotContain(entries, e => e.StartsWith("flush:") && e != "flush:0");
            Assert.Equal(orders, await CountAsync(db => db.Orders.CountAsync()));
            Assert.Equal(audits, await CountAsync(db => db.AuditEntries.CountAsync()));
        }
        finally
        {
            TestMediator.Publisher.Application.EventHandlers.DomainEventHandlers.OrderPlacedDomainEventHandler.FailAfterAudit = false;
        }
    }

    [Fact]
    public async Task BusFlushHappensAfterUnitOfWorkCommit()
    {
        var response = await Client().PostAsJsonAsync("api/orders", new { customerName = "Erin", total = 1m });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var entries = _fixture.Recorder.ForTrace(TraceOf(response)).Select(e => e.What).ToList();
        var trace = string.Join(" -> ", entries);
        _output.WriteLine(trace);

        Assert.Contains("publish:OrderCreatedEvent", entries);
        Assert.True(entries.Count(e => e.StartsWith("flush:")) == 1, $"Expected a single flush, by the outermost dispatch: {trace}");
        Assert.Equal("flush:1", entries[^1]);
        Assert.True(entries.LastIndexOf("commit") >= 0, $"UnitOfWork commit not observed: {trace}");
        Assert.True(entries.LastIndexOf("commit") < entries.Count - 1, $"Bus flush must follow the final commit: {trace}");
        Assert.True(entries.IndexOf("publish:OrderCreatedEvent") < entries.IndexOf("flush:1"), $"Publish is buffered before the flush: {trace}");
    }

    [Fact]
    public async Task DispatchDoesNotFlushWhileAnOuterOwnerHoldsTheScope()
    {
        // IntegrationEventConsumer claims MessageBusFlushOwner before its handler dispatches, then flushes
        // after its own transaction completes. Reproduce that from outside the consumer.
        var trace = $"ownership-{Guid.NewGuid():N}";
        using var scope = _fixture.Publisher.Services.CreateScope();
        var accessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        accessor.HttpContext = new DefaultHttpContext { TraceIdentifier = trace };
        try
        {
            var owner = scope.ServiceProvider.GetRequiredService<MessageBusFlushOwner>();
            Assert.True(owner.Enter());
            try
            {
                await scope.ServiceProvider.GetRequiredService<ISender>().Send(new CreateOrderCommand("Ivy", 3m));
            }
            finally
            {
                owner.Exit();
            }
        }
        finally
        {
            accessor.HttpContext = null;
        }

        var entries = _fixture.Recorder.ForTrace(trace).Select(e => e.What).ToList();
        _output.WriteLine(string.Join(" -> ", entries));
        Assert.Contains("publish:OrderCreatedEvent", entries);
        Assert.DoesNotContain(entries, e => e.StartsWith("flush:"));
    }

    private sealed record ShippedObservation(int Count, bool RowWasShippedOnArrival, string? RowStatusAfterwards);

    // POST creates an order and publishes OrderCreatedEvent. The Publisher's own consumer handles it and its handler
    // Sends MarkOrderShippedCommand, which publishes OrderShippedEvent from inside the consumer's dispatch.
    private async Task<ShippedObservation> CreateOrderAndObserveShippedAsync(TimeSpan wait)
    {
        var bus = _fixture.Publisher.Services.GetRequiredService<MassTransit.IBus>();
        var received = new System.Collections.Concurrent.ConcurrentDictionary<Guid, bool>();
        var arrival = new TaskCompletionSource<Guid>(TaskCreationOptions.RunContinuationsAsynchronously);
        var count = 0;
        var shippedOnArrival = false;
        Guid expected = Guid.Empty;

        var observer = bus.ConnectReceiveEndpoint($"test-shipped-{Guid.NewGuid():N}", endpoint =>
            endpoint.Handler<TestMediator.Publisher.Eventing.Messages.OrderShippedEvent>(async context =>
            {
                using var scope = _fixture.Publisher.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TestMediator.Publisher.Infrastructure.Persistence.ApplicationDbContext>();
                var order = await db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == context.Message.OrderId);
                if (context.Message.OrderId == expected)
                {
                    shippedOnArrival = order?.Status == TestMediator.Publisher.Domain.OrderStatus.Shipped;
                    Interlocked.Increment(ref count);
                    arrival.TrySetResult(expected);
                }
            }));
        await observer.Ready;

        string? statusAfter;
        try
        {
            var response = await Client().PostAsJsonAsync("api/orders", new { customerName = "Judy", total = 2m });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            expected = await response.Content.ReadFromJsonAsync<Guid>();
            await Task.WhenAny(arrival.Task, Task.Delay(wait));
            await Task.Delay(TimeSpan.FromSeconds(1));

            using var scope = _fixture.Publisher.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TestMediator.Publisher.Infrastructure.Persistence.ApplicationDbContext>();
            statusAfter = (await db.Orders.AsNoTracking().FirstAsync(o => o.Id == expected)).Status.ToString();
        }
        finally
        {
            await observer.StopAsync();
        }

        return new ShippedObservation(count, shippedOnArrival, statusAfter);
    }

    [Fact]
    public async Task ConsumerDispatch_FlushesOnceAfterItsOwnCommit()
    {
        var result = await CreateOrderAndObserveShippedAsync(TimeSpan.FromSeconds(20));

        Assert.Equal(1, result.Count);
        Assert.True(result.RowWasShippedOnArrival, "OrderShippedEvent reached the broker before the consumer's work committed.");
        Assert.Equal("Shipped", result.RowStatusAfterwards);
    }

    [Fact]
    public async Task ConsumerWhoseOwnSaveChangesFails_PublishesNothing_EvenThoughTheNestedCommandCommitted()
    {
        // The nested MarkOrderShippedCommand commits its own unit of work (save 1). If it flushed, OrderShippedEvent
        // would escape. Only the consumer, which claimed MessageBusFlushOwner, may flush - after its own save (save 2).
        RecordingUnitOfWork.FailSecondSave = true;
        try
        {
            var result = await CreateOrderAndObserveShippedAsync(TimeSpan.FromSeconds(8));

            Assert.Equal(0, result.Count);
            Assert.Equal("Placed", result.RowStatusAfterwards);
        }
        finally
        {
            RecordingUnitOfWork.FailSecondSave = false;
        }
    }
}
