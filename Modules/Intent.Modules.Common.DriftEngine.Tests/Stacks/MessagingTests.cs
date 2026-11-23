using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// Domain events raised, integration events published and commands sent (<c>intent.messaging</c>): a link is borne out by
/// code that creates its message and hands it to a call, whatever the call is, and a message the code raises that no link
/// covers is drift by default.
/// </summary>
public class MessagingTests
{
    private static readonly JsonNode Model = JsonNode.Parse("""
        {
          "designers": {
            "Domain": {
              "packages": [{
                "id": "domain", "name": "Shop.Domain",
                "elements": [
                  { "id": "order", "type": "Class", "name": "Order",
                    "children": [
                      { "id": "order-ctor", "type": "Class Constructor", "name": "Order",
                        "children": [{ "id": "order-ctor-user", "type": "Parameter", "name": "userId", "typeRef": { "type": "string" } }] },
                      { "id": "order-ship", "type": "Operation", "name": "SetShippedStatus" },
                      { "id": "order-pay", "type": "Operation", "name": "SetPaidStatus" },
                      { "id": "order-cancel", "type": "Operation", "name": "Cancel" },
                      { "id": "order-complete", "type": "Operation", "name": "Complete" }
                    ] },
                  { "id": "started", "type": "Domain Event", "name": "OrderStartedDomainEvent" },
                  { "id": "shipped", "type": "Domain Event", "name": "OrderShippedDomainEvent" },
                  { "id": "paid", "type": "Domain Event", "name": "OrderPaidDomainEvent" }
                ]
              }],
              "associations": [
                { "id": "ctor-raises-started", "type": "Domain Event Origin Association", "source": { "element": "order-ctor", "navigable": false }, "target": { "element": "started" } },
                { "id": "ship-raises-shipped", "type": "Domain Event Origin Association", "source": { "element": "order-ship", "navigable": false }, "target": { "element": "shipped" } },
                { "id": "pay-raises-paid", "type": "Domain Event Origin Association", "source": { "element": "order-pay", "navigable": false }, "target": { "element": "paid" } },
                { "id": "complete-raises-shipped", "type": "Domain Event Origin Association", "source": { "element": "order-complete", "navigable": false }, "target": { "element": "shipped" } }
              ]
            },
            "Services": {
              "packages": [{
                "id": "services", "name": "Shop.Api",
                "elements": [
                  { "id": "create-order", "type": "Command", "name": "CreateOrderCommand" },
                  { "id": "on-shipped", "type": "Domain Event Handler", "name": "OrderShippedDomainEventHandler" },
                  { "id": "order-started-ie", "type": "Message", "name": "OrderStartedIntegrationEvent" },
                  { "id": "order-shipped-ie", "type": "Message", "name": "OrderShippedIntegrationEvent" },
                  { "id": "ship-order", "type": "Command", "name": "ShipOrderCommand" },
                  { "id": "notify-buyer", "type": "Integration Command", "name": "NotifyBuyerCommand" },
                  { "id": "archive-order", "type": "Command", "name": "ArchiveOrderCommand" },
                  { "id": "order-archived-ie", "type": "Message", "name": "OrderArchivedIntegrationEvent" }
                ]
              }],
              "associations": [
                { "id": "ship-order-sends", "type": "Send Integration Command", "source": { "element": "ship-order", "navigable": false }, "target": { "element": "notify-buyer" } },
                { "id": "create-order-publishes", "type": "Publish Integration Event", "source": { "element": "create-order", "navigable": false }, "target": { "element": "order-started-ie" } },
                { "id": "on-shipped-handles", "type": "Domain Event Handler Association", "source": { "element": "on-shipped", "navigable": false }, "target": { "element": "shipped" } },
                { "id": "on-shipped-publishes", "type": "Publish Integration Event", "source": { "element": "on-shipped-handles", "navigable": false }, "target": { "element": "order-shipped-ie" } }
              ]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
            }
          }
        }
        """)!;

