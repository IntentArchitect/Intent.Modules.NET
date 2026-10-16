using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wolverine;
using Xunit;
using TestMediator.WolverineSubscribeOnly.Application.Orders.RecordOrder;
using TestMediator.WolverineSubscribeOnly.Eventing.Messages;
using AppMessageBus = TestMediator.WolverineSubscribeOnly.Application.Common.Eventing.IMessageBus;

namespace TestMediator.WolverineSubscribeOnly.IntegrationTests;

public sealed class SubscribeOnlyFactory : WebApplicationFactory<TestMediator.WolverineSubscribeOnly.Api.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
    }
}

/// <summary>
/// Wolverine, Outbox = None, subscribe-only, no persistence: the one configuration where the Mediator pipeline is empty.
/// </summary>
public class EmptyPipelineTests : IClassFixture<SubscribeOnlyFactory>
{
    private readonly SubscribeOnlyFactory _factory;

    public EmptyPipelineTests(SubscribeOnlyFactory factory) => _factory = factory;

    [Fact]
    public void Subscribe_only_app_registers_no_message_bus_and_generates_no_integration_middleware()
    {
        using var scope = _factory.Services.CreateScope();

        Assert.Null(scope.ServiceProvider.GetService<AppMessageBus>());
        Assert.Null(typeof(OrderPlacedEvent).Assembly.GetType(
            "TestMediator.WolverineSubscribeOnly.Infrastructure.Eventing.WolverineIntegrationEventMiddleware"));
    }

    [Fact]
    public async Task Command_dispatches_through_the_empty_pipeline()
    {
        // The generated pipeline would resolve MessageBusPublishBehaviour (and so the unregistered bus) if the flush item survived.
        using var scope = _factory.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var orderId = Guid.NewGuid();

        await sender.Send(new RecordOrderCommand(orderId, "direct"));

        Assert.Contains(RecordOrderCommandHandler.Recorded, r => r.OrderId == orderId && r.CustomerName == "direct");
    }

    [Fact]
    public async Task Integration_event_reaches_the_handler_and_its_command_runs_without_a_pipeline()
    {
        using var scope = _factory.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var orderId = Guid.NewGuid();

        await bus.PublishAsync(new OrderPlacedEvent { OrderId = orderId, CustomerName = "via-wolverine" });

        var deadline = DateTime.UtcNow.AddSeconds(15);
        while (DateTime.UtcNow < deadline && !RecordOrderCommandHandler.Recorded.Any(r => r.OrderId == orderId))
        {
            await Task.Delay(100);
        }

        Assert.Contains(RecordOrderCommandHandler.Recorded, r => r.OrderId == orderId && r.CustomerName == "via-wolverine");
    }
}
