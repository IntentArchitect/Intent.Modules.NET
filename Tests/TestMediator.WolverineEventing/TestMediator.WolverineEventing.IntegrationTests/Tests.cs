using System.Collections.Concurrent;
using System.Transactions;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
using Wolverine;
using Xunit;
using Xunit.Abstractions;
using TestMediator.WolverineEventing.Infrastructure.Dispatch.Behaviours;
using TestMediator.WolverineEventing.Application.Common.Eventing;
using TestMediator.WolverineEventing.Application.Orders.AcceptOrder;
using TestMediator.WolverineEventing.Domain.Common.Interfaces;
using TestMediator.WolverineEventing.Eventing.Messages;
using TestMediator.WolverineEventing.Infrastructure.Eventing;
using TestMediator.WolverineEventing.Infrastructure.Persistence;
using IMessageBus = TestMediator.WolverineEventing.Application.Common.Eventing.IMessageBus;
using WolverineBus = Wolverine.IMessageBus;

namespace TestMediator.WolverineEventing.IntegrationTests;

public sealed record PipelineEntry(long Sequence, string What);

/// <summary>Records bus buffering, unit-of-work activity and bus flushes in the order they happen.</summary>
public sealed class PipelineRecorder
{
    private long _sequence;
    private readonly ConcurrentQueue<PipelineEntry> _entries = new();

    public void Record(string what) => _entries.Enqueue(new PipelineEntry(Interlocked.Increment(ref _sequence), what));

    public long Mark() => Interlocked.Read(ref _sequence);

    public IReadOnlyList<string> Since(long mark) =>
        _entries.Where(e => e.Sequence > mark).OrderBy(e => e.Sequence).Select(e => e.What).ToList();
}

internal sealed class RecordingMessageBus : IMessageBus
{
    private readonly IMessageBus _inner;
    private readonly PipelineRecorder _recorder;

    public RecordingMessageBus(IMessageBus inner, PipelineRecorder recorder)
    {
        _inner = inner;
        _recorder = recorder;
    }

    public void Publish<TMessage>(TMessage message) where TMessage : class
    {
        _recorder.Record($"publish:{typeof(TMessage).Name}");
        _inner.Publish(message);
    }

    public void Send<TMessage>(TMessage message) where TMessage : class
    {
        _recorder.Record($"send:{typeof(TMessage).Name}");
        _inner.Send(message);
    }

    public Task FlushAllAsync(CancellationToken cancellationToken = default)
    {
        _recorder.Record("flush");
        return _inner.FlushAllAsync(cancellationToken);
    }
}

internal sealed class RecordingUnitOfWork : IUnitOfWork
{
    private readonly IUnitOfWork _inner;
    private readonly PipelineRecorder _recorder;

    public RecordingUnitOfWork(IUnitOfWork inner, PipelineRecorder recorder)
    {
        _inner = inner;
        _recorder = recorder;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _recorder.Record($"save:ambientScope={Transaction.Current is not null}");
        return await _inner.SaveChangesAsync(cancellationToken);
    }

    public bool HasDbTransaction()
    {
        var has = _inner.HasDbTransaction();
        _recorder.Record($"hasDbTransaction={has}");
        return has;
    }
}

/// <summary>Handles the event the app publishes, so a test can observe what reached the (local) transport.</summary>
public sealed class AcceptedObserverHandler
{
    public static Func<OrderAcceptedEvent, Task>? Callback;

    public Task Handle(OrderAcceptedEvent message) => Callback is null ? Task.CompletedTask : Callback(message);
}

public sealed class WolverineFactory : WebApplicationFactory<TestMediator.WolverineEventing.Api.Program>
{
    private readonly PipelineRecorder _recorder;

    public WolverineFactory(PipelineRecorder recorder) => _recorder = recorder;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(_recorder);
            services.ConfigureWolverine(opts => opts.Discovery.IncludeType<AcceptedObserverHandler>());

            // Decorate (never replace) the real collaborators so the pipeline can be observed.
            services.AddScoped<IMessageBus>(sp => new RecordingMessageBus(
                ActivatorUtilities.CreateInstance<WolverineMessageBus>(sp), _recorder));
            services.AddScoped<IUnitOfWork>(sp => new RecordingUnitOfWork(
                sp.GetRequiredService<ApplicationDbContext>(), _recorder));
        });
    }
}

