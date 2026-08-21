extern alias Sub;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Wolverine;
using PubMessages = Wolverine.Publish.RabbitMQ.Eventing.Messages;
using PubEventing = Wolverine.Publish.RabbitMQ.Infrastructure.Eventing;
using PubPersistence = Wolverine.Publish.RabbitMQ.Infrastructure.Persistence;
using PubContractsBus = Wolverine.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;
using SubEventing = Sub::Wolverine.Subscribe.RabbitMQ.Infrastructure.Eventing;
using SubPersistence = Sub::Wolverine.Subscribe.RabbitMQ.Infrastructure.Persistence;
using SubHandlers = Sub::Wolverine.Subscribe.RabbitMQ.Application.Common.Eventing;
// The subscriber project compiles its own copy of the message contract (same namespace and type
// name as the publisher's, different assembly - see IntegrationEvents/OrderShippedEvent.cs in both
// projects). Wolverine matches messages across the wire by full type name, not by shared CLR
// identity, so the subscriber-side handler/consumer wiring must use ITS OWN compiled copy of the
// message type, reached here via the `Sub` extern alias.
using SubOrderShippedEvent = Sub::Wolverine.Publish.RabbitMQ.Eventing.Messages.OrderShippedEvent;
using SubProcessOrderCommand = Sub::Wolverine.Publish.RabbitMQ.Eventing.Messages.ProcessOrderCommand;

namespace Wolverine.Reference.Tests;

/// <summary>
/// Test-only observation handler. Registered ONLY in this test host, in place of the shipped
/// scaffold handler (whose generated body is a `throw new NotImplementedException(...)` TODO stub -
/// there is no real business logic to invoke). Records that the message reached the handler
/// resolution point exactly as the real Consumer -> IIntegrationEventHandler&lt;T&gt; wiring would.
/// The shipped Consumer/Handler files under Wolverine.Subscribe.RabbitMQ are not modified.
/// </summary>
public class ObservingHandler<TMessage> : SubHandlers.IIntegrationEventHandler<TMessage>
    where TMessage : class
{
    public static readonly List<TMessage> Received = new();
    private static TaskCompletionSource<TMessage>? _tcs;
    private static readonly object Lock = new();

    public static void Reset()
    {
        lock (Lock)
        {
            Received.Clear();
            _tcs = null;
        }
    }

    public static Task<TMessage> WaitForNextAsync()
    {
        lock (Lock)
        {
            _tcs = new TaskCompletionSource<TMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
            return _tcs.Task;
        }
    }

    public Task HandleAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        lock (Lock)
        {
            Received.Add(message);
            _tcs?.TrySetResult(message);
        }
        return Task.CompletedTask;
    }
}

/// <summary>
/// Test-only handler that always throws, used to prove R17.3 (retry-with-cooldown then Error Queue,
/// and R7.5's empty-delay-list degradation). Records the wall-clock time of every attempt so the test
/// can assert both the retry cadence and that attempts stop once the policy is exhausted.
/// </summary>
public class AlwaysThrowingHandler<TMessage> : SubHandlers.IIntegrationEventHandler<TMessage>
    where TMessage : class
{
    public static readonly List<DateTime> Attempts = new();
    private static readonly object Lock = new();

    public static void Reset()
    {
        lock (Lock)
        {
            Attempts.Clear();
        }
    }

    public Task HandleAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        lock (Lock)
        {
            Attempts.Add(DateTime.UtcNow);
        }
        throw new InvalidOperationException("Deliberate failure for R17.3 test.");
    }
}

public sealed class ReferenceSolutionFixture : IAsyncLifetime
{
    public RabbitMqContainer RabbitMq { get; } = new RabbitMqBuilder()
        .WithImage("rabbitmq:3.13-management")
        .Build();

    public MsSqlContainer Sql { get; } = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public IHost? PublisherHost { get; private set; }
    public IHost? SubscriberHost { get; private set; }

    private string PublisherDbConnectionString => ReplaceDatabase(Sql.GetConnectionString(), "PublisherDb");
    private string SubscriberDbConnectionString => ReplaceDatabase(Sql.GetConnectionString(), "SubscriberDb");

