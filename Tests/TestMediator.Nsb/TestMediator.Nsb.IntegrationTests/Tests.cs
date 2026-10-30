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
using NServiceBus;
using Testcontainers.MsSql;
using Xunit;
using Xunit.Abstractions;
using TestMediator.Nsb.Infrastructure.Dispatch.Behaviours;
using TestMediator.Nsb.Application.Common.Eventing;
using TestMediator.Nsb.Application.Orders.AcceptOrder;
using TestMediator.Nsb.Domain.Common.Interfaces;
using TestMediator.Nsb.Eventing.Messages;
using TestMediator.Nsb.Infrastructure.Eventing;
using TestMediator.Nsb.Infrastructure.Persistence;

namespace TestMediator.Nsb.IntegrationTests;

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

/// <summary>A second endpoint on the same learning transport that observes the events the app publishes.</summary>
public sealed class AcceptedObserver : IHandleMessages<OrderAcceptedEvent>
{
    // Both endpoints in this process scan this assembly, so one publish can invoke the handler once per endpoint. The
    // callback therefore receives the NServiceBus message id, which is the same for every copy of one publish.
    public static Func<OrderAcceptedEvent, string, Task>? Callback;

    public Task Handle(OrderAcceptedEvent message, IMessageHandlerContext context) =>
        Callback is null ? Task.CompletedTask : Callback(message, context.MessageId);
}

public sealed class NServiceBusFactory : WebApplicationFactory<TestMediator.Nsb.Api.Program>
{
    private readonly PipelineRecorder _recorder;
    private readonly string _database;
    private readonly string _storage;

    public NServiceBusFactory(PipelineRecorder recorder, string database, string storage)
    {
        _recorder = recorder;
        _database = database;
        _storage = storage;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _database,
                ["NServiceBus:LearningTransport:StorageDirectory"] = _storage,
                ["NServiceBus:EnableInstallers"] = "true",
                ["NServiceBus:Recoverability:ImmediateRetries"] = "1",
                ["NServiceBus:Recoverability:DelayedRetries"] = "0",
            });
        });
        builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(_recorder);
            // Decorate (never replace) the real collaborators so the pipeline can be observed.
            services.AddScoped<IMessageBus>(sp => new RecordingMessageBus(sp.GetRequiredService<NServiceBusMessageBus>(), _recorder));
            services.AddScoped<IUnitOfWork>(sp => new RecordingUnitOfWork(sp.GetRequiredService<ApplicationDbContext>(), _recorder));
        });
    }
}

/// <summary>Starts a SQL Server container and boots the app in-process on the learning transport with SQL persistence.</summary>
public sealed class Fixture : IAsyncLifetime
{
    private MsSqlContainer? _sqlServer;
    private IEndpointInstance? _observer;
    private string _storage = null!;

    public PipelineRecorder Recorder { get; } = new();
    public NServiceBusFactory App { get; private set; } = null!;
    public string Database { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _storage = Path.Combine(Path.GetTempPath(), $"nsb-test-{Guid.NewGuid():N}");
        _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
        await _sqlServer.StartAsync();
        Database = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "TestMediatorNServiceBus"
        }.ConnectionString;

        // The DbContext takes an IMessageBus only when the outbox is enabled, so build it from whichever constructor was generated.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(Database).Options;
        var constructor = typeof(ApplicationDbContext).GetConstructors().Single();
        var arguments = constructor.GetParameters()
            .Select(p => p.ParameterType == typeof(DbContextOptions<ApplicationDbContext>) ? (object?)options : null)
            .ToArray();
        await using (var db = (ApplicationDbContext)constructor.Invoke(arguments))
        {
            await db.Database.EnsureCreatedAsync();
        }

        // The generated endpoint configuration reads these while the host is being built, before the factory's in-memory
        // configuration is applied, so they are supplied through the environment.
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", Database);
        Environment.SetEnvironmentVariable("NServiceBus__LearningTransport__StorageDirectory", _storage);
        Environment.SetEnvironmentVariable("NServiceBus__EnableInstallers", "true");
        Environment.SetEnvironmentVariable("NServiceBus__Recoverability__ImmediateRetries", "1");
        Environment.SetEnvironmentVariable("NServiceBus__Recoverability__DelayedRetries", "0");

        App = new NServiceBusFactory(Recorder, Database, _storage);
        _ = App.CreateClient();

