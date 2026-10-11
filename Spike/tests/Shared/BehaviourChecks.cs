using CqrsSpike.Publisher.Application.Common.Behaviours;
using CqrsSpike.Publisher.Application.Common.Interfaces;
using CqrsSpike.Publisher.Application.Common.Models;
using CqrsSpike.Publisher.Application.Orders;
using CqrsSpike.Publisher.Application.Orders.GetOrders;
using CqrsSpike.Publisher.Domain.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CqrsSpike.Scenarios;

/// <summary>
/// In-process checks of pipeline pieces that need neither a broker nor a running host.
/// </summary>
public class BehaviourChecks
{
    [Fact]
    public async Task PerformanceBehaviour_TimesEachCallSeparately_WhenTheScopedInstanceIsReused()
    {
        // Mediator registers behaviours Scoped, so one instance serves every GetOrdersQuery in a scope.
        var logger = new CountingLogger<PerformanceBehaviour<GetOrdersQuery, List<OrderDto>>>();
        var behaviour = new PerformanceBehaviour<GetOrdersQuery, List<OrderDto>>(
            logger, new NoCurrentUser(), new ConfigurationBuilder().Build());

        // Four 150 ms calls: each is well under the 500 ms "long running" threshold, while time
        // accumulated across calls (the defect) passes it on the fourth (~600 ms).
        for (var i = 0; i < 4; i++)
        {
            await behaviour.Handle(new GetOrdersQuery(), async (_, ct) =>
            {
                await Task.Delay(150, ct);
                return new List<OrderDto>();
            }, CancellationToken.None);
        }

        Assert.Equal(0, logger.Warnings);
    }

    [Fact]
    public void DomainEventNotificationFactory_Throws_WhenAnEventHasNoNotification()
    {
        Assert.Throws<InvalidOperationException>(() => DomainEventNotificationFactory.Create(new UnmappedDomainEvent()));
    }

    private sealed class UnmappedDomainEvent : DomainEvent
    {
    }

    private sealed class NoCurrentUser : ICurrentUserService
    {
        public Task<ICurrentUser?> GetAsync() => Task.FromResult<ICurrentUser?>(null);
        public Task<bool> IsInRoleAsync(string role) => Task.FromResult(false);
        public Task<bool> AuthorizeAsync(string policy) => Task.FromResult(false);
    }

    private sealed class CountingLogger<T> : ILogger<T>
    {
        public int Warnings { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Warning)
            {
                Warnings++;
            }
        }
    }
}