    private static string ReplaceDatabase(string connectionString, string dbName)
    {
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = dbName
        };
        return builder.ConnectionString;
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(RabbitMq.StartAsync(), Sql.StartAsync());
        await EnsureDatabaseAsync(PublisherDbConnectionString);
        await EnsureDatabaseAsync(SubscriberDbConnectionString);
    }

    private async Task EnsureDatabaseAsync(string connectionString)
    {
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
        var dbName = builder.InitialCatalog;
        builder.InitialCatalog = "master";
        await using var connection = new Microsoft.Data.SqlClient.SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"IF DB_ID('{dbName}') IS NULL CREATE DATABASE [{dbName}]";
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IHost> StartPublisherAsync()
    {
        var uri = new Uri(RabbitMq.GetConnectionString());
        var config = BuildConfig(uri, PublisherDbConnectionString, retryDelays: new[] { "00:00:01", "00:00:05", "00:00:15" });

        var host = Host.CreateDefaultBuilder()
            .ConfigureLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Debug))
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IConfiguration>(config);
                services.AddDbContext<PubPersistence.ApplicationDbContext>(o => o.UseSqlServer(PublisherDbConnectionString));
                services.AddScoped<PubContractsBus, PubEventing.WolverineMessageBus>();
            })
            .UseWolverine(opts => PubEventing.WolverineEventingConfiguration.ConfigureRabbitMq(opts, config))
            .Build();

        await host.StartAsync();
        PublisherHost = host;
        return host;
    }

    public async Task<IHost> StartSubscriberAsync(
        bool useObservingHandlers = true,
        bool useThrowingOrderShippedHandler = false,
        string[]? retryDelays = null)
    {
        var uri = new Uri(RabbitMq.GetConnectionString());
        var config = BuildConfig(uri, SubscriberDbConnectionString, retryDelays: retryDelays ?? new[] { "00:00:01", "00:00:05", "00:00:15" });

        var host = Host.CreateDefaultBuilder()
            .ConfigureLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Debug))
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IConfiguration>(config);
                services.AddDbContext<SubPersistence.ApplicationDbContext>(o => o.UseSqlServer(SubscriberDbConnectionString));

                if (useThrowingOrderShippedHandler)
                {
                    services.AddTransient<SubHandlers.IIntegrationEventHandler<SubOrderShippedEvent>, AlwaysThrowingHandler<SubOrderShippedEvent>>();
                }
                else if (useObservingHandlers)
                {
                    services.AddTransient<SubHandlers.IIntegrationEventHandler<SubOrderShippedEvent>, ObservingHandler<SubOrderShippedEvent>>();
                }

                if (useObservingHandlers)
                {
                    services.AddTransient<SubHandlers.IIntegrationEventHandler<SubProcessOrderCommand>, ObservingHandler<SubProcessOrderCommand>>();
                }
            })
            .UseWolverine(opts => SubEventing.WolverineEventingConfiguration.ConfigureRabbitMq(opts, config))
            .Build();

        await host.StartAsync();
        SubscriberHost = host;
        return host;
    }

    private static IConfiguration BuildConfig(Uri rabbitUri, string sqlConnectionString, string[] retryDelays)
    {
        var dict = new Dictionary<string, string?>
        {
            ["Wolverine:RabbitMq:Host"] = rabbitUri.Host,
            ["Wolverine:RabbitMq:Port"] = rabbitUri.Port.ToString(),
            ["Wolverine:RabbitMq:VirtualHost"] = string.IsNullOrEmpty(rabbitUri.AbsolutePath) || rabbitUri.AbsolutePath == "/" ? "/" : rabbitUri.AbsolutePath.TrimStart('/'),
            ["Wolverine:RabbitMq:Username"] = string.IsNullOrEmpty(rabbitUri.UserInfo) ? "guest" : rabbitUri.UserInfo.Split(':')[0],
            ["Wolverine:RabbitMq:Password"] = string.IsNullOrEmpty(rabbitUri.UserInfo) ? "guest" : rabbitUri.UserInfo.Split(':')[1],
            ["ConnectionStrings:DefaultConnection"] = sqlConnectionString,
        };

        for (var i = 0; i < retryDelays.Length; i++)
        {
            dict[$"Wolverine:ErrorHandling:RetryWithCooldown:Delays:{i}"] = retryDelays[i];
        }

        return new ConfigurationBuilder().AddInMemoryCollection(dict!).Build();
    }

    public async Task StopHostsAsync()
    {
        if (PublisherHost is not null)
        {
            await PublisherHost.StopAsync();
            PublisherHost.Dispose();
            PublisherHost = null;
        }

        if (SubscriberHost is not null)
        {
            await SubscriberHost.StopAsync();
            SubscriberHost.Dispose();
            SubscriberHost = null;
        }
    }

    public async Task DisposeAsync()
    {
        await StopHostsAsync();
        await RabbitMq.DisposeAsync();
        await Sql.DisposeAsync();
    }
}
