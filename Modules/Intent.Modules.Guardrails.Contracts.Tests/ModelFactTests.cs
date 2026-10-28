using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using NSubstitute;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Fakes;
using DomainDrift = Intent.Modules.Domain.Guardrails.Contracts.Drift;
using DomainEventsMetadata = Intent.Modules.Domain.Guardrails.Contracts.Templates.DomainEventsMetadata;
using OptionalModelerMetadata = Intent.Modules.Services.Guardrails.Contracts.Templates.OptionalModelerMetadata;
using ServicesDrift = Intent.Modules.Services.Guardrails.Contracts.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>The facts each module reads from its designer's metadata, in the model's own vocabulary.</summary>
    public class ModelFactTests
    {
        [Fact]
        public void Services_endpoints_state_their_HTTP_facts_and_inherited_security()
        {
            var package = Package(ApplicationId, Stereotype("Secured"));
            var folder = Element(package, "Folder", "Orders");
            var service = Element(folder, "Service", "OrdersController", Stereotype("Http Service Settings", ("Route", "api/orders")), Stereotype("Secured", ("Roles", "Admin, Manager")));
            var get = Element(service, "Operation", "GetOrder", Stereotype("Http Settings", ("Verb", "GET"), ("Route", "{id}")));
            Element(get, "Parameter", "id", Stereotype("Parameter Settings", ("Source", "From Route")));
            Element(get, "Parameter", "expand");
            var create = Element(service, "Operation", "Create", Stereotype("Http Settings", ("Verb", "POST"), ("Route", "")), Stereotype("Unsecured"));
            Element(create, "Parameter", "dto", Stereotype("Parameter Settings", ("Source", "")));
            Element(service, "Operation", "Helper");
            var command = Element(folder, "Command", "ArchiveOrderCommand", Stereotype("Http Settings", ("Verb", "DELETE"), ("Route", "api/orders/{id}")));
            Element(command, "DTO-Field", "Id", Stereotype("Parameter Settings", ("Source", "From Route")));

            var facts = Facts(ServicesDrift.DriftModel.Read(Designer(new[] { package, Package("other-app") }), ApplicationId)
                .Select(x => (x.Kind.Key, x.Element, x.Value)));

            facts.ShouldBe(new[]
            {
                "operation.route ArchiveOrderCommand = api/orders/{id}",
                "operation.route OrdersController.Create = ",
                "operation.route OrdersController.GetOrder = {id}",
                "operation.verb ArchiveOrderCommand = DELETE",
                "operation.verb OrdersController.Create = POST",
                "operation.verb OrdersController.GetOrder = GET",
                "parameter.source ArchiveOrderCommand(Id) = From Route",
                "parameter.source OrdersController.Create(dto) = Default",
                "parameter.source OrdersController.GetOrder(id) = From Route",
                "security ArchiveOrderCommand = Secured",
                "security OrdersController.Create = Unsecured",
                "security OrdersController.GetOrder = Secured",
                "security.roles OrdersController.GetOrder = Admin,Manager",
                "service.route OrdersController = api/orders"
            });
        }

        [Fact]
        public void Services_placeholders_and_parameter_names_identify_the_code()
        {
            var package = Package();
            var folder = Element(package, "Folder", "Orders");
            var service = Element(folder, "Service", "OrdersController");
            var get = Element(service, "Operation", "GetOrder", Stereotype("Http Settings", ("Verb", "GET")));
            Element(get, "Parameter", "id");
            Element(get, "Parameter", "expand");
            Element(folder, "Query", "GetOrdersQuery", Stereotype("Http Settings", ("Verb", "GET")));

            var facts = ServicesDrift.DriftModel.Read(Designer(new[] { package }), ApplicationId);

            var operation = facts.First(x => x.Element == "OrdersController.GetOrder");
            operation.Placeholders.ShouldBe(new Dictionary<string, string> { ["Service"] = "OrdersController", ["Operation"] = "GetOrder", ["Folder"] = "Orders" });
            operation.ParameterNames.ShouldBe(new[] { "id", "expand" });
            var query = facts.First(x => x.Element == "GetOrdersQuery");
            query.Placeholders.ShouldBe(new Dictionary<string, string> { ["Service"] = "", ["Operation"] = "GetOrdersQuery", ["Folder"] = "Orders" });
            query.ParameterNames.ShouldBeNull();
        }

        [Fact]
        public void Services_security_is_stated_only_once_the_designer_uses_it()
        {
            var package = Package();
            var service = Element(package, "Service", "OrdersController");
            Element(service, "Operation", "GetOrder", Stereotype("Http Settings", ("Verb", "GET"), ("Route", "")));

            ServicesDrift.DriftModel.Read(Designer(new[] { package }), ApplicationId).ShouldNotContain(x => x.Kind.Key == "security");

            Element(service, "Operation", "Ping", Stereotype("Http Settings", ("Verb", "GET"), ("Route", "ping")), Stereotype("Unsecured"));
            var facts = ServicesDrift.DriftModel.Read(Designer(new[] { package }), ApplicationId);
            facts.Single(x => x.Kind.Key == "security" && x.Element == "OrdersController.GetOrder").Value.ShouldBe("None");
            facts.Single(x => x.Kind.Key == "security" && x.Element == "OrdersController.Ping").Value.ShouldBe("Unsecured");
        }

        [Fact]
        public void Domain_persisted_classes_state_their_database_facts()
        {
            var package = Package(ApplicationId, Stereotype("Relational Database"), Stereotype("Schema", ("Name", "dbo")));
            var folder = Element(package, "Folder", "Sales", Stereotype("Schema", ("Name", "sales")));
            var customer = Element(folder, "Class", "Customer", Stereotype("Table", ("Name", "Customers"), ("Schema", "")));
            Element(customer, "Attribute", "CustomerId", Stereotype("Primary Key")).Typed("int");
            Element(customer, "Attribute", "Name", Stereotype("Text Constraints", ("SQL Data Type", "VARCHAR"), ("MaxLength", "100"))).Typed("string");
            Element(customer, "Attribute", "Nickname").Typed("string", nullable: true);
            Element(customer, "Attribute", "CreditLimit", Stereotype("Decimal Constraints", ("Precision", "18"), ("Scale", "2"))).Typed("decimal");
            Element(customer, "Attribute", "CreatedOn", Stereotype("Column", ("Name", "created_on"), ("Type", "datetime")), Stereotype("Default Constraint", ("Value", "(getdate())"))).Typed("datetime");
            var helper = Element(package, "Class", "PriceCalculator");
            Element(helper, "Attribute", "Rate").Typed("decimal");

            var facts = Facts(DomainDrift.DriftModel.Read(Designer(new[] { package }), ApplicationId).Select(x => (x.Kind.Key, x.Element, x.Value)));

            facts.ShouldBe(new[]
            {
                "column.default Customer.CreatedOn = (getdate())",
                "column.default Customer.CreditLimit = ",
                "column.default Customer.CustomerId = ",
                "column.default Customer.Name = ",
                "column.default Customer.Nickname = ",
                "column.maxLength Customer.Name = 100",
                "column.maxLength Customer.Nickname = ",
                "column.name Customer.CreatedOn = created_on",
                "column.name Customer.CreditLimit = ",
                "column.name Customer.CustomerId = ",
                "column.name Customer.Name = ",
                "column.name Customer.Nickname = ",
                "column.precision Customer.CreditLimit = 18",
                "column.required Customer.Name = true",
                "column.required Customer.Nickname = false",
                "column.scale Customer.CreditLimit = 2",
                "column.textType Customer.Name = VARCHAR",
                "column.textType Customer.Nickname = DEFAULT",
                "column.type Customer.CreatedOn = datetime",
                "column.type Customer.CreditLimit = ",
                "column.type Customer.CustomerId = ",
                "column.type Customer.Name = ",
                "column.type Customer.Nickname = ",
                "key Customer = CustomerId",
                "table.kind Customer = Table",
                "table.name Customer = Customers",
                "table.schema Customer = sales"
            });
        }

        [Fact]
        public void Domain_relationships_state_multiplicity_and_foreign_keys_only_where_the_database_has_them()
        {
            var package = Package();
            var customer = Element(package, "Class", "Customer", Stereotype("Table", ("Name", "Customers")));
            var order = Element(package, "Class", "Order", Stereotype("Table", ("Name", "Orders")));
            var association = Association("assoc-1", order, "Orders", sourceCollection: true, customer, "Customer", targetNullable: false);
            Element(order, "Attribute", "CustomerId", Stereotype("Foreign Key", ("Association", "assoc-1:target"))).Typed("int");

            // Owned: a composition to a class with no table of its own.
            var owned = Association("assoc-2", order, "Order", sourceCollection: false, Element(package, "Class", "Address"), "ShippingAddress", targetNullable: false);

            // A class nothing maps (Basislink's scaffolded contracts, with no DbSet or Entity<T>) has no relationship in the
            // database, even to a mapped class; nor does a mapped class's reference to an unmapped class it doesn't own.
            var unmapped = Association("assoc-3", Element(package, "Class", "contracts"), "contracts", sourceCollection: true, customer, "membernoNavigation", targetNullable: true);
            var reference = Association("assoc-4", order, "Orders", sourceCollection: true, Element(package, "Class", "Draft"), "Draft", targetNullable: true);
            var unpersisted = Association("assoc-5", Element(package, "Class", "Note"), "Notes", sourceCollection: true, Element(package, "Class", "Memo"), "Memo", targetNullable: true);

            var facts = Facts(DomainDrift.DriftModel.Read(Designer(new[] { package }, new[] { association, owned, unmapped, reference, unpersisted }), ApplicationId)
                .Where(x => x.Kind.Key.StartsWith("relationship"))
                .Select(x => (x.Kind.Key, x.Element, x.Value)));

            facts.ShouldBe(new[]
            {
                "relationship.foreignKey Order.Customer = CustomerId",
                "relationship.foreignKey Order.ShippingAddress = ",
                "relationship.multiplicity Order.Customer = *->1",
                "relationship.multiplicity Order.ShippingAddress = 1->1"
            });
        }

        [Fact]
        public void Domain_named_indexes_state_their_columns_and_uniqueness()
        {
            var package = Package();
            var order = Element(package, "Class", "Order", Stereotype("Table", ("Name", "Orders")));
            Element(order, "Attribute", "PlacedOn", Stereotype("Index", ("UniqueKey", "IX_Order_Customer_Date"), ("Order", "2"), ("IsUnique", "true"))).Typed("datetime");
            var number = Element(order, "Attribute", "Number", Stereotype("Index", ("UniqueKey", ""), ("IsUnique", "true"))).Typed("string");
            Element(order, "Attribute", "CustomerId", Stereotype("Index", ("UniqueKey", "IX_Order_Customer_Date"), ("Order", "1"), ("IsUnique", "true"))).Typed("int");
            Element(order, "Attribute", "Total").Typed("decimal");

            // Index elements: one named, with a key column mapped to its attribute and an included column; one using the default name.
            var named = Element(order, "Index", "IX_Order_Number", Stereotype("Settings", ("Unique", "true"), ("Use Default Name", "false")));
            var key = Element(named, "Index Column", "Number", Stereotype("Settings", ("Type", "Key")));
            var mapping = Substitute.For<IElementMapping>();
            mapping.Element.Returns(number);
            key.MappedElement.Returns(mapping);
            Element(named, "Index Column", "Total", Stereotype("Settings", ("Type", "Included")));
            var unnamed = Element(order, "Index", "IX_Order_Total", Stereotype("Settings", ("Unique", "false"), ("Use Default Name", "true")));
            Element(unnamed, "Index Column", "Total", Stereotype("Settings", ("Type", "Key")));

            var facts = DomainDrift.DriftModel.Read(Designer(new[] { package }), ApplicationId).Where(x => x.Kind.Key.StartsWith("index.")).ToList();

            Facts(facts.Select(x => (x.Kind.Key, x.Element, x.Value))).ShouldBe(new[]
            {
                "index.columns Order.IX_Order_Customer_Date = CustomerId,PlacedOn",
                "index.columns Order.IX_Order_Number = Number",
                "index.unique Order.IX_Order_Customer_Date = true",
                "index.unique Order.IX_Order_Number = true"
            });
            facts[0].Placeholders.ShouldBe(new Dictionary<string, string> { ["Entity"] = "Order", ["Index"] = "IX_Order_Customer_Date" });
        }

        /// <summary>
        /// Basislink's <c>basislinkLinkedMember.Username</c> is in two indexes, with two <c>Index</c> stereotypes. Reading
        /// only the first left it out of the composite index, and the check contradicted a correct model.
        /// </summary>
        [Fact]
        public void Domain_an_attribute_in_several_indexes_is_in_each_of_them()
        {
            var package = Package();
            var members = Element(package, "Class", "basislinkLinkedMember", Stereotype("Table", ("Name", "basislinkLinkedMembers")));
            Element(members, "Attribute", "Username",
                Stereotype("Index", ("UniqueKey", "IX_basislinkLinkedMembers_Username"), ("Order", ""), ("IsUnique", "false")),
                Stereotype("Index", ("UniqueKey", "IX_basislinkLinkedMembers_UsernameClientNumber"), ("Order", "1"), ("IsUnique", "false"))).Typed("string");
            Element(members, "Attribute", "ClientNumber",
                Stereotype("Index", ("UniqueKey", "IX_basislinkLinkedMembers_UsernameClientNumber"), ("Order", "2"), ("IsUnique", "false"))).Typed("string");

            var facts = Facts(DomainDrift.DriftModel.Read(Designer(new[] { package }), ApplicationId)
                .Where(x => x.Kind.Key.StartsWith("index."))
                .Select(x => (x.Kind.Key, x.Element, x.Value)));

            facts.ShouldBe(new[]
            {
                "index.columns basislinkLinkedMember.IX_basislinkLinkedMembers_Username = Username",
                "index.columns basislinkLinkedMember.IX_basislinkLinkedMembers_UsernameClientNumber = Username,ClientNumber",
                "index.unique basislinkLinkedMember.IX_basislinkLinkedMembers_Username = false",
                "index.unique basislinkLinkedMember.IX_basislinkLinkedMembers_UsernameClientNumber = false"
            });
        }

        [Fact]
        public void Services_each_interaction_association_states_an_intent_fact_about_its_target()
        {
            var package = Package();
            var domain = Package("domain-app");
            var order = Element(domain, "Class", "Order");
            var orderConstructor = Element(order, "Class Constructor", "Order");
            var cancel = Element(order, "Operation", "Cancel");
            var buyer = Element(domain, "Class", "Buyer");
            var factory = Element(buyer, "Operation", "Create");
            var procedure = Element(Element(domain, "Repository", "OrderRepository"), "Stored Procedure", "ArchiveOrders");
            var folder = Element(package, "Folder", "Orders");
            var approve = Element(Element(folder, "Service", "OrderService"), "Operation", "Approve");
            var charge = Element(Element(package, "Service Proxy", "PaymentsClient"), "Operation", "ChargeAsync");
            var archive = Element(folder, "Command", "ArchiveOrderCommand");
            var started = Element(package, "Message", "OrderStarted");
            var cancelPayment = Element(package, "Integration Command", "CancelPayment");

            var command = Element(folder, "Command", "CreateOrderCommand");
            Interaction(command, OptionalModelerMetadata.CreateEntityActionTypeId, order);
            Interaction(command, OptionalModelerMetadata.CreateEntityActionTypeId, orderConstructor);
            Interaction(command, OptionalModelerMetadata.CreateEntityActionTypeId, factory);
            Interaction(command, OptionalModelerMetadata.UpdateEntityActionTypeId, cancel);
            Interaction(command, OptionalModelerMetadata.UpdateEntityActionTypeId, buyer);
            Interaction(command, OptionalModelerMetadata.DeleteEntityActionTypeId, order);
            Interaction(command, OptionalModelerMetadata.QueryEntityActionTypeId, buyer);
            Interaction(command, OptionalModelerMetadata.PerformInvocationTypeId, approve);
            Interaction(command, OptionalModelerMetadata.PerformInvocationTypeId, charge);
            Interaction(command, OptionalModelerMetadata.PerformInvocationTypeId, procedure);
            Interaction(command, OptionalModelerMetadata.PerformInvocationTypeId, archive);
            Interaction(command, OptionalModelerMetadata.PublishIntegrationEventTypeId, started);
            Interaction(command, OptionalModelerMetadata.SendIntegrationCommandTypeId, cancelPayment);

            var facts = ServicesDrift.DriftModel.Read(Designer(new[] { package, domain }), ApplicationId)
                .Where(x => x.Kind.Key.StartsWith("intent."))
                .Select(x => $"{x.Kind.Key} {x.Element} = {x.Value} | {Targets(x.Placeholders)}")
                .OrderBy(x => x, System.StringComparer.Ordinal);

            facts.ShouldBe(new[]
            {
                "intent.creates CreateOrderCommand = Buyer | Entity=Buyer, Member=Create",
                "intent.creates CreateOrderCommand = Order | Entity=Order, Member=",
                "intent.creates CreateOrderCommand = Order | Entity=Order, Member=Order",
                "intent.deletes CreateOrderCommand = Order | Entity=Order",
                "intent.invokes CreateOrderCommand = Approve | Invoked=Approve, InvokedOwner=OrderService",
                "intent.invokes CreateOrderCommand = ArchiveOrderCommand | Invoked=ArchiveOrderCommand, InvokedOwner=",
                "intent.invokes CreateOrderCommand = ArchiveOrders | Invoked=ArchiveOrders, InvokedOwner=",
                "intent.invokes CreateOrderCommand = ChargeAsync | Invoked=ChargeAsync, InvokedOwner=PaymentsClient",
                "intent.publishes CreateOrderCommand = OrderStarted | Message=OrderStarted",
                "intent.queries CreateOrderCommand = Buyer | Entity=Buyer",
                "intent.sends CreateOrderCommand = CancelPayment | Message=CancelPayment",
                "intent.updates CreateOrderCommand = Buyer | Entity=Buyer, Member=",
                "intent.updates CreateOrderCommand = Order | Entity=Order, Member=Cancel"
            });
        }

        [Fact]
        public void Services_intent_facts_are_stated_for_every_kind_of_handler()
        {
            var package = Package();
            var domain = Package("domain-app");
            var order = Element(domain, "Class", "Order");
            var folder = Element(package, "Folder", "Ordering");
            var service = Element(folder, "Service", "OrderService");
            var approve = Element(service, "Operation", "Approve");
            Element(approve, "Parameter", "orderId");
            Element(approve, "Parameter", "reason");
            var query = Element(folder, "Query", "GetOrderQuery");
            var started = Element(package, "Message", "OrderStarted");
            var cancelPayment = Element(package, "Integration Command", "CancelPayment");
            var placed = Element(domain, "Domain Event", "OrderPlacedDomainEvent");
            var integrationHandler = Element(folder, "Integration Event Handler", "OrderStartedHandler");
            var domainHandler = Element(package, "Domain Event Handler", "OrderPlacedHandler");

            Interaction(approve, OptionalModelerMetadata.UpdateEntityActionTypeId, order);
            Interaction(query, OptionalModelerMetadata.QueryEntityActionTypeId, order);
            var onStarted = Interaction(integrationHandler, OptionalModelerMetadata.SubscribeIntegrationEventTypeId, started);
            Interaction(onStarted, OptionalModelerMetadata.CreateEntityActionTypeId, order);
            var onCancelPayment = Interaction(integrationHandler, OptionalModelerMetadata.SubscribeIntegrationCommandTypeId, cancelPayment);
            Interaction(onCancelPayment, OptionalModelerMetadata.DeleteEntityActionTypeId, order);
            var onPlaced = Interaction(domainHandler, OptionalModelerMetadata.DomainEventHandlerAssociationTypeId, placed);
            Interaction(onPlaced, OptionalModelerMetadata.PublishIntegrationEventTypeId, started);

            var facts = ServicesDrift.DriftModel.Read(Designer(new[] { package, domain }), ApplicationId)
                .Where(x => x.Kind.Key.StartsWith("intent."))
                .ToDictionary(x => $"{x.Kind.Key} {x.Element}");

            facts.Keys.OrderBy(x => x, System.StringComparer.Ordinal).ShouldBe(new[]
            {
                "intent.creates OrderStartedHandler(OrderStarted)",
                "intent.deletes OrderStartedHandler(CancelPayment)",
                "intent.publishes OrderPlacedHandler(OrderPlacedDomainEvent)",
                "intent.queries GetOrderQuery",
                "intent.updates OrderService.Approve"
            });

            // A Service operation: as an endpoint, with its parameter names for overloads.
            var operation = facts["intent.updates OrderService.Approve"];
            operation.Placeholders.ShouldBe(new Dictionary<string, string> { ["Service"] = "OrderService", ["Operation"] = "Approve", ["Folder"] = "Ordering", ["Entity"] = "Order", ["Member"] = "" });
            operation.ParameterNames.ShouldBe(new[] { "orderId", "reason" });
            operation.ElementIds.ShouldBe(new[] { service.Id, approve.Id });

            // A Query: no Service.
            var request = facts["intent.queries GetOrderQuery"];
            request.Placeholders.ShouldBe(new Dictionary<string, string> { ["Service"] = "", ["Operation"] = "GetOrderQuery", ["Folder"] = "Ordering", ["Entity"] = "Order" });
            request.ParameterNames.ShouldBeNull();
            request.ElementIds.ShouldBe(new[] { query.Id });

            // A subscription: the handler as the Service, the message it handles as the operation, the handler's element.
            var subscription = facts["intent.creates OrderStartedHandler(OrderStarted)"];
            subscription.Placeholders.ShouldBe(new Dictionary<string, string> { ["Service"] = "OrderStartedHandler", ["Operation"] = "OrderStarted", ["Folder"] = "Ordering", ["Entity"] = "Order", ["Member"] = "" });
            subscription.ElementIds.ShouldBe(new[] { integrationHandler.Id });
            facts["intent.publishes OrderPlacedHandler(OrderPlacedDomainEvent)"].Placeholders.ShouldBe(new Dictionary<string, string>
            {
                ["Service"] = "OrderPlacedHandler", ["Operation"] = "OrderPlacedDomainEvent", ["Folder"] = "", ["Message"] = "OrderStarted"
            });
        }

        [Fact]
        public void Services_intent_facts_come_only_from_this_applications_handlers_and_the_interaction_associations()
        {
            var package = Package();
            var other = Package("other-app");
            var order = Element(other, "Class", "Order");
            var theirs = Element(other, "Command", "TheirCommand");
            Interaction(theirs, OptionalModelerMetadata.CreateEntityActionTypeId, order);
            var ours = Element(package, "Command", "OurCommand");
            Interaction(ours, "an-association-the-check-doesn't-read", order);

            // Intent.Modelers.Services.DomainInteractions' earlier name for Perform Invocation, with the same type id.
            Interaction(ours, OptionalModelerMetadata.PerformInvocationTypeId, Element(other, "Query", "GetOrderQuery"), specializationType: "Call Service Operation");

            var facts = Facts(ServicesDrift.DriftModel.Read(Designer(new[] { package, other }), ApplicationId).Select(x => (x.Kind.Key, x.Element, x.Value)));

            facts.ShouldBe(new[] { "intent.invokes OurCommand = GetOrderQuery" });
        }

        [Fact]
        public void Domain_constructors_and_operations_state_the_domain_events_they_raise()
        {
            var package = Package();
            var order = Element(package, "Class", "Order");
            var constructor = Element(order, "Class Constructor", "Order");
            Element(constructor, "Parameter", "userId");
            Element(constructor, "Parameter", "userName");
            var cancel = Element(order, "Operation", "SetCancelledStatus");
            var started = Element(package, "Domain Event", "OrderStartedDomainEvent");
            Interaction(constructor, DomainEventsMetadata.OriginAssociationTypeId, started);
            Interaction(cancel, DomainEventsMetadata.OriginAssociationTypeId, Element(package, "Domain Event", "OrderCancelledDomainEvent"));
            Interaction(cancel, DomainEventsMetadata.OriginAssociationTypeId, Element(package, "Domain Event", "OrderStatusChangedDomainEvent"));
            Interaction(cancel, "an-association-the-check-doesn't-read", started);

            var invoice = Element(Package("other-app"), "Class", "Invoice");
            Interaction(Element(invoice, "Operation", "Issue"), DomainEventsMetadata.OriginAssociationTypeId, started);

            var facts = DomainDrift.DriftModel.Read(Designer(new[] { package, invoice.Package }), ApplicationId).Where(x => x.Kind.Key == "intent.raises").ToList();

            Facts(facts.Select(x => (x.Kind.Key, x.Element, x.Value))).ShouldBe(new[]
            {
                "intent.raises Order.Order = OrderStartedDomainEvent",
                "intent.raises Order.SetCancelledStatus = OrderCancelledDomainEvent",
                "intent.raises Order.SetCancelledStatus = OrderStatusChangedDomainEvent"
            });
            var raised = facts.Single(x => x.Element == "Order.Order");
            raised.Placeholders.ShouldBe(new Dictionary<string, string> { ["Entity"] = "Order", ["Member"] = "Order", ["DomainEvent"] = "OrderStartedDomainEvent" });
            raised.ParameterNames.ShouldBe(new[] { "userId", "userName" });
            raised.ElementIds.ShouldBe(new[] { order.Id, constructor.Id });
        }

        private static IReadOnlyList<string> Facts(IEnumerable<(string Kind, string Element, string Value)> facts)
        {
            return facts.Select(x => $"{x.Kind} {x.Element} = {x.Value}").OrderBy(x => x, System.StringComparer.Ordinal).ToList();
        }

        /// <summary>An intent fact's target placeholders, in name order.</summary>
        private static string Targets(IReadOnlyDictionary<string, string> placeholders)
        {
            return string.Join(", ", placeholders.Where(x => x.Key is not ("Service" or "Operation" or "Folder")).OrderBy(x => x.Key, System.StringComparer.Ordinal).Select(x => $"{x.Key}={x.Value}"));
        }
    }
}