    private static readonly Dictionary<string, string> Code = new()
    {
        ["src/Domain/Order.cs"] = """
            namespace Shop.Domain;
            public class Order : Entity
            {
                public Order(string userId)
                {
                    AddStartedEvent(userId);
                }

                public void SetShippedStatus() => AddDomainEvent(new OrderShippedDomainEvent(this));

                public void SetPaidStatus() { }

                public void Cancel()
                {
                    var cancelled = new OrderCancelledDomainEvent(this);
                    AddDomainEvent(cancelled);
                }

                public void Complete() => SetShippedStatus();

                private void AddStartedEvent(string userId) => AddDomainEvent(new OrderStartedDomainEvent(this, userId));

                public void Archive() => AddDomainEvent(new OrderShippedDomainEvent(this));
            }
            """,
        ["src/Domain/Entity.cs"] = """
            namespace Shop.Domain;
            public abstract class Entity
            {
                private readonly List<INotification> _domainEvents = [];

                public void AddDomainEvent(INotification item) => _domainEvents.Add(item);
            }
            """,
        ["src/Domain/Events.cs"] = """
            namespace Shop.Domain;
            public record OrderStartedDomainEvent(Order Order, string UserId) : INotification;
            public record OrderShippedDomainEvent(Order Order) : INotification;
            public record OrderPaidDomainEvent(Order Order) : INotification;
            public record OrderCancelledDomainEvent(Order Order) : INotification;
            """,
        ["src/Api/IntegrationEvents.cs"] = """
            namespace Shop.Api;
            public record OrderStartedIntegrationEvent(string UserId) : IntegrationEvent;
            public record OrderShippedIntegrationEvent(int OrderId) : IntegrationEvent;
            public record GracePeriodConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;
            """,
        ["src/Api/Handlers.cs"] = """
            namespace Shop.Api;
            public record CreateOrderCommand(string UserId);

            public class CreateOrderCommandHandler(IOutbox outbox)
            {
                public async Task<bool> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
                {
                    var started = new OrderStartedIntegrationEvent(command.UserId);
                    await outbox.AddAndSaveEventAsync(started);
                    return true;
                }
            }

            public class OrderShippedDomainEventHandler(IOutbox outbox)
            {
                public Task Handle(OrderShippedDomainEvent domainEvent, CancellationToken cancellationToken) =>
                    outbox.AddAndSaveEventAsync(new OrderShippedIntegrationEvent(1));
            }

            public class GracePeriodManager(IEventBus bus)
            {
                public Task CheckAsync(int orderId) => bus.PublishAsync(new GracePeriodConfirmedIntegrationEvent(orderId));

                public Task ReplayAsync(int orderId) => Process(new GracePeriodConfirmedIntegrationEvent(orderId));

                private Task Process(GracePeriodConfirmedIntegrationEvent message) => Task.CompletedTask;
            }
            """,
        ["src/Api/Collaborators.cs"] = """
            namespace Shop.Api;
            public record ShipOrderCommand(int OrderId);
            public record NotifyBuyerCommand(int OrderId);
            public record OrderExpiredIntegrationEvent(int OrderId) : IntegrationEvent;
            public record LogEntry(IntegrationEvent Event);

            public interface INotifier
            {
                Task Notify(int orderId);
            }

            public class Notifier(IMessageSession session) : INotifier
            {
                public Task Notify(int orderId) => session.Send(new NotifyBuyerCommand(orderId));
            }

            public class ShipOrderCommandHandler
            {
                private readonly INotifier _notifier;

                public ShipOrderCommandHandler(INotifier notifier)
                {
                    _notifier = notifier;
                }

                public Task Handle(ShipOrderCommand command, CancellationToken cancellationToken) => _notifier.Notify(command.OrderId);
            }

            public class Archiver
            {
                public void Archive(NotifyBuyerCommand command) => Console.WriteLine(command.OrderId);
            }

            public class Auditor(Archiver archiver)
            {
                public void Audit(int orderId) => archiver.Archive(new NotifyBuyerCommand(orderId));
            }

            public class CommandFactory
            {
                public Task<NotifyBuyerCommand> MakeAsync(int orderId)
                {
                    var command = new NotifyBuyerCommand(orderId);
                    return Task.FromResult(command);
                }
            }

            public class EventLog(ICollection<LogEntry> entries)
            {
                public void Save(IntegrationEvent @event)
                {
                    var entry = new LogEntry(@event);
                    entries.Add(entry);
                }
            }

            public class Expirer(EventLog log)
            {
                public void Expire(int orderId) => log.Save(new OrderExpiredIntegrationEvent(orderId));
            }

            public record ArchiveOrderCommand(int OrderId);
            public record OrderArchivedIntegrationEvent(int OrderId) : IntegrationEvent;

            public class ArchivePublisher(EventLog log)
            {
                public void PublishArchived(int orderId) => log.Save(new OrderArchivedIntegrationEvent(orderId));
            }

            public class ArchiveOrderCommandHandler(ArchivePublisher publisher)
            {
                public void Handle(ArchiveOrderCommand command, CancellationToken cancellationToken) => publisher.PublishArchived(command.OrderId);
            }

            public class OrderPaidHandler(IEventBus bus)
            {
                public Task Handle(OrderPaidDomainEvent domainEvent, CancellationToken cancellationToken) => bus.PublishAsync(new OrderArchivedIntegrationEvent(2));
            }

            public class ArchiveRouter(IEventBus bus)
            {
                public Task RouteAsync(int orderId) => bus.PublishAsync(new OrderArchivedIntegrationEvent(orderId));
            }
            """
    };