/// <summary>
/// The app is generated in one configuration at a time. Each test states which one it belongs to with a trait and asserts
/// the generated app really is in that configuration. Run one with: dotnet test --filter "Config=Durable".
/// </summary>
public static class Mode
{
    // Wolverine's integration-event middleware is generated only when the outbox is not Durable.
    public static bool HasIntegrationMiddleware =>
        typeof(WolverineMessageBus).Assembly.GetType("TestMediator.WolverineEventing.Infrastructure.Eventing.WolverineIntegrationEventMiddleware") is not null;

    public static bool HasPersistence =>
        typeof(WolverineMessageBus).Assembly.GetType("TestMediator.WolverineEventing.Infrastructure.Persistence.ApplicationDbContext") is not null;
}

/// <summary>Starts a SQL Server container and boots the app in-process on Wolverine's local transport.</summary>
public sealed class Fixture : IAsyncLifetime
{
    private MsSqlContainer? _sqlServer;

    public PipelineRecorder Recorder { get; } = new();
    public WolverineFactory App { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
        await _sqlServer.StartAsync();
        var database = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "TestMediatorWolverine"
        }.ConnectionString;

        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(database).Options;
        var constructor = typeof(ApplicationDbContext).GetConstructors().Single();
        var arguments = constructor.GetParameters()
            .Select(p => p.ParameterType == typeof(DbContextOptions<ApplicationDbContext>) ? (object?)options : null)
            .ToArray();
        await using (var db = (ApplicationDbContext)constructor.Invoke(arguments))
        {
            await db.Database.EnsureCreatedAsync();
        }

        // The generated Wolverine configuration reads these while the host is being built, before the factory's own
        // configuration is applied, so they are supplied through the environment.
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", database);
        Environment.SetEnvironmentVariable("Wolverine__ErrorHandling__RetryWithCooldown__Delays", "00:00:00.200");

        App = new WolverineFactory(Recorder);
        _ = App.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await App.DisposeAsync();
        if (_sqlServer is not null)
        {
            await _sqlServer.DisposeAsync();
        }
    }
}

[CollectionDefinition(Name)]
public sealed class FixtureCollection : ICollectionFixture<Fixture>
{
    public const string Name = "wolverine";
}

public class PipelineShapeChecks
{
    private static Type[] PipelineOf()
    {
        var services = new ServiceCollection();
        TestMediator.WolverineEventing.Infrastructure.DependencyInjection.AddInfrastructure(services,
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=.;Database=x"
            }).Build());

        return services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<AcceptOrderCommand, Unit>))
            .Select(d => d.ImplementationType!.GetGenericTypeDefinition())
            .ToArray();
    }

    [Fact]
    [Trait("Config", "NoneOutbox")]
    public void NoneOutbox_PipelineKeepsTheFlushItem_AndTheIntegrationMiddlewareExists()
    {
        Assert.True(Mode.HasIntegrationMiddleware, "This test belongs to the Outbox None generation.");
        Assert.Equal(
            [.. AlwaysOn, typeof(MessageBusPublishBehaviour<,>), typeof(ValidationBehaviour<,>), typeof(UnitOfWorkBehaviour<,>)],
            PipelineOf());
    }

    [Fact]
    [Trait("Config", "Durable")]
    public void Durable_PipelineHasNoFlushItem_AndThereIsNoIntegrationMiddleware()
    {
        Assert.False(Mode.HasIntegrationMiddleware, "This test belongs to the Durable generation.");
        Assert.Equal([.. AlwaysOn, typeof(ValidationBehaviour<,>), typeof(UnitOfWorkBehaviour<,>)], PipelineOf());
    }

    private static readonly Type[] AlwaysOn =
        [
        typeof(LoggingBehaviour<,>), typeof(UnhandledExceptionBehaviour<,>), typeof(PerformanceBehaviour<,>),
        typeof(AuthorizationBehaviour<,>)
        ];
}

[Collection(FixtureCollection.Name)]
public class WolverineScenarios
{
    private readonly Fixture _fixture;
    private readonly ITestOutputHelper _output;