        var observer = new EndpointConfiguration("test-observer");
        observer.UseTransport(new LearningTransport { StorageDirectory = _storage });
        observer.UseSerialization<SystemJsonSerializer>();
        observer.Conventions().DefiningEventsAs(t => t == typeof(OrderAcceptedEvent));
        observer.EnableInstallers();
        observer.SendFailedMessagesTo("error");
        _observer = await Endpoint.Start(observer);
    }

    public async Task DisposeAsync()
    {
        if (_observer is not null)
        {
            await _observer.Stop();
        }

        await App.DisposeAsync();
        if (_sqlServer is not null)
        {
            await _sqlServer.DisposeAsync();
        }

        try
        {
            Directory.Delete(_storage, recursive: true);
        }
        catch (IOException)
        {
        }
    }
}

[CollectionDefinition(Name)]
public sealed class FixtureCollection : ICollectionFixture<Fixture>
{
    public const string Name = "nservicebus";
}

/// <summary>
/// The app is generated in one mode at a time (Enable Outbox on or off). Each test says which mode it belongs to with a
/// trait and asserts the generated app really is in that mode, so a run against the wrong generation fails loudly.
/// Run one mode with: dotnet test --filter "Outbox=On" (or Outbox=Off).
/// </summary>
public static class Mode
{
    // With the outbox the generated NServiceBus handler enlists the handler's DbContext in the SQL persistence transaction.
    public static bool OutboxEnabled =>
        typeof(NServiceBusMessageBus).Assembly
            .GetType("TestMediator.Nsb.Infrastructure.Eventing.NServiceBusMessageHandler`1", throwOnError: true)!
            .GetConstructors().Single().GetParameters().Any(p => p.ParameterType == typeof(ApplicationDbContext));
}

public class PipelineShapeChecks
{
    [Fact]
    [Trait("Outbox", "Off")]
    public void NoOutbox_PipelineKeepsTheFlushItem()
    {
        Assert.False(Mode.OutboxEnabled, "This test belongs to the outbox-off generation.");
        var services = new ServiceCollection();
        TestMediator.Nsb.Infrastructure.DependencyInjection.AddInfrastructure(services,
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["NServiceBus:EndpointName"] = "shape",
                ["ConnectionStrings:DefaultConnection"] = "Server=.;Database=x"
            }).Build());

        var pipeline = services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<AcceptOrderCommand, Unit>))
            .Select(d => d.ImplementationType!.GetGenericTypeDefinition())
            .ToArray();

        Assert.Equal(
            new[]
            {
                typeof(LoggingBehaviour<,>), typeof(UnhandledExceptionBehaviour<,>), typeof(PerformanceBehaviour<,>),
                typeof(AuthorizationBehaviour<,>), typeof(MessageBusPublishBehaviour<,>), typeof(ValidationBehaviour<,>),
                typeof(UnitOfWorkBehaviour<,>)
            },
            pipeline);
    }

    [Fact]
    [Trait("Outbox", "On")]
    public void SqlOutbox_PipelineHasNoFlushItem()
    {
        Assert.True(Mode.OutboxEnabled, "This test belongs to the outbox-on generation.");
        var services = new ServiceCollection();
        TestMediator.Nsb.Infrastructure.DependencyInjection.AddInfrastructure(services,
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["NServiceBus:EndpointName"] = "shape",
                ["ConnectionStrings:DefaultConnection"] = "Server=.;Database=x"
            }).Build());

        var pipeline = services
            .Where(d => d.ServiceType == typeof(IPipelineBehavior<AcceptOrderCommand, Unit>))
            .Select(d => d.ImplementationType!.GetGenericTypeDefinition())
            .ToArray();

        Assert.Equal(
            new[]
            {
                typeof(LoggingBehaviour<,>), typeof(UnhandledExceptionBehaviour<,>), typeof(PerformanceBehaviour<,>),
                typeof(AuthorizationBehaviour<,>), typeof(ValidationBehaviour<,>), typeof(UnitOfWorkBehaviour<,>)
            },
            pipeline);
    }
}

[Collection(FixtureCollection.Name)]
public class NServiceBusScenarios
{
    private readonly Fixture _fixture;
    private readonly ITestOutputHelper _output;

