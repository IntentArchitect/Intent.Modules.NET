extern alias Sub;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.RabbitMq;
using Wolverine;
using PubEventing = Wolverine.Publish.RabbitMQ.Infrastructure.Eventing;
using PubContractsBus = Wolverine.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;
using SubEventing = Sub::Wolverine.Subscribe.RabbitMQ.Infrastructure.Eventing;
using SubHandlers = Sub::Wolverine.Subscribe.RabbitMQ.Application.Common.Eventing;
// The subscriber project compiles its own copy of the message contract - same namespace and type
// name as the publisher's, different assembly. That is how Intent's Eventing.Contracts works: each
// application generates its own copy, and Wolverine matches messages across the wire by type NAME
// rather than by shared CLR identity. The `Sub` extern alias is how this single test process refers
// to the subscriber's copy unambiguously.
using SubOrderShippedEvent = Sub::Wolverine.Publish.RabbitMQ.Eventing.Messages.OrderShippedEvent;
using SubProcessOrderCommand = Sub::Wolverine.Publish.RabbitMQ.Eventing.Messages.ProcessOrderCommand;

namespace Wolverine.Reference.Tests;

/// <summary>
/// Test-only observation handler, registered in place of the shipped scaffold handler (whose
/// generated body is a <c>throw new NotImplementedException</c> TODO stub, so there is no real
/// business logic to invoke). The generated Consumer is still the thing that receives from the
/// transport and resolves this - the Consumer is not bypassed.
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
/// Test-only handler that always throws, used to prove the Error Handling Policy re-delivers and
/// then stops. Records the wall-clock time of every attempt.
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
        throw new InvalidOperationException("Deliberate failure for the error-handling test.");
    }
}

/// <summary>
/// Transactional Outbox = None. One RabbitMQ container, no SQL Server and no database at all -
/// which is the point: the module's default configuration requires neither.
/// </summary>
public sealed class ReferenceSolutionFixture : IAsyncLifetime
{
    public RabbitMqContainer RabbitMq { get; } = new RabbitMqBuilder()
        .WithImage("rabbitmq:3.13-management")
        .Build();

    public IHost? PublisherHost { get; private set; }
    public IHost? SubscriberHost { get; private set; }

    public Task InitializeAsync() => RabbitMq.StartAsync();

    public async Task<IHost> StartPublisherAsync()
    {
        var config = BuildConfig(new Uri(RabbitMq.GetConnectionString()),
            retryDelays: new[] { "00:00:01", "00:00:05", "00:00:15" });

        var host = Host.CreateDefaultBuilder()
            .ConfigureLogging(b => b.ClearProviders().AddSimpleConsole(o => o.SingleLine = true).SetMinimumLevel(LogLevel.Warning))
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IConfiguration>(config);
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
        var config = BuildConfig(new Uri(RabbitMq.GetConnectionString()),
            retryDelays: retryDelays ?? new[] { "00:00:01", "00:00:05", "00:00:15" });

        var host = Host.CreateDefaultBuilder()
            .ConfigureLogging(b => b.ClearProviders().AddSimpleConsole(o => o.SingleLine = true).SetMinimumLevel(LogLevel.Warning))
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IConfiguration>(config);

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

    private static IConfiguration BuildConfig(Uri rabbitUri, string[] retryDelays)
    {
        var dict = new Dictionary<string, string?>
        {
            ["Wolverine:RabbitMq:Host"] = rabbitUri.Host,
            ["Wolverine:RabbitMq:Port"] = rabbitUri.Port.ToString(),
            ["Wolverine:RabbitMq:VirtualHost"] = string.IsNullOrEmpty(rabbitUri.AbsolutePath) || rabbitUri.AbsolutePath == "/" ? "/" : rabbitUri.AbsolutePath.TrimStart('/'),
            ["Wolverine:RabbitMq:Username"] = string.IsNullOrEmpty(rabbitUri.UserInfo) ? "guest" : rabbitUri.UserInfo.Split(':')[0],
            ["Wolverine:RabbitMq:Password"] = string.IsNullOrEmpty(rabbitUri.UserInfo) ? "guest" : rabbitUri.UserInfo.Split(':')[1],
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
    }
}
