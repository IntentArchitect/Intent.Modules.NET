using CqrsSpike.Publisher.Application.Common.Behaviours;
using CqrsSpike.Publisher.Application.Common.Interfaces;
using CqrsSpike.Publisher.Application.Common.Models;
using CqrsSpike.Publisher.Application.Orders;
using CqrsSpike.Publisher.Application.Orders.CreateOrder;
using CqrsSpike.Publisher.Application.Orders.GetOrders;
using CqrsSpike.Publisher.Domain.Common;
using FluentValidation;
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
    public async Task PerformanceBehaviour_TimesEachCallSeparately_WhenAnInstanceIsReused()
    {
        // Behaviours are Transient (D23), but timing must stay per call even if an instance is reused,
        // as it was under the earlier Scoped lifetime.
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

    [Fact]
    public async Task ValidationBehaviour_RunsValidatorsOneAtATime()
    {
        // Async rules share the scoped DbContext, which rejects a second concurrent operation.
        var probe = new ConcurrencyProbe();
        var behaviour = new ValidationBehaviour<CreateOrderCommand, Guid>(
            [new AsyncRuleValidator(probe), new AsyncRuleValidator(probe)]);

        await behaviour.Handle(new CreateOrderCommand("A", 1m), (_, _) => ValueTask.FromResult(Guid.NewGuid()), CancellationToken.None);

        Assert.Equal(2, probe.Calls);
        Assert.Equal(1, probe.MaxInFlight);
    }

    private sealed class ConcurrencyProbe
    {
        private int _inFlight;
        private int _maxInFlight;
        private int _calls;

        public int MaxInFlight => _maxInFlight;
        public int Calls => _calls;

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _calls);
            var inFlight = Interlocked.Increment(ref _inFlight);
            int seen;
            while (inFlight > (seen = _maxInFlight) && Interlocked.CompareExchange(ref _maxInFlight, inFlight, seen) != seen)
            {
            }

            await Task.Delay(50, cancellationToken);
            Interlocked.Decrement(ref _inFlight);
        }
    }

    private sealed class AsyncRuleValidator : AbstractValidator<CreateOrderCommand>
    {
        public AsyncRuleValidator(ConcurrencyProbe probe)
        {
            RuleFor(x => x.CustomerName).MustAsync(async (_, ct) =>
            {
                await probe.RunAsync(ct);
                return true;
            });
        }
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