    public WolverineScenarios(Fixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    private sealed record Observation(int AcceptedCount, bool RowExistedOnArrival, int RowCount, IReadOnlyList<string> Entries);

    private async Task<Observation> PublishOrderPlacedAndObserveAsync(Guid orderId, TimeSpan wait, string customerName = "Peggy")
    {
        var accepted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var count = 0;
        var rowExistedOnArrival = false;
        AcceptedObserverHandler.Callback = async message =>
        {
            if (message.OrderId != orderId)
            {
                return;
            }

            using var scope = _fixture.App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            rowExistedOnArrival = await db.Orders.AnyAsync(o => o.Id == orderId);
            Interlocked.Increment(ref count);
            accepted.TrySetResult(true);
        };

        var mark = _fixture.Recorder.Mark();
        try
        {
            using var scope = _fixture.App.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<WolverineBus>();
            await bus.PublishAsync(new OrderPlacedEvent { OrderId = orderId, CustomerName = customerName });
            await Task.WhenAny(accepted.Task, Task.Delay(wait));
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        finally
        {
            AcceptedObserverHandler.Callback = null;
        }

        var entries = _fixture.Recorder.Since(mark);
        _output.WriteLine(string.Join(" -> ", entries));
        using var verify = _fixture.App.Services.CreateScope();
        var rows = await verify.ServiceProvider.GetRequiredService<ApplicationDbContext>().Orders.CountAsync(o => o.Id == orderId);
        return new Observation(count, rowExistedOnArrival, rows, entries);
    }

    [Fact]
    [Trait("Config", "NoneOutbox")]
    public async Task NoneOutbox_NestedCommandDoesNotFlush_TheMiddlewareFlushesOnceAfterItsCommit()
    {
        Assert.True(Mode.HasIntegrationMiddleware, "This test belongs to the Outbox None generation.");
        var orderId = Guid.NewGuid();

        var result = await PublishOrderPlacedAndObserveAsync(orderId, TimeSpan.FromSeconds(30));

        // The middleware claims MessageBusFlushOwner, so the nested command's MessageBusPublishBehaviour must not flush.
        // The only flush is the middleware's, after its own save, and it is the last thing that happens.
        Assert.Contains("publish:OrderAcceptedEvent", result.Entries);
        Assert.Equal(1, result.Entries.Count(e => e == "flush"));
        Assert.Equal("flush", result.Entries[^1]);
        Assert.True(result.Entries.Count(e => e.StartsWith("save:")) >= 2,
            $"Expected the nested command's save and the middleware's own save before the flush: {string.Join(" -> ", result.Entries)}");

        Assert.Equal(1, result.AcceptedCount);
        Assert.True(result.RowExistedOnArrival, "OrderAcceptedEvent reached the transport before the order was committed.");
        Assert.Equal(1, result.RowCount);
    }

    [Fact]
    [Trait("Config", "NoneOutbox")]
    public async Task NoneOutbox_HandlerThatFailsAfterTheNestedCommand_NeverFlushesAndRollsBack()
    {
        Assert.True(Mode.HasIntegrationMiddleware, "This test belongs to the Outbox None generation.");
        var orderId = Guid.NewGuid();

        var result = await PublishOrderPlacedAndObserveAsync(orderId, TimeSpan.FromSeconds(8), customerName: "FailAfterDispatch");

        Assert.Contains("publish:OrderAcceptedEvent", result.Entries);
        Assert.DoesNotContain("flush", result.Entries);
        Assert.Equal(0, result.AcceptedCount);
        Assert.Equal(0, result.RowCount);
    }

    [Fact]
    [Trait("Config", "Durable")]
    public async Task Durable_HandlerThatInvokesACommand_DeliversExactlyOnceAfterCommit()
    {
        Assert.False(Mode.HasIntegrationMiddleware, "This test belongs to the Durable generation.");
        var orderId = Guid.NewGuid();

        var result = await PublishOrderPlacedAndObserveAsync(orderId, TimeSpan.FromSeconds(30));

        Assert.Contains("publish:OrderAcceptedEvent", result.Entries);
        Assert.Equal(1, result.AcceptedCount);
        Assert.True(result.RowExistedOnArrival, "OrderAcceptedEvent reached the transport before the order was committed.");
        Assert.Equal(1, result.RowCount);
    }

    [Fact]
    [Trait("Config", "Durable")]
    public async Task Durable_HandlerWhoseSaveChangesFails_DeliversNothingAndPersistsNothing()
    {
        Assert.False(Mode.HasIntegrationMiddleware, "This test belongs to the Durable generation.");
        var orderId = Guid.NewGuid();
        var first = await PublishOrderPlacedAndObserveAsync(orderId, TimeSpan.FromSeconds(30));
        Assert.Equal(1, first.AcceptedCount);

        // The same order id inserts a duplicate primary key: SaveChanges fails after the handler queued its publish.
        var second = await PublishOrderPlacedAndObserveAsync(orderId, TimeSpan.FromSeconds(8));

        Assert.Contains("publish:OrderAcceptedEvent", second.Entries);
        Assert.Equal(0, second.AcceptedCount);
        Assert.Equal(1, second.RowCount);
    }
}