    private static readonly RunResult Result = Run();

    [Fact]
    public void A_domain_event_raised_through_any_call_bears_out_its_link()
    {
        Claim("ship-raises-shipped").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_domain_event_raised_in_a_private_method_the_constructor_calls_bears_out_its_link()
    {
        Claim("ctor-raises-started").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_method_raises_what_the_methods_of_its_class_it_calls_raise()
    {
        Claim("complete-raises-shipped").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_linked_domain_event_the_code_never_raises_is_drift()
    {
        Claim("pay-raises-paid").Outcome.ShouldBe("contradicted");
        Result.Findings.Single(x => x.Rule == "claim/intent.raises").Message.ShouldContain("`SetPaidStatus` raises `OrderPaidDomainEvent` in the model, but it isn't in the code");
    }

    [Fact]
    public void A_command_publishes_through_the_method_that_takes_it_even_through_a_local()
    {
        Claim("create-order-publishes").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_domain_event_handler_publishes_through_its_method_that_takes_the_event()
    {
        Claim("on-shipped-publishes").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_modelled_operation_raising_an_unmodelled_domain_event_with_no_link_is_drift()
    {
        Finding("messaging/unmodelled.raise", "Order.Cancel").Message.ShouldBe(
            "`Order.Cancel` raises `OrderCancelledDomainEvent` (src/Domain/Order.cs:16), which the diagram doesn't show. Model the Domain Event `OrderCancelledDomainEvent`, then add a Domain Event Origin link to `OrderCancelledDomainEvent` from `Order.Cancel`.");
    }

    [Fact]
    public void A_modelled_class_raising_from_a_method_the_diagram_doesnt_show_is_drift()
    {
        Finding("messaging/unmodelled.raise", "Order.Archive").Message.ShouldBe(
            "`Order.Archive` raises `OrderShippedDomainEvent` (src/Domain/Order.cs:23), which the diagram doesn't show. Model `Archive` on `Order`, then add a Domain Event Origin link from it to `OrderShippedDomainEvent`.");
    }

    [Fact]
    public void A_command_sends_through_a_collaborator_it_calls_through_an_interface()
    {
        Claim("ship-order-sends").Outcome.ShouldBe("verified");
    }

    /// <summary>The helper hands the event to a log that stores it in an entry it creates, which is a publish.</summary>
    [Fact]
    public void A_publish_in_a_helper_a_modelled_command_calls_is_the_commands_and_drift_without_its_link()
    {
        Finding("messaging/unmodelled.publish", "ArchivePublisher.PublishArchived").Message.ShouldEndWith(
            "which the diagram doesn't show. Add a Publish Integration Event link to `OrderArchivedIntegrationEvent` from `ArchiveOrderCommand`, whose code calls this.");
    }

    [Fact]
    public void A_publish_in_an_unmodelled_handler_of_a_modelled_event_is_drift()
    {
        Finding("messaging/unmodelled.publish", "OrderPaidHandler.Handle").Message.ShouldEndWith(
            "which the diagram doesn't show. Model `OrderPaidHandler` as a handler of `OrderPaidDomainEvent`, then add a Publish Integration Event link to `OrderArchivedIntegrationEvent` from its subscription.");
    }

    [Fact]
    public void A_message_type_the_diagram_doesnt_show_is_drift_wherever_it_is_published()
    {
        Finding("messaging/unmodelled.message", "GracePeriodConfirmedIntegrationEvent").Message.ShouldBe(
            "`GracePeriodConfirmedIntegrationEvent` (src/Api/IntegrationEvents.cs:4) is an integration event the diagram doesn't show: it has what every modelled integration event has (`IntegrationEvent`). Model the Message `GracePeriodConfirmedIntegrationEvent`.");
    }

    /// <summary>
    /// <c>GracePeriodManager</c>, <c>Expirer</c> and <c>ArchiveRouter</c> publish from code the diagram can't show, so only
    /// their unmodelled messages are reported, once each. <c>Notifier</c>'s send is the link's, through the handler's
    /// collaborator. <c>ReplayAsync</c> and <c>Audit</c> hand their message to a method that only reads it, and
    /// <c>MakeAsync</c> returns its message to its caller. <c>OrderCancelledDomainEvent</c> isn't reported on its own: the
    /// raise's finding asks for it.
    /// </summary>
    [Fact]
    public void A_send_from_code_the_diagram_cant_show_a_link_covers_processed_in_place_or_returned_is_never_reported()
    {
        Result.Findings.Where(x => x.Rule.StartsWith("messaging/", StringComparison.Ordinal)).Select(x => x.Message[..x.Message.IndexOf('`', 1)].TrimStart('`') + " " + x.Rule).Order()
            .ShouldBe([
                "ArchivePublisher.PublishArchived messaging/unmodelled.publish",
                "GracePeriodConfirmedIntegrationEvent messaging/unmodelled.message",
                "Order.Archive messaging/unmodelled.raise",
                "Order.Cancel messaging/unmodelled.raise",
                "OrderExpiredIntegrationEvent messaging/unmodelled.message",
                "OrderPaidHandler.Handle messaging/unmodelled.publish"
            ]);
    }

    private static Claim Claim(string element) => Result.Claims.Single(x => x.Element == element);

    private static Finding Finding(string rule, string about) =>
        Result.Findings.Single(x => x.Rule == rule && x.Message.StartsWith($"`{about}`", StringComparison.Ordinal));

    /// <summary>
    /// The Software Factory hands the engine no referenced packages of another application (eShop's Ordering, Catalog and
    /// PaymentProcessor), so a subscriber's copy of another application's message is known only by the subscription that
    /// reaches it. Ordering owns <c>OrderStartedIntegrationEvent</c>, which teaches the family, and subscribes to
    /// OrderProcessor's <c>GracePeriodConfirmedIntegrationEvent</c> through a copy of its own: the copy isn't reported as a
    /// message the diagram doesn't show.
    /// </summary>
    [Fact]
    public void A_subscribers_copy_of_another_applications_message_is_that_message_when_its_package_isnt_handed_over()
    {
        var model = JsonNode.Parse("""
            {
              "designers": {
                "Services": {
                  "packages": [
                    { "id": "eventing", "name": "Ordering.API.Eventing", "type": "Eventing Package",
                      "elements": [{ "id": "order-started", "type": "Message", "name": "OrderStartedIntegrationEvent" }] },
                    { "id": "services", "name": "Ordering.API",
                      "elements": [{ "id": "grace-handler", "type": "Integration Event Handler", "name": "GracePeriodConfirmedIntegrationEventHandler" }] }
                  ],
                  "associations": [
                    { "id": "handler-subscribes-grace", "type": "Subscribe Integration Event", "source": { "element": "grace-handler" }, "target": { "element": "grace-confirmed" } }
                  ]
                },
                "Codebase Structure": {
                  "packages": [{ "id": "root", "name": "Ordering", "type": "Root Folder",
                    "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
                }
              },
              "references": {
                "Services": {
                  "packages": [{ "id": "processor-eventing", "name": "OrderProcessor.Eventing", "type": "Eventing Package",
                    "elements": [{ "id": "grace-confirmed", "type": "Message", "name": "GracePeriodConfirmedIntegrationEvent" }] }]
                }
              }
            }
            """)!;
        var code = new Dictionary<string, string>
        {
            ["src/Ordering.API/IntegrationEvents.cs"] = """
                namespace Ordering.API.IntegrationEvents;
                public record IntegrationEvent;
                public record OrderStartedIntegrationEvent(string UserId) : IntegrationEvent;
                public record GracePeriodConfirmedIntegrationEvent(int OrderId) : IntegrationEvent;
                public record OrderExpiredIntegrationEvent(int OrderId) : IntegrationEvent;
                public class GracePeriodConfirmedIntegrationEventHandler : IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
                {
                    public Task Handle(GracePeriodConfirmedIntegrationEvent @event) => Task.CompletedTask;
                }
                """
        };

        // Built without its references, as the Software Factory hands the engine only this application's packages.
        var result = Run(model, code);

        result.Findings.Where(x => x.Rule == "messaging/unmodelled.message").Select(x => x.Message[1..x.Message.IndexOf('`', 1)])
            .ShouldBe(["OrderExpiredIntegrationEvent"]);
    }

    private static RunResult Run() => Run(Model, Code);

    private static RunResult Run(JsonNode model, IReadOnlyDictionary<string, string> code)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-messaging-").FullName;
        try
        {
            foreach (var (path, source) in code)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            return DriftEngine.Run(new DriftInput(FixtureModel.Build(model), root,
                [Pack.Stock("intent.domain")!, Pack.Stock("intent.services")!],
                [StockExtensions.Find("csharp")!, StockExtensions.Find("intent.messaging")!]));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
