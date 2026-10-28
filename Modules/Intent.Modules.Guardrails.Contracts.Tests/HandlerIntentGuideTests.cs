using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;
using static Intent.Modules.Guardrails.Contracts.Tests.Guides;
using ServicesGuides = Intent.Modules.Services.Guardrails.Contracts.Templates.DriftRecipeGuides;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The handler intent guide's recipes, run against handlers in each style it covers (MediatR request handlers, a
    /// service class, Intent, MassTransit and NServiceBus event handlers, a MediatR domain event handler) and a model whose
    /// interactions agree with them: no drift, no warnings from validate-drift-recipes, every value read from the code, and
    /// each planted change reported as exactly its row.
    /// </summary>
    public class HandlerIntentGuideTests
    {
        private const string Services = FactKinds.Services;

        private static readonly Dictionary<string, string> Handlers = new()
        {
            // MediatR, a repository named after the entity, a child added through its aggregate, an outbox.
            ["src/Ordering.Application/Orders/CreateOrderCommandHandler.cs"] = """
                namespace Ordering.Application.Orders;

                public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
                {
                    private readonly IOrderRepository _orderRepository;
                    private readonly IOrderingIntegrationEventService _integrationEventService;

                    public CreateOrderCommandHandler(IOrderRepository orderRepository, IOrderingIntegrationEventService integrationEventService)
                    {
                        _orderRepository = orderRepository;
                        _integrationEventService = integrationEventService;
                    }

                    public async Task<bool> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
                    {
                        var orderStarted = new OrderStartedIntegrationEvent(message.UserId);
                        await _integrationEventService.AddAndSaveEventAsync(orderStarted);

                        var order = new Order(message.UserId, message.City);
                        foreach (var item in message.OrderItems)
                        {
                            order.AddOrderItem(item.ProductId, item.Units);
                        }

                        _orderRepository.Add(order);
                        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                    }
                }
                """,
            // An update that targets the entity's operation, and a command sent through MassTransit.
            ["src/Ordering.Application/Orders/CancelOrderCommandHandler.cs"] = """
                namespace Ordering.Application.Orders;

                public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
                {
                    private readonly IOrderRepository _orderRepository;
                    private readonly ISendEndpointProvider _sendEndpointProvider;

                    public CancelOrderCommandHandler(IOrderRepository orderRepository, ISendEndpointProvider sendEndpointProvider)
                    {
                        _orderRepository = orderRepository;
                        _sendEndpointProvider = sendEndpointProvider;
                    }

                    public async Task<bool> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
                    {
                        var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);
                        if (orderToUpdate == null)
                        {
                            return false;
                        }

                        orderToUpdate.SetCancelledStatus();

                        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:payments"));
                        await endpoint.Send(new CancelPaymentCommand(command.OrderNumber), cancellationToken);

                        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                    }
                }
                """,
            ["src/Ordering.Application/Orders/DeleteOrderCommandHandler.cs"] = """
                namespace Ordering.Application.Orders;

                public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
                {
                    private readonly IOrderRepository _orderRepository;

                    public DeleteOrderCommandHandler(IOrderRepository orderRepository)
                    {
                        _orderRepository = orderRepository;
                    }

                    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
                    {
                        var order = await _orderRepository.FindByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException($"Order {request.Id}");
                        _orderRepository.Remove(order);
                    }
                }
                """,
            // A child removed through its aggregate.
            ["src/Ordering.Application/Orders/RemoveOrderItemCommandHandler.cs"] = """
                namespace Ordering.Application.Orders;

                public class RemoveOrderItemCommandHandler : IRequestHandler<RemoveOrderItemCommand>
                {
                    private readonly IOrderRepository _orderRepository;

                    public RemoveOrderItemCommandHandler(IOrderRepository orderRepository)
                    {
                        _orderRepository = orderRepository;
                    }

                    public async Task Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
                    {
                        var order = await _orderRepository.FindByIdAsync(request.OrderId, cancellationToken);
                        order.RemoveOrderItem(request.ProductId);
                    }
                }
                """,
            // Queries through an EF Core DbSet, a repository and Set<T>().
            ["src/Ordering.Application/Orders/GetOrderByIdQueryHandler.cs"] = """
                namespace Ordering.Application.Orders;

                public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
                {
                    private readonly OrderingContext _dbContext;

                    public GetOrderByIdQueryHandler(OrderingContext dbContext)
                    {
                        _dbContext = dbContext;
                    }

                    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
                    {
                        var order = await _dbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.OrderItems)
                            .SingleOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
                        return order is null ? null : OrderDto.From(order);
                    }
                }
                """,
            ["src/Ordering.Application/Orders/GetOrdersQueryHandler.cs"] = """
                namespace Ordering.Application.Orders;

                public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
                {
                    private readonly IOrderRepository _orderRepository;

                    public GetOrdersQueryHandler(IOrderRepository orderRepository)
                    {
                        _orderRepository = orderRepository;
                    }

                    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
                    {
                        var orders = await _orderRepository.FindAllAsync(cancellationToken);
                        return orders.Select(OrderDto.From).ToList();
                    }
                }
                """,
            ["src/Ordering.Application/Buyers/BuyerHandlers.cs"] = """
                namespace Ordering.Application.Buyers;

                public class GetBuyersQueryHandler : IQueryHandler<GetBuyersQuery, List<BuyerDto>>
                {
                    private readonly OrderingContext _dbContext;

                    public GetBuyersQueryHandler(OrderingContext dbContext)
                    {
                        _dbContext = dbContext;
                    }

                    public async Task<List<BuyerDto>> Handle(GetBuyersQuery request, CancellationToken cancellationToken)
                    {
                        var buyers = await _dbContext.Set<Buyer>().ToListAsync(cancellationToken);
                        return buyers.Select(BuyerDto.From).ToList();
                    }
                }

                public class UpdateBuyerCommandHandler : ICommandHandler<UpdateBuyerCommand>
                {
                    private readonly OrderingContext _dbContext;

                    public UpdateBuyerCommandHandler(OrderingContext dbContext)
                    {
                        _dbContext = dbContext;
                    }

                    public async Task Handle(UpdateBuyerCommand request, CancellationToken cancellationToken)
                    {
                        var buyer = await _dbContext.Buyers.FindAsync(new object[] { request.Id }, cancellationToken);
                        buyer.Name = request.Name;
                        await _dbContext.SaveChangesAsync(cancellationToken);
                    }
                }

                public class DeleteBuyerCommandHandler : ICommandHandler<DeleteBuyerCommand>
                {
                    private readonly OrderingContext _dbContext;

                    public DeleteBuyerCommandHandler(OrderingContext dbContext)
                    {
                        _dbContext = dbContext;
                    }

                    public async Task Handle(DeleteBuyerCommand request, CancellationToken cancellationToken)
                    {
                        var buyer = await _dbContext.Buyers.FindAsync(new object[] { request.Id }, cancellationToken);
                        _dbContext.Buyers.Remove(buyer);
                        await _dbContext.SaveChangesAsync(cancellationToken);
                    }
                }
                """,
            // A service class: an untargeted update, a Service Proxy call, Intent's event bus, overloads that send a
            // command and call a stored procedure.
            ["src/Ordering.Application/Services/OrderService.cs"] = """
                namespace Ordering.Application.Services;

                public class OrderService : IOrderService
                {
                    private readonly IOrderRepository _orderRepository;
                    private readonly IPaymentsClient _paymentsClient;
                    private readonly IEventBus _eventBus;
                    private readonly ISender _sender;

                    public OrderService(IOrderRepository orderRepository, IPaymentsClient paymentsClient, IEventBus eventBus, ISender sender)
                    {
                        _orderRepository = orderRepository;
                        _paymentsClient = paymentsClient;
                        _eventBus = eventBus;
                        _sender = sender;
                    }

                    public async Task Approve(int orderId, CancellationToken cancellationToken = default)
                    {
                        var order = await _orderRepository.FindByIdAsync(orderId, cancellationToken);
                        order.Status = OrderStatus.Approved;
                        await _paymentsClient.ChargeAsync(orderId, cancellationToken);
                        _eventBus.Publish(new OrderApprovedEvent { OrderId = orderId });
                    }

                    public async Task Archive(int orderId)
                    {
                        await _sender.Send(new ArchiveOrderCommand(orderId));
                    }

                    public async Task Archive(int orderId, string reason)
                    {
                        await _orderRepository.ArchiveOrders(orderId, reason);
                    }
                }
                """,
            // Intent's integration event handler.
            ["src/Ordering.Application/IntegrationEvents/PaymentSucceededIntegrationEventHandler.cs"] = """
                namespace Ordering.Application.IntegrationEvents;

                public class PaymentSucceededIntegrationEventHandler : IIntegrationEventHandler<PaymentSucceededIntegrationEvent>
                {
                    private readonly ISender _sender;

                    public PaymentSucceededIntegrationEventHandler(ISender sender)
                    {
                        _sender = sender;
                    }

                    public async Task HandleAsync(PaymentSucceededIntegrationEvent message, CancellationToken cancellationToken = default)
                    {
                        await _sender.Send(new SetPaidOrderStatusCommand(message.OrderId), cancellationToken);
                    }
                }
                """,
            // A MassTransit consumer publishing an anonymous message.
            ["src/Ordering.Application/IntegrationEvents/StockConfirmedConsumer.cs"] = """
                namespace Ordering.Application.IntegrationEvents;

                public class StockConfirmedConsumer : IConsumer<StockConfirmedIntegrationEvent>
                {
                    private readonly IOrderRepository _orderRepository;

                    public StockConfirmedConsumer(IOrderRepository orderRepository)
                    {
                        _orderRepository = orderRepository;
                    }

                    public async Task Consume(ConsumeContext<StockConfirmedIntegrationEvent> context)
                    {
                        var order = await _orderRepository.FindByIdAsync(context.Message.OrderId);
                        order.SetStockConfirmedStatus();
                        await context.Publish<OrderStockConfirmedIntegrationEvent>(new { context.Message.OrderId });
                    }
                }
                """,
            // An NServiceBus handler.
            ["src/Ordering.Application/IntegrationEvents/GracePeriodConfirmedHandler.cs"] = """
                namespace Ordering.Application.IntegrationEvents;

                public class GracePeriodConfirmedHandler : IHandleMessages<GracePeriodConfirmedIntegrationEvent>
                {
                    public async Task Handle(GracePeriodConfirmedIntegrationEvent message, IMessageHandlerContext context)
                    {
                        await context.Send(new SubmitOrderForPaymentCommand(message.OrderId));
                    }
                }
                """,
            // A MediatR domain event handler creating through a static factory.
            ["src/Ordering.Application/DomainEventHandlers/OrderStartedDomainEventHandler.cs"] = """
                namespace Ordering.Application.DomainEventHandlers;

                public class OrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
                {
                    private readonly IBuyerRepository _buyerRepository;

                    public OrderStartedDomainEventHandler(IBuyerRepository buyerRepository)
                    {
                        _buyerRepository = buyerRepository;
                    }

                    public async Task Handle(OrderStartedDomainEvent domainEvent, CancellationToken cancellationToken)
                    {
                        var buyer = await _buyerRepository.FindAsync(domainEvent.UserId);
                        if (buyer is null)
                        {
                            buyer = Buyer.Create(domainEvent.UserId, domainEvent.UserName);
                            _buyerRepository.Add(buyer);
                        }
                    }
                }
                """
        };

        private static List<ModelFact> HandlersModel() => new()
        {
            Request(FactKinds.IntentCreates, "CreateOrderCommand", "Order", ("Entity", "Order"), ("Member", "")),
            Request(FactKinds.IntentCreates, "CreateOrderCommand", "OrderItem", ("Entity", "OrderItem"), ("Member", "")),
            Request(FactKinds.IntentPublishes, "CreateOrderCommand", "OrderStartedIntegrationEvent", ("Message", "OrderStartedIntegrationEvent")),
            Request(FactKinds.IntentUpdates, "CancelOrderCommand", "Order", ("Entity", "Order"), ("Member", "SetCancelledStatus")),
            Request(FactKinds.IntentSends, "CancelOrderCommand", "CancelPaymentCommand", ("Message", "CancelPaymentCommand")),
            Request(FactKinds.IntentDeletes, "DeleteOrderCommand", "Order", ("Entity", "Order")),
            Request(FactKinds.IntentDeletes, "RemoveOrderItemCommand", "OrderItem", ("Entity", "OrderItem")),
            Request(FactKinds.IntentQueries, "GetOrderByIdQuery", "Order", ("Entity", "Order")),
            Request(FactKinds.IntentQueries, "GetOrdersQuery", "Order", ("Entity", "Order")),
            Request(FactKinds.IntentQueries, "GetBuyersQuery", "Buyer", ("Entity", "Buyer")),
            Request(FactKinds.IntentUpdates, "UpdateBuyerCommand", "Buyer", ("Entity", "Buyer"), ("Member", "")),
            Request(FactKinds.IntentDeletes, "DeleteBuyerCommand", "Buyer", ("Entity", "Buyer")),
            ServiceOperation(FactKinds.IntentUpdates, "OrderService", "Approve", new[] { "orderId" }, "Order", ("Entity", "Order"), ("Member", "")),
            ServiceOperation(FactKinds.IntentInvokes, "OrderService", "Approve", new[] { "orderId" }, "ChargeAsync", ("Invoked", "ChargeAsync"), ("InvokedOwner", "PaymentsClient")),
            ServiceOperation(FactKinds.IntentPublishes, "OrderService", "Approve", new[] { "orderId" }, "OrderApprovedEvent", ("Message", "OrderApprovedEvent")),
            ServiceOperation(FactKinds.IntentInvokes, "OrderService", "Archive", new[] { "orderId" }, "ArchiveOrderCommand", ("Invoked", "ArchiveOrderCommand"), ("InvokedOwner", "")),
            ServiceOperation(FactKinds.IntentInvokes, "OrderService", "Archive", new[] { "orderId", "reason" }, "ArchiveOrders", ("Invoked", "ArchiveOrders"), ("InvokedOwner", "")),
            Subscription(FactKinds.IntentInvokes, "PaymentSucceededIntegrationEventHandler", "PaymentSucceededIntegrationEvent", "SetPaidOrderStatusCommand", ("Invoked", "SetPaidOrderStatusCommand"), ("InvokedOwner", "")),
            Subscription(FactKinds.IntentUpdates, "StockConfirmedConsumer", "StockConfirmedIntegrationEvent", "Order", ("Entity", "Order"), ("Member", "SetStockConfirmedStatus")),
            Subscription(FactKinds.IntentPublishes, "StockConfirmedConsumer", "StockConfirmedIntegrationEvent", "OrderStockConfirmedIntegrationEvent", ("Message", "OrderStockConfirmedIntegrationEvent")),
            Subscription(FactKinds.IntentSends, "GracePeriodConfirmedHandler", "GracePeriodConfirmedIntegrationEvent", "SubmitOrderForPaymentCommand", ("Message", "SubmitOrderForPaymentCommand")),
            Subscription(FactKinds.IntentCreates, "OrderStartedDomainEventHandler", "OrderStartedDomainEvent", "Buyer", ("Entity", "Buyer"), ("Member", "Create"))
        };

        private static string HandlersRecipe => RecipeFile(ServicesGuides.HandlerIntent);

        [Fact]
        public void The_handler_intent_recipes_read_every_handler_style_with_no_drift()
        {
            Check(Services, HandlersRecipe, HandlersModel(), Handlers).ShouldBeClean();
        }

        [Fact]
        public void The_handler_intent_recipes_validate_and_read_every_value_from_the_code()
        {
            var validation = ShouldValidate(Services, HandlersRecipe, HandlersModel(), Handlers);

            validation.Sources(FactKinds.IntentCreates).ShouldBe((3, 0, 0));
            validation.Sources(FactKinds.IntentUpdates).ShouldBe((4, 0, 0));
            validation.Sources(FactKinds.IntentDeletes).ShouldBe((3, 0, 0));
            validation.Sources(FactKinds.IntentQueries).ShouldBe((3, 0, 0));
            validation.Sources(FactKinds.IntentInvokes).ShouldBe((4, 0, 0));
            validation.Sources(FactKinds.IntentPublishes).ShouldBe((3, 0, 0));
            validation.Sources(FactKinds.IntentSends).ShouldBe((2, 0, 0));
        }

        [Theory]
        [InlineData("src/Ordering.Application/Services/OrderService.cs", "_eventBus.Publish(new OrderApprovedEvent { OrderId = orderId });", "",
            "`OrderService.Approve` publishes: model `OrderApprovedEvent`, code (none)")]
        [InlineData("src/Ordering.Application/Orders/DeleteOrderCommandHandler.cs", "_orderRepository.Remove(order);", "",
            "`DeleteOrderCommand` deletes: model `Order`, code (none)")]
        [InlineData("src/Ordering.Application/Services/OrderService.cs", "_paymentsClient.ChargeAsync(", "_paymentsClient.AuthorizeAsync(",
            "`OrderService.Approve` invokes: model `ChargeAsync`, code (none)")]
        [InlineData("src/Ordering.Application/Orders/CancelOrderCommandHandler.cs", "orderToUpdate.SetCancelledStatus();", "",
            "`CancelOrderCommand` updates: model `Order`, code (none)")]
        [InlineData("src/Ordering.Application/Orders/CancelOrderCommandHandler.cs", "await endpoint.Send(new CancelPaymentCommand(command.OrderNumber), cancellationToken);", "",
            "`CancelOrderCommand` sends: model `CancelPaymentCommand`, code (none)")]
        [InlineData("src/Ordering.Application/Orders/CreateOrderCommandHandler.cs", "order.AddOrderItem(item.ProductId, item.Units);", "",
            "`CreateOrderCommand` creates: model `OrderItem`, code (none)")]
        [InlineData("src/Ordering.Application/Orders/GetOrdersQueryHandler.cs", "_orderRepository.FindAllAsync(", "_cache.GetOrders(",
            "`GetOrdersQuery` queries: model `Order`, code (none)")]
        [InlineData("src/Ordering.Application/Services/OrderService.cs", "await _orderRepository.ArchiveOrders(orderId, reason);", "",
            "`OrderService.Archive` invokes: model `ArchiveOrders`, code (none)")]
        [InlineData("src/Ordering.Application/IntegrationEvents/StockConfirmedConsumer.cs", "await context.Publish<OrderStockConfirmedIntegrationEvent>(new { context.Message.OrderId });", "",
            "`StockConfirmedConsumer(StockConfirmedIntegrationEvent)` publishes: model `OrderStockConfirmedIntegrationEvent`, code (none)")]
        [InlineData("src/Ordering.Application/DomainEventHandlers/OrderStartedDomainEventHandler.cs", "buyer = Buyer.Create(", "buyer = Buyer.Register(",
            "`OrderStartedDomainEventHandler(OrderStartedDomainEvent)` creates: model `Buyer`, code (none)")]
        public void A_handler_change_is_reported_as_its_one_row(string path, string from, string to, string row)
        {
            Check(Services, HandlersRecipe, HandlersModel(), Mutate(Handlers, path, from, to)).ShouldReport(row);
        }

        [Fact]
        public void A_missing_interaction_says_what_the_handler_lacks()
        {
            var files = Mutate(Handlers, "src/Ordering.Application/Orders/CreateOrderCommandHandler.cs", "var orderStarted = new OrderStartedIntegrationEvent(message.UserId);", "");

            Check(Services, HandlersRecipe, HandlersModel(), files).WrongLines.ShouldHaveSingleItem()
                .ShouldStartWith("`CreateOrderCommand` publishes: model `OrderStartedIntegrationEvent`, code (none) (no `new OrderStartedIntegrationEvent` or `Publish<OrderStartedIntegrationEvent>` in the handler)");
        }

        [Fact]
        public void Two_creates_on_one_handler_each_read_their_own_entity()
        {
            var validation = RecipeValidation.Run(new RecipeValidationInput
            {
                Designer = Services,
                RecipePath = "guardrails/services.drift.json",
                RecipeJson = HandlersRecipe,
                Code = new CodeFiles(Handlers),
                Facts = HandlersModel(),
                Filter = RecipeFilter.Of(FactKinds.IntentCreates.Key, "CreateOrderCommand", null)
            });

            validation.Readings.Select(x => (x.Model, x.Code, x.Matched)).ShouldBe(new[]
            {
                ("Order", "Order", "new Order"),
                ("OrderItem", "OrderItem", ".AddOrderItem(")
            });
        }

        /// <summary>A Command's or Query's fact, its handler found through its request type.</summary>
        private static ModelFact Request(FactKind kind, string request, string value, params (string Name, string Value)[] target) =>
            Intent(kind, request, value, "", request, null, target);

        private static ModelFact ServiceOperation(FactKind kind, string service, string operation, string[] parameterNames, string value, params (string Name, string Value)[] target) =>
            Intent(kind, $"{service}.{operation}", value, service, operation, parameterNames, target);

        /// <summary>An event handler subscription's fact: the handler as the Service, the message it handles as the operation.</summary>
        private static ModelFact Subscription(FactKind kind, string handler, string message, string value, params (string Name, string Value)[] target) =>
            Intent(kind, $"{handler}({message})", value, handler, message, null, target);

        private static ModelFact Intent(FactKind kind, string element, string value, string service, string operation, IReadOnlyList<string> parameterNames, (string Name, string Value)[] target)
        {
            var placeholders = new Dictionary<string, string> { ["Service"] = service, ["Operation"] = operation, ["Folder"] = "" };
            foreach (var (name, placeholder) in target)
            {
                placeholders[name] = placeholder;
            }

            return new ModelFact { Kind = kind, Element = element, Value = value, Placeholders = placeholders, ParameterNames = parameterNames };
        }
    }
}
