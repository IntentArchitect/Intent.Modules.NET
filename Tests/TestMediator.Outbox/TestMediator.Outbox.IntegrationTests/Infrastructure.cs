using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Transactions;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Xunit;
using TestMediator.Outbox.Application.Common.Eventing;
using TestMediator.Outbox.Domain.Common.Interfaces;
using TestMediator.Outbox.Infrastructure.Eventing;
using TestMediator.Outbox.Infrastructure.Persistence;

namespace TestMediator.Outbox.IntegrationTests;

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

    public void Send<TMessage>(TMessage message, Uri address) where TMessage : class
    {
        _recorder.Record($"send:{typeof(TMessage).Name}");
        _inner.Send(message, address);
    }

    public Task FlushAllAsync(CancellationToken cancellationToken = default)
    {
        _recorder.Record("flush");
        return _inner.FlushAllAsync(cancellationToken);
    }
}

/// <summary>Records what the unit-of-work behaviour sees: whether EF already has a transaction, and whether an ambient TransactionScope exists when it saves.</summary>
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

public sealed class OutboxFactory : WebApplicationFactory<TestMediator.Outbox.Api.Program>
{
    private readonly PipelineRecorder _recorder;
    private readonly string _database;

    public OutboxFactory(PipelineRecorder recorder, string database)
    {
        _recorder = recorder;
        _database = database;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _database,
                ["RabbitMq:Host"] = "localhost",
                ["RabbitMq:VirtualHost"] = "/",
                ["RabbitMq:Username"] = Fixture.BrokerUser,
                ["RabbitMq:Password"] = Fixture.BrokerPassword,
                ["MassTransit:RetryInterval:RetryCount"] = "1",
                ["MassTransit:RetryInterval:Interval"] = "00:00:00.500",
            });
        });
        builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
        builder.ConfigureTestServices(services =>
        {
            services.Configure<MassTransitHostOptions>(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
            });
            services.AddSingleton(_recorder);
            // Decorate (never replace) the real collaborators so the pipeline can be observed while the real broker round trip keeps working.
            services.AddScoped<IMessageBus>(sp => new RecordingMessageBus(sp.GetRequiredService<MassTransitMessageBus>(), _recorder));
            services.AddScoped<IUnitOfWork>(sp => new RecordingUnitOfWork(sp.GetRequiredService<ApplicationDbContext>(), _recorder));
        });
    }
}

/// <summary>Starts (or reuses) RabbitMQ on localhost:5672 and a SQL Server container, then boots the Outbox app in-process.</summary>
public sealed class Fixture : IAsyncLifetime
{
    public const string BrokerUser = "guest";
    public const string BrokerPassword = "guest";

    private RabbitMqContainer? _broker;
    private MsSqlContainer? _sqlServer;

    public PipelineRecorder Recorder { get; } = new();
    public OutboxFactory App { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        if (!await IsPortOpenAsync("localhost", 5672))
        {
            _broker = new RabbitMqBuilder("rabbitmq:3.13-management")
                .WithUsername(BrokerUser)
                .WithPassword(BrokerPassword)
                .WithPortBinding(5672, 5672)
                .Build();
            await _broker.StartAsync();
        }

        _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
        await _sqlServer.StartAsync();
        var database = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(_sqlServer.GetConnectionString())
        {
            InitialCatalog = "TestMediatorOutbox"
        }.ConnectionString;

        await using (var db = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(database).Options, null!))
        {
            await db.Database.EnsureCreatedAsync();
        }

        App = new OutboxFactory(Recorder, database);
        _ = App.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await App.DisposeAsync();
        if (_broker is not null)
        {
            await _broker.DisposeAsync();
        }

        if (_sqlServer is not null)
        {
            await _sqlServer.DisposeAsync();
        }
    }

    private static async Task<bool> IsPortOpenAsync(string host, int port)
    {
        try
        {
            using var client = new TcpClient();
            var connect = client.ConnectAsync(host, port);
            return await Task.WhenAny(connect, Task.Delay(500)) == connect && client.Connected;
        }
        catch
        {
            return false;
        }
    }
}

[CollectionDefinition(Name)]
public sealed class FixtureCollection : ICollectionFixture<Fixture>
{
    public const string Name = "outbox";
}
