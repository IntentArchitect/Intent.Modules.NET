using System.Net.Sockets;
using Testcontainers.RabbitMq;
using Xunit;

namespace CqrsSpike.Scenarios.Infrastructure;

/// <summary>
/// Starts (or reuses) a RabbitMQ broker on localhost:5672 and boots BOTH applications in-process
/// against it. One instance is shared by every scenario so that no two hosts compete for the same
/// MassTransit queues.
/// </summary>
public sealed class SpikeFixture : IAsyncLifetime
{
    public const string BrokerUser = "guest";
    public const string BrokerPassword = "guest";

    private RabbitMqContainer? _container;

    public PublisherFactory Publisher { get; private set; } = null!;
    public SubscriberFactory Subscriber { get; private set; } = null!;
    public PipelineRecorder Recorder { get; } = new();

    public async Task InitializeAsync()
    {
        if (!await IsPortOpenAsync("localhost", 5672))
        {
            _container = new RabbitMqBuilder()
                .WithImage("rabbitmq:3.13-management")
                .WithUsername(BrokerUser)
                .WithPassword(BrokerPassword)
                .WithPortBinding(5672, 5672)
                .Build();
            await _container.StartAsync();
        }

        Publisher = new PublisherFactory(Recorder);
        Subscriber = new SubscriberFactory();

        // Start both hosts before any scenario runs. MassTransitHostOptions.WaitUntilStarted (see
        // FactoryConfiguration) makes each start block until its bus is connected.
        _ = Publisher.CreateClient();
        _ = Subscriber.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await Publisher.DisposeAsync();
        await Subscriber.DisposeAsync();
        if (_container is not null)
        {
            await _container.DisposeAsync();
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
public sealed class SpikeCollection : ICollectionFixture<SpikeFixture>
{
    public const string Name = "spike";
}
