extern alias Sub;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using PubMessages = Wolverine.Publish.RabbitMQ.Eventing.Messages;
using PubContractsBus = Wolverine.Publish.RabbitMQ.Application.Common.Eventing.IMessageBus;
using SubOrderShippedEvent = Sub::Wolverine.Publish.RabbitMQ.Eventing.Messages.OrderShippedEvent;

namespace Wolverine.Reference.Tests;

/// <summary>
/// R17.3 and R7.5: a throwing Integration Event Handler is redelivered per the configured
/// retry-with-cooldown policy and then stops (lands on the Error Queue) once exhausted; an empty
/// Delays list degrades to no retry at all rather than retrying indefinitely.
/// </summary>
[Collection(nameof(ReferenceSolutionCollection))]
public class RetryAndErrorQueueTests : IAsyncLifetime
{
    private readonly ReferenceSolutionFixture _fixture;

    public RetryAndErrorQueueTests(ReferenceSolutionFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _fixture.StopHostsAsync();
    }

    [Fact]
    public async Task R17_3_Throwing_Handler_Is_Retried_With_Cooldown_Then_Stops_After_Exhaustion()
    {
        AlwaysThrowingHandler<SubOrderShippedEvent>.Reset();

        // Default policy: 00:00:01, 00:00:05, 00:00:15 -> 1 initial attempt + 3 retries = 4 attempts total.
        await _fixture.StartSubscriberAsync(useThrowingOrderShippedHandler: true, retryDelays: new[] { "00:00:01", "00:00:05", "00:00:15" });
        await _fixture.StartPublisherAsync();

        var evt = new PubMessages.OrderShippedEvent { OrderId = Guid.NewGuid(), ShippedAt = DateTime.UtcNow };
        using (var scope = _fixture.PublisherHost!.Services.CreateScope())
        {
            var bus = scope.ServiceProvider.GetRequiredService<PubContractsBus>();
            bus.Publish(evt);
            await bus.FlushAllAsync();
        }

        // Wait past the full retry cadence (1 + 5 + 15 = 21s) plus margin for the 4th (final) attempt.
        await WaitUntilAsync(() => AlwaysThrowingHandler<SubOrderShippedEvent>.Attempts.Count >= 4, TimeSpan.FromSeconds(40));

        AlwaysThrowingHandler<SubOrderShippedEvent>.Attempts.Count.ShouldBe(4,
            "expected exactly one initial attempt plus three cooldown retries");

        // Confirm exhaustion: no further attempts arrive after the policy is exhausted (i.e. the
        // message moved to the Error Queue rather than being retried indefinitely).
        var countAfterExhaustion = AlwaysThrowingHandler<SubOrderShippedEvent>.Attempts.Count;
        await Task.Delay(TimeSpan.FromSeconds(10));
        AlwaysThrowingHandler<SubOrderShippedEvent>.Attempts.Count.ShouldBe(countAfterExhaustion,
            "no further redelivery attempts should occur once the retry-with-cooldown policy is exhausted");
    }

    [Fact]
    public async Task R7_5_Empty_Delays_List_Degrades_To_No_Retry()
    {
        AlwaysThrowingHandler<SubOrderShippedEvent>.Reset();

        // Empty Delays list (explicitly present, zero entries) must degrade to no retry: first
        // failure goes straight to the Error Queue.
        await _fixture.StartSubscriberAsync(useThrowingOrderShippedHandler: true, retryDelays: Array.Empty<string>());
        await _fixture.StartPublisherAsync();

        var evt = new PubMessages.OrderShippedEvent { OrderId = Guid.NewGuid(), ShippedAt = DateTime.UtcNow };
        using (var scope = _fixture.PublisherHost!.Services.CreateScope())
        {
            var bus = scope.ServiceProvider.GetRequiredService<PubContractsBus>();
            bus.Publish(evt);
            await bus.FlushAllAsync();
        }

        await WaitUntilAsync(() => AlwaysThrowingHandler<SubOrderShippedEvent>.Attempts.Count >= 1, TimeSpan.FromSeconds(15));
        AlwaysThrowingHandler<SubOrderShippedEvent>.Attempts.Count.ShouldBe(1, "the first failure should go straight to the Error Queue with no retry");

        // Confirm no infinite/implicit retry happens even after waiting well past any cooldown window.
        await Task.Delay(TimeSpan.FromSeconds(10));
        AlwaysThrowingHandler<SubOrderShippedEvent>.Attempts.Count.ShouldBe(1, "an empty Delays list must not retry indefinitely");
    }

    private static async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(250));
        }
    }
}
