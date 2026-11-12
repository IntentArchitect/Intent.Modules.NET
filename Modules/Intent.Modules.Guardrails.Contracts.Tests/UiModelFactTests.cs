using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Drift;
using Intent.Modules.UI.Guardrails.Contracts.Drift;
using NSubstitute;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Fakes;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The User Interface designer, as Intent.Modelers.UI 1.1.6 and Intent.Modelers.UI.Core 1.0.4 shape it, read into facts
    /// by <see cref="UserInterfaceDesigner1"/> and <see cref="UiDrift"/>. The type ids below are those versions' own, so these
    /// tests also pin what the reader reads.
    /// </summary>
    public class UiModelFactTests
    {
        private const string ComponentTypeId = "b1c481e1-e91e-4c29-9817-00ab9cad4b6b";
        private const string ComponentOperationTypeId = "e030c97a-e066-40a7-8188-808c275df3cb";
        private const string LayoutTypeId = "776a9393-6b23-4a8c-8937-fd7e833fa0ef";
        private const string LayoutHeaderTypeId = "a6c3a89e-5932-4ab6-a406-75444f05beee";
        private const string DisplayComponentTypeId = "866a90f7-4044-43b9-bb05-7270c7889796";
        private const string CallServiceOperationAction = "fe5a5cd8-aabd-472f-8d42-f5c233e658dc";
        private const string Navigation = "6d2b2070-c1cb-4cd2-88b4-4e5f8414bd9e";
        private const string Composition = "503b9ea9-4e8b-41d7-bbc6-92c97666c476";
        private const string ShowDialog = "2a309fb2-c487-4c00-9462-16dac9824731";

        [Fact]
        public void A_page_states_its_kind_route_and_security_with_roles_and_policies_in_model_order()
        {
            var package = Package();
            var folder = Element(package, "Folder", "Checkout");
            var checkout = Ui(folder, "Checkout", Stereotype("Page", ("Route", "/checkout"), ("Title", "Checkout")),
                Stereotype("Secured", ("Roles", "Customer, Admin"), ("Policy", "CanBuy")));

            var facts = Read(package);

            Values(facts).ShouldBe(new[]
            {
                "component.kind Checkout = Page",
                "page.route Checkout = checkout",
                "security Checkout = Secured",
                "security.roles Checkout = Customer,Admin",
                "security.policy Checkout = CanBuy"
            });
            facts.ShouldAllBe(x => x.ElementIds.SequenceEqual(new[] { checkout.Id }));
            facts[0].Placeholders.ShouldBe(new Dictionary<string, string> { ["Folder"] = "Checkout", ["Component"] = "Checkout" }, ignoreOrder: true);
        }

        [Fact]
        public void A_page_with_no_Secured_states_None_and_a_plain_component_states_no_security()
        {
            var package = Package();
            Ui(package, "Catalog", Stereotype("Page", ("Route", "/")));
            Ui(package, "FooterBar");
            Ui(package, "AdminPanel", Stereotype("Secured", ("Roles", ""), ("Policy", "")));

            Values(Read(package)).ShouldBe(new[]
            {
                "component.kind Catalog = Page",
                "page.route Catalog = ",
                "security Catalog = None",
                "component.kind FooterBar = Component",
                "component.kind AdminPanel = Component",
                "security AdminPanel = Secured"
            });
        }

        [Fact]
        public void A_call_or_navigation_modelled_on_a_Component_Operation_counts_as_the_component_s()
        {
            var package = Package();
            var service = Element(package, "Service", "BasketService");
            var getBasket = Element(service, "Operation", "GetBasketById");
            var createOrder = Element(package, "Command", "CreateOrderCommand");
            var orders = Ui(package, "Orders", Stereotype("Page", ("Route", "orders")));
            var checkout = Ui(package, "Checkout", Stereotype("Page", ("Route", "checkout")));
            var operation = Element(checkout, "Component Operation", "CheckoutCart");
            operation.SpecializationTypeId.Returns(ComponentOperationTypeId);
            Interaction(operation, CallServiceOperationAction, createOrder);
            Interaction(operation, Navigation, orders);
            Interaction(checkout, CallServiceOperationAction, getBasket);

            var facts = Read(package).Where(x => x.Element == "Checkout" && x.Kind.Key.StartsWith("intent.")).ToList();

            Values(facts).ShouldBe(new[]
            {
                "intent.invokes Checkout = CreateOrderCommand",
                "intent.invokes Checkout = GetBasketById",
                "intent.navigates Checkout = Orders"
            }, ignoreOrder: true);
            facts.Single(x => x.Value == "CreateOrderCommand").Placeholders["InvokedOwner"].ShouldBe("");
            facts.Single(x => x.Value == "GetBasketById").Placeholders["InvokedOwner"].ShouldBe("BasketService");
            var navigation = facts.Single(x => x.Kind == UiDrift.IntentNavigates);
            navigation.Placeholders["Route"].ShouldBe("orders");
            navigation.Placeholders["RouteStart"].ShouldBe("orders");
        }

        [Fact]
        public void A_layout_states_its_kind_and_owns_what_its_header_navigates_to_and_renders()
        {
            var package = Package();
            var cart = Ui(package, "Cart", Stereotype("Page", ("Route", "/cart")));
            var headerBar = Ui(package, "HeaderBar");
            var layout = Element(package, "Layout", "MainLayout");
            layout.SpecializationTypeId.Returns(LayoutTypeId);
            var header = Element(layout, "Layout Header", "Header");
            header.SpecializationTypeId.Returns(LayoutHeaderTypeId);
            Interaction(header, Navigation, cart);
            var display = Element(header, "Display Component", "HeaderBar");
            display.SpecializationTypeId.Returns(DisplayComponentTypeId);
            var reference = Substitute.For<ITypeReference>();
            reference.Element.Returns(headerBar);
            display.TypeReference.Returns(reference);

            Values(Read(package).Where(x => x.Element == "MainLayout")).ShouldBe(new[]
            {
                "component.kind MainLayout = Layout",
                "intent.navigates MainLayout = Cart",
                "intent.uses MainLayout = HeaderBar"
            });
        }

        [Fact]
        public void A_component_a_Show_Dialog_opens_is_a_dialog_with_or_without_the_Dialog_stereotype()
        {
            var package = Package();
            var add = Ui(package, "BrandAddDialog");
            var edit = Ui(package, "BrandEditDialog", Stereotype("Dialog"));
            var list = Ui(package, "BrandListPage", Stereotype("Page", ("Route", "brand/list")));
            Interaction(list, ShowDialog, add);
            Interaction(list, ShowDialog, edit);

            var facts = Read(package);

            Values(facts.Where(x => x.Kind == UiDrift.ComponentKind)).ShouldBe(new[]
            {
                "component.kind BrandAddDialog = Dialog",
                "component.kind BrandEditDialog = Dialog",
                "component.kind BrandListPage = Page"
            });
            Values(facts.Where(x => x.Kind == UiDrift.IntentOpens)).ShouldBe(new[]
            {
                "intent.opens BrandListPage = BrandAddDialog",
                "intent.opens BrandListPage = BrandEditDialog"
            }, ignoreOrder: true);
        }

        [Fact]
        public void A_Composition_states_what_the_component_renders()
        {
            var package = Package();
            var card = Ui(package, "CustomerCard");
            var list = Ui(package, "CustomerList");
            Interaction(list, Composition, card);

            Values(Read(package).Where(x => x.Kind == UiDrift.IntentUses)).ShouldBe(new[] { "intent.uses CustomerList = CustomerCard" });
        }

        [Fact]
        public void A_two_way_Navigation_leads_both_ways()
        {
            var package = Package();
            var list = Ui(package, "CustomerListPage", Stereotype("Page", ("Route", "customer/list")));
            var edit = Ui(package, "CustomerEditPage", Stereotype("Page", ("Route", "/customer/edit/{CustomerId:guid}")));
            var association = Interaction(list, Navigation, edit).Association;
            var back = Substitute.For<IAssociationEnd>();
            back.Association.Returns(association);
            back.IsTargetEnd().Returns(false);
            back.IsSourceEnd().Returns(true);
            back.IsNavigable.Returns(true);
            var reference = Substitute.For<ITypeReference>();
            reference.Element.Returns(list);
            back.TypeReference.Returns(reference);
            ((List<IAssociationEnd>)edit.AssociatedElements).Add(back);

            var facts = Read(package).Where(x => x.Kind == UiDrift.IntentNavigates).ToList();

            Values(facts).ShouldBe(new[]
            {
                "intent.navigates CustomerListPage = CustomerEditPage",
                "intent.navigates CustomerEditPage = CustomerListPage"
            });
            facts[0].Placeholders["Route"].ShouldBe("customer/edit/{CustomerId:guid}");
            facts[0].Placeholders["RouteStart"].ShouldBe("customer/edit");
        }

        [Fact]
        public void A_one_way_Navigation_leads_only_from_its_source()
        {
            var package = Package();
            var list = Ui(package, "CustomerListPage", Stereotype("Page", ("Route", "customer/list")));
            var edit = Ui(package, "CustomerEditPage", Stereotype("Page", ("Route", "customer/edit")));
            var association = Interaction(list, Navigation, edit).Association;
            var back = Substitute.For<IAssociationEnd>();
            back.Association.Returns(association);
            back.IsTargetEnd().Returns(false);
            back.IsNavigable.Returns(false);
            ((List<IAssociationEnd>)edit.AssociatedElements).Add(back);

            Values(Read(package).Where(x => x.Kind == UiDrift.IntentNavigates)).ShouldBe(new[] { "intent.navigates CustomerListPage = CustomerEditPage" });
        }

        [Fact]
        public void Only_this_application_s_packages_count()
        {
            var mine = Package();
            Ui(mine, "Checkout", Stereotype("Page", ("Route", "checkout")));
            var theirs = Package("another-app");
            Ui(theirs, "Admin", Stereotype("Page", ("Route", "admin")));

            var designer = Designer(new[] { mine, theirs });

            UiDrift.Facts(UserInterfaceDesigner1.Read(designer, ApplicationId)).ShouldAllBe(x => x.Element == "Checkout");
            UserInterfaceDesigner1.ElementsById(designer, ApplicationId).Keys.ShouldBe(new[] { "Component:Checkout" });
        }

        [Theory]
        [InlineData("customer/edit/{CustomerId:guid}", "customer/edit")]
        [InlineData("item/{ProductId:int}", "item")]
        [InlineData("customers/:customerId/edit", "customers")]
        [InlineData("orders", "orders")]
        [InlineData("", "")]
        public void A_route_starts_before_its_first_parameter(string route, string start)
        {
            UiDrift.StartOf(route).ShouldBe(start);
        }

        [Fact]
        public void The_designer_contribution_names_the_UI_designer_and_its_task()
        {
            UiDrift.Designer.Name.ShouldBe("UI");
            UiDrift.Designer.DesignerId.ShouldBe("f492faed-0665-4513-9853-5a230721786f");
            UiDrift.Designer.Order.ShouldBe(30);
            UiDrift.Designer.ValidateTaskId.ShouldBe("Intent.UI.Guardrails.Contracts.ValidateDriftRecipes");
        }

        private static IElement Ui(object parent, string name, params IStereotype[] stereotypes)
        {
            var element = Element(parent, "Component", name, stereotypes);
            element.SpecializationTypeId.Returns(ComponentTypeId);
            return element;
        }

        private static IReadOnlyList<ModelFact> Read(IPackage package) =>
            UiDrift.Facts(UserInterfaceDesigner1.Read(Designer(new[] { package }), ApplicationId));

        private static IEnumerable<string> Values(IEnumerable<ModelFact> facts) => facts.Select(x => $"{x.Kind.Key} {x.Element} = {x.Value}");
    }
}
