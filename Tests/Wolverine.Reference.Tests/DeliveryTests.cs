extern alias Sub;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using PubMessages = Wolverine.Publish.RabbitMQ.Eventing.Messages;
using PubContractsBus = Wolverine.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;
using SubOrderShippedEvent = Sub::Wolverine.Publish.RabbitMQ.Eventing.Messages.OrderShippedEvent;
using SubProcessOrderCommand = Sub::Wolverine.Publish.RabbitMQ.Eventing.Messages.ProcessOrderCommand;

namespace Wolverine.Reference.Tests;

/// <summary>
/// R17.1 and R17.2: real RabbitMQ + real SQL Server (Testcontainers), the publisher and subscriber
/// Wolverine hosts run in-process against the same broker, and delivery is proven end-to-end.
/// </summary>
[Collection(nameof(ReferenceSolutionCollection))]
public class DeliveryTests : IAsyncLifetime
{
    private readonly ReferenceSolutionFixture _fixture;

    public DeliveryTests(ReferenceSolutionFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        ObservingHandler<SubOrderShippedEvent>.Reset();
        ObservingHandler<SubProcessOrderCommand>.Reset();
        await _fixture.StartSubscriberAsync(useObservingHandlers: true);
        await _fixture.StartPublisherAsync();
    }

    public async Task DisposeAsync()
    {
        await _fixture.StopHostsAsync();
    }

    [Fact]
    public async Task R17_1_Published_OrderShippedEvent_Reaches_Subscribers_Handler()
    {
        var waitTask = ObservingHandler<SubOrderShippedEvent>.WaitForNextAsync();

        var evt = new PubMessages.OrderShippedEvent { OrderId = Guid.NewGuid(), ShippedAt = DateTime.UtcNow };
        using (var scope = _fixture.PublisherHost!.Services.CreateScope())
        {
            var bus = scope.ServiceProvider.GetRequiredService<PubContractsBus>();
            bus.Publish(evt);
            await bus.FlushAllAsync();
        }

        var completed = await Task.WhenAny(waitTask, Task.Delay(TimeSpan.FromSeconds(30)));
        completed.ShouldBe(waitTask, "the subscriber's OrderShippedEventHandler should have received the published event within 30s");

        var received = await waitTask;
        received.OrderId.ShouldBe(evt.OrderId);
    }

    [Fact]
    public async Task R17_2_Sent_ProcessOrderCommand_Is_Handled_Exactly_Once()
    {
        var waitTask = ObservingHandler<SubProcessOrderCommand>.WaitForNextAsync();

        var cmd = new PubMessages.ProcessOrderCommand { OrderId = Guid.NewGuid() };
        using (var scope = _fixture.PublisherHost!.Services.CreateScope())
        {
            var bus = scope.ServiceProvider.GetRequiredService<PubContractsBus>();
            bus.Send(cmd);
            await bus.FlushAllAsync();
        }

        var completed = await Task.WhenAny(waitTask, Task.Delay(TimeSpan.FromSeconds(30)));
        completed.ShouldBe(waitTask, "the subscriber's ProcessOrderCommandHandler should have received the sent command within 30s");

        // Give any duplicate delivery a chance to arrive before asserting exactly-once.
        await Task.Delay(TimeSpan.FromSeconds(3));

        ObservingHandler<SubProcessOrderCommand>.Received.Count.ShouldBe(1, "the command must be handled exactly once");
        ObservingHandler<SubProcessOrderCommand>.Received[0].OrderId.ShouldBe(cmd.OrderId);
    }
}