    public NServiceBusScenarios(Fixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    private sealed record Observation(int AcceptedCount, bool RowExistedOnArrival, int RowCount, IReadOnlyList<string> Entries);

    private async Task<Observation> PublishOrderPlacedAndObserveAsync(Guid orderId, TimeSpan wait, string customerName = "Olivia")
    {
        var accepted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var seen = new ConcurrentDictionary<string, bool>();
        var rowExistedOnArrival = false;
        AcceptedObserver.Callback = async (message, messageId) =>
        {
            if (message.OrderId != orderId)
            {
                return;
            }

            using var scope = _fixture.App.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            rowExistedOnArrival = await db.Orders.AnyAsync(o => o.Id == orderId);
            seen.TryAdd(messageId, true);
            accepted.TrySetResult(true);
        };

        var mark = _fixture.Recorder.Mark();
        try
        {
            var session = _fixture.App.Services.GetRequiredService<IMessageSession>();
            await session.Publish(new OrderPlacedEvent { OrderId = orderId, CustomerName = customerName });
            await Task.WhenAny(accepted.Task, Task.Delay(wait));
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
        finally
        {
            AcceptedObserver.Callback = null;
        }

        var entries = _fixture.Recorder.Since(mark);
        _output.WriteLine(string.Join(" -> ", entries));
        using var verify = _fixture.App.Services.CreateScope();
        var rows = await verify.ServiceProvider.GetRequiredService<ApplicationDbContext>().Orders.CountAsync(o => o.Id == orderId);
        return new Observation(seen.Count, rowExistedOnArrival, rows, entries);
    }

    [Fact]
    [Trait("Outbox", "On")]
    public async Task HandlerThatInvokesACommand_RunsOnTheSqlOutboxTransaction_AndDeliversExactlyOnceAfterCommit()
    {
        Assert.True(Mode.OutboxEnabled, "This test belongs to the outbox-on generation.");
        var orderId = Guid.NewGuid();

        var result = await PublishOrderPlacedAndObserveAsync(orderId, TimeSpan.FromSeconds(30));

        // NServiceBus opens the SQL transaction (outbox) before the handler runs, so the nested command must not open a TransactionScope.
        Assert.Contains("hasDbTransaction=True", result.Entries);
        Assert.DoesNotContain("hasDbTransaction=False", result.Entries);
        Assert.DoesNotContain("save:ambientScope=True", result.Entries);
        Assert.Contains("publish:OrderAcceptedEvent", result.Entries);

        Assert.Equal(1, result.AcceptedCount);
        Assert.True(result.RowExistedOnArrival, "OrderAcceptedEvent reached the transport before the order was committed.");
        Assert.Equal(1, result.RowCount);
    }

    [Fact]
    [Trait("Outbox", "Off")]
    public async Task NoOutbox_NestedCommandPublishIsDeliveredOnceAfterTheHandlerCompletes()
    {
        Assert.False(Mode.OutboxEnabled, "This test belongs to the outbox-off generation.");
        var orderId = Guid.NewGuid();

        var result = await PublishOrderPlacedAndObserveAsync(orderId, TimeSpan.FromSeconds(30));

        Assert.Contains("publish:OrderAcceptedEvent", result.Entries);
        Assert.Equal(1, result.AcceptedCount);
        Assert.True(result.RowExistedOnArrival, "OrderAcceptedEvent reached the transport before the order was committed.");
        Assert.Equal(1, result.RowCount);
    }

    [Fact]
    [Trait("Outbox", "Off")]
    public async Task NoOutbox_PublishFromANestedCommandDoesNotEscapeWhenTheHandlerFailsAfterwards()
    {
        Assert.False(Mode.OutboxEnabled, "This test belongs to the outbox-off generation.");
        var orderId = Guid.NewGuid();

        // The nested command commits its own unit of work and its pipeline flush runs inside the handler. The handler then
        // throws. NServiceBus batches outgoing messages until the handler completes, so the message must be discarded.
        var result = await PublishOrderPlacedAndObserveAsync(orderId, TimeSpan.FromSeconds(8), customerName: "FailAfterDispatch");

        Assert.Contains("publish:OrderAcceptedEvent", result.Entries);
        Assert.Contains("flush", result.Entries);
        Assert.Equal(0, result.AcceptedCount);
    }

    [Fact]
    [Trait("Outbox", "On")]
    public async Task HandlerWhoseSaveChangesFails_DeliversNothingAndPersistsNothing()
    {
        Assert.True(Mode.OutboxEnabled, "This test belongs to the outbox-on generation.");
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
