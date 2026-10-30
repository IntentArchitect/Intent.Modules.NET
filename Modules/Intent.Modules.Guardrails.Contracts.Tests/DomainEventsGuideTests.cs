using System.Collections.Generic;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;
using static Intent.Modules.Guardrails.Contracts.Tests.Guides;
using DomainGuides = Intent.Modules.Domain.Guardrails.Contracts.Templates.DriftRecipeGuides;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The domain events guide's recipe, run against entities raising events in each style it covers and a model whose
    /// Domain Event Origins agree with them: no drift, no warnings from validate-drift-recipes, every value read from the
    /// code, and each planted change reported as exactly its row.
    /// </summary>
    public class DomainEventsGuideTests
    {
        private const string Domain = FactKinds.Domain;

        private static readonly Dictionary<string, string> Entities = new()
        {
            // eShop's Order: a base entity's AddDomainEvent, and a private helper named after the event, called from the
            // constructor that a parameterless one overloads.
            ["src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs"] = """
                namespace Ordering.Domain.AggregatesModel.OrderAggregate;

                public class Order : Entity, IAggregateRoot
                {
                    private readonly List<OrderItem> _orderItems = new();

                    protected Order()
                    {
                    }

                    public Order(string userId, string userName, Address address) : this()
                    {
                        Address = address;
                        OrderStatus = OrderStatus.Submitted;
                        AddOrderStartedDomainEvent(userId, userName);
                    }

                    public Address Address { get; private set; }

                    public OrderStatus OrderStatus { get; private set; }

                    public void SetAwaitingValidationStatus()
                    {
                        if (OrderStatus == OrderStatus.Submitted)
                        {
                            AddDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(Id, _orderItems));
                            OrderStatus = OrderStatus.AwaitingValidation;
                        }
                    }

                    public void SetCancelledStatus()
                    {
                        OrderStatus = OrderStatus.Cancelled;
                        AddDomainEvent(new OrderCancelledDomainEvent(this));
                    }

                    private void AddOrderStartedDomainEvent(string userId, string userName)
                    {
                        var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName);
                        this.AddDomainEvent(orderStartedDomainEvent);
                    }
                }
                """,
            // Intent's own entities: a DomainEvents list.
            ["src/Ordering.Domain/AggregatesModel/BuyerAggregate/Buyer.cs"] = """
                namespace Ordering.Domain.AggregatesModel.BuyerAggregate;

                public class Buyer : IHasDomainEvent
                {
                    public Buyer(string identityGuid, string name)
                    {
                        IdentityGuid = identityGuid;
                        Name = name;
                        DomainEvents.Add(new BuyerCreatedDomainEvent(this));
                    }

                    public string IdentityGuid { get; private set; }

                    public string Name { get; private set; }

                    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

                    public void VerifyOrAddPaymentMethod(int cardTypeId, string alias, int orderId)
                    {
                        Alias = alias;
                        DomainEvents.Add(new BuyerAndPaymentMethodVerifiedDomainEvent(this, cardTypeId, orderId));
                    }
                }
                """,
            // A RaiseDomainEvent method, the entity's own list, and an overload that raises nothing.
            ["src/Ordering.Domain/AggregatesModel/InvoiceAggregate/Invoice.cs"] = """
                namespace Ordering.Domain.AggregatesModel.InvoiceAggregate;

                public class Invoice
                {
                    private readonly List<object> _domainEvents = new();

                    public int Id { get; private set; }

                    public decimal Paid { get; private set; }

                    public void Issue()
                    {
                        Status = InvoiceStatus.Issued;
                        RaiseDomainEvent(new InvoiceIssuedDomainEvent(Id));
                    }

                    public void MarkPaid(decimal amount)
                    {
                        Paid = Paid + amount;
                    }

                    public void MarkPaid(decimal amount, string reference)
                    {
                        Paid += amount;
                        _domainEvents.Add(new InvoicePaidDomainEvent(Id, reference));
                    }

                    private void RaiseDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);
                }
                """
        };

        private static List<ModelFact> EntitiesModel() => new()
        {
            Raises("Order", "Order", "OrderStartedDomainEvent", "userId", "userName", "address"),
            Raises("Order", "SetAwaitingValidationStatus", "OrderStatusChangedToAwaitingValidationDomainEvent"),
            Raises("Order", "SetCancelledStatus", "OrderCancelledDomainEvent"),
            Raises("Buyer", "Buyer", "BuyerCreatedDomainEvent", "identityGuid", "name"),
            Raises("Buyer", "VerifyOrAddPaymentMethod", "BuyerAndPaymentMethodVerifiedDomainEvent", "cardTypeId", "alias", "orderId"),
            Raises("Invoice", "Issue", "InvoiceIssuedDomainEvent"),
            Raises("Invoice", "MarkPaid", "InvoicePaidDomainEvent", "amount", "reference")
        };

        private static string EntitiesRecipe => RecipeFile(DomainGuides.DomainEvents);

        [Fact]
        public void The_domain_events_recipe_reads_every_style_with_no_drift()
        {
            Check(Domain, EntitiesRecipe, EntitiesModel(), Entities).ShouldBeClean();
        }

        [Fact]
        public void The_domain_events_recipe_validates_and_reads_every_value_from_the_code()
        {
            var validation = ShouldValidate(Domain, EntitiesRecipe, EntitiesModel(), Entities);

            validation.Sources(FactKinds.IntentRaises).ShouldBe((7, 0, 0));
        }

        [Theory]
        [InlineData("src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs", "AddOrderStartedDomainEvent(userId, userName);", "",
            "`Order.Order` raises: model `OrderStartedDomainEvent`, code (none)")]
        [InlineData("src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs", "AddDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(Id, _orderItems));", "",
            "`Order.SetAwaitingValidationStatus` raises: model `OrderStatusChangedToAwaitingValidationDomainEvent`, code (none)")]
        [InlineData("src/Ordering.Domain/AggregatesModel/BuyerAggregate/Buyer.cs", "new BuyerAndPaymentMethodVerifiedDomainEvent(", "new BuyerVerifiedDomainEvent(",
            "`Buyer.VerifyOrAddPaymentMethod` raises: model `BuyerAndPaymentMethodVerifiedDomainEvent`, code (none)")]
        [InlineData("src/Ordering.Domain/AggregatesModel/BuyerAggregate/Buyer.cs", "DomainEvents.Add(new BuyerCreatedDomainEvent(this));", "",
            "`Buyer.Buyer` raises: model `BuyerCreatedDomainEvent`, code (none)")]
        [InlineData("src/Ordering.Domain/AggregatesModel/InvoiceAggregate/Invoice.cs", "RaiseDomainEvent(new InvoiceIssuedDomainEvent(Id));", "",
            "`Invoice.Issue` raises: model `InvoiceIssuedDomainEvent`, code (none)")]
        public void A_domain_event_no_longer_raised_is_reported_as_its_one_row(string path, string from, string to, string row)
        {
            Check(Domain, EntitiesRecipe, EntitiesModel(), Mutate(Entities, path, from, to)).ShouldReport(row);
        }

        [Fact]
        public void The_overload_the_model_links_is_the_one_read()
        {
            // The event moves from the overload the model links to the one it doesn't.
            const string path = "src/Ordering.Domain/AggregatesModel/InvoiceAggregate/Invoice.cs";
            var moved = Mutate(Entities, path, "_domainEvents.Add(new InvoicePaidDomainEvent(Id, reference));", "");
            var files = Mutate(moved, path, "Paid = Paid + amount;", "Paid = Paid + amount; _domainEvents.Add(new InvoicePaidDomainEvent(Id, null));");

            Check(Domain, EntitiesRecipe, EntitiesModel(), files).WrongLines.ShouldHaveSingleItem()
                .ShouldStartWith("`Invoice.MarkPaid` raises: model `InvoicePaidDomainEvent`, code (none) (no `new InvoicePaidDomainEvent` or `AddInvoicePaidDomainEvent(…)` in `MarkPaid`)");
        }

        /// <summary>
        /// eShop's <c>Buyer.VerifyOrAddPaymentMethod</c> raises the same event in both branches. The probe changes the first
        /// and the recipe reads the second: that is the event stated twice, not a case misreading the code.
        /// </summary>
        [Fact]
        public void An_event_raised_in_two_branches_passes_the_probe()
        {
            var files = new Dictionary<string, string>
            {
                ["src/Ordering.Domain/AggregatesModel/BuyerAggregate/Buyer.cs"] = """
                    namespace Ordering.Domain.AggregatesModel.BuyerAggregate;

                    public class Buyer : Entity, IAggregateRoot
                    {
                        public PaymentMethod VerifyOrAddPaymentMethod(int cardTypeId, string alias, int orderId)
                        {
                            var existingPayment = _paymentMethods.SingleOrDefault(p => p.IsEqualTo(cardTypeId));
                            if (existingPayment != null)
                            {
                                AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));
                                return existingPayment;
                            }

                            var payment = new PaymentMethod(cardTypeId, alias);
                            _paymentMethods.Add(payment);
                            AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));
                            return payment;
                        }
                    }
                    """
            };
            var model = new List<ModelFact> { Raises("Buyer", "VerifyOrAddPaymentMethod", "BuyerAndPaymentMethodVerifiedDomainEvent", "cardTypeId", "alias", "orderId") };

            ShouldValidate(Domain, EntitiesRecipe, model, files).Sources(FactKinds.IntentRaises).ShouldBe((1, 0, 0));
            Check(Domain, EntitiesRecipe, model, files).ShouldBeClean();
        }

        private static ModelFact Raises(string entity, string member, string domainEvent, params string[] parameterNames)
        {
            return new ModelFact
            {
                Kind = FactKinds.IntentRaises,
                Element = $"{entity}.{member}",
                Value = domainEvent,
                Placeholders = new Dictionary<string, string> { ["Entity"] = entity, ["Member"] = member, ["DomainEvent"] = domainEvent },
                ParameterNames = parameterNames
            };
        }
    }
}
