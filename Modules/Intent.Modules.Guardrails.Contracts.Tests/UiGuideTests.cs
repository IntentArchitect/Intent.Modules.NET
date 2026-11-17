using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Modules.Common.Drift;
using Intent.Modules.UI.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;
using static Intent.Modules.Guardrails.Contracts.Tests.Guides;
using UiGuides = Intent.Modules.UI.Guardrails.Contracts.Templates.DriftRecipeGuides;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The UI guides' recipe files, run against fixture code in each style a guide covers and a model that agrees with it:
    /// no drift, no warnings from validate-drift-recipes, values from where the guide says they come from, and each planted
    /// change reported as exactly its row. The Blazor and Angular fixtures are trimmed from the samples generated in
    /// NewSolution156 (eShop.Web.Client, the Blazor Server sample and the Angular sample, with a guarded route and a child
    /// component added to the last); the React fixture is hand-written in React Router's style.
    /// </summary>
    public class UiGuideTests
    {
        private const string Ui = FactKinds.Ui;

        private static readonly Dictionary<string, string> EShop = FixtureFolder("Ui/Blazor/eShop");
        private static readonly Dictionary<string, string> BlazorServer = FixtureFolder("Ui/Blazor/Server");
        private static readonly Dictionary<string, string> Angular = FixtureFolder("Ui/Angular");
        private static readonly Dictionary<string, string> React = FixtureFolder("Ui/React");

        private static string BlazorRecipe => RecipeFile(UiGuides.Blazor);
        private static string AngularRecipe => RecipeFile(UiGuides.Angular);
        private static string ReactRecipe => RecipeFile(UiGuides.React);

        // eShop.Web.Client: pages secured with @attribute [Authorize], calls through typed service proxies, navigation by
        // NavigateTo and Href, a layout with child components.
        private static IReadOnlyList<ModelFact> EShopModel() => UiDrift.Facts(new UiModel
        {
            Components = new[]
            {
                Component("Catalog", "Catalog", UiKind.Page, route: "", calls: new[] { new UiCall("ItemsByTypeIdAndBrandId", "CatalogService") }),
                Component("Cart", "Cart", UiKind.Page, route: "cart", secured: true,
                    calls: new[] { new UiCall("GetBasketById", "BasketService"), new UiCall("UpdateBasket", "BasketService") },
                    navigations: new[] { new UiLink("Checkout", "checkout"), new UiLink("Catalog", "") },
                    children: new[] { new UiLink("ProductImagePrice") }),
                Component("Components", "ProductImagePrice", UiKind.Component),
                Component("Checkout", "Checkout", UiKind.Page, route: "checkout", secured: true,
                    calls: new[] { new UiCall("CreateOrderCommand", ""), new UiCall("GetBasketById", "BasketService") },
                    navigations: new[] { new UiLink("Orders", "orders") }),
                Component("User", "Orders", UiKind.Page, route: "orders", secured: true, roles: new[] { "Customer", "Admin" },
                    calls: new[] { new UiCall("GetOrdersQuery", "") }),
                Component("Layout", "MainLayout", UiKind.Layout,
                    navigations: new[] { new UiLink("Cart", "cart"), new UiLink("Orders", "orders"), new UiLink("Catalog", "") },
                    children: new[] { new UiLink("HeaderBar"), new UiLink("FooterBar") }),
                Component("Components", "HeaderBar", UiKind.Component),
                Component("Components", "FooterBar", UiKind.Component)
            }
        });

        // The Blazor Server sample: Mediator calls, MudBlazor dialogs, NavigateTo with route parameters, a policy.
        private static IReadOnlyList<ModelFact> BlazorServerModel() => UiDrift.Facts(new UiModel
        {
            Components = new[]
            {
                Component("Brand", "BrandListPage", UiKind.Page, route: "brand/list", calls: new[] { new UiCall("GetBrandsQuery", "") },
                    dialogs: new[] { new UiLink("BrandAddDialog"), new UiLink("BrandEditDialog") }),
                Component("Brand", "BrandAddDialog", UiKind.Dialog, calls: new[] { new UiCall("CreateBrandCommand", "") }),
                Component("Brand", "BrandEditDialog", UiKind.Dialog, calls: new[] { new UiCall("GetBrandByIdQuery", ""), new UiCall("UpdateBrandCommand", "") }),
                Component("Customer", "CustomerListPage", UiKind.Page, route: "customer/list", calls: new[] { new UiCall("GetCustomersQuery", "") },
                    navigations: new[] { new UiLink("CustomerAddPage", "customer/customer-add-page"), new UiLink("CustomerEditPage", "customer/edit/{CustomerId:guid}") }),
                Component("Customer", "CustomerAddPage", UiKind.Page, route: "customer/customer-add-page", secured: true, policies: new[] { "CanManageCustomers" },
                    calls: new[] { new UiCall("CreateCustomerCommand", "") }, navigations: new[] { new UiLink("CustomerListPage", "customer/list") }),
                Component("Customer", "CustomerEditPage", UiKind.Page, route: "customer/edit/{CustomerId:guid}", calls: new[] { new UiCall("GetCustomerByIdQuery", "") })
            }
        });

        // The Angular sample: routes in app.routes.ts, generated client services, router.navigate with segment arrays and
        // routerLink, Angular Material dialogs, a child component.
        private static IReadOnlyList<ModelFact> AngularModel() => UiDrift.Facts(new UiModel
        {
            Components = new[]
            {
                Component("Layout", "MainLayout", UiKind.Layout,
                    navigations: new[] { new UiLink("Home", ""), new UiLink("BrandListPage", "brand/list"), new UiLink("CustomerListPage", "customer-list") }),
                Component("Pages", "Home", UiKind.Page, route: "", navigations: new[] { new UiLink("CustomerListPage", "customer-list") }),
                Component("Brand", "BrandListPage", UiKind.Page, route: "brand/list", calls: new[] { new UiCall("GetBrandsQuery", "") },
                    dialogs: new[] { new UiLink("BrandAddDialog") }),
                Component("Brand", "BrandAddDialog", UiKind.Dialog, calls: new[] { new UiCall("CreateBrandCommand", "") }),
                Component("Customer", "CustomerListPage", UiKind.Page, route: "customer-list", calls: new[] { new UiCall("GetCustomersQuery", "") },
                    navigations: new[] { new UiLink("CustomerAddPage", "customer/add"), new UiLink("CustomerEditPage", "customer/edit/{customerId}") },
                    children: new[] { new UiLink("CustomerSummaryCard") }),
                Component("Customer", "CustomerAddPage", UiKind.Page, route: "customer/add", secured: true, roles: new[] { "Admin", "Sales" },
                    calls: new[] { new UiCall("CreateCustomerCommand", "") }, navigations: new[] { new UiLink("CustomerListPage", "customer-list") }),
                Component("Customer", "CustomerEditPage", UiKind.Page, route: "customer/edit/{customerId}", secured: true,
                    calls: new[] { new UiCall("GetCustomerByIdQuery", ""), new UiCall("UpdateCustomerCommand", "") }),
                Component("Customer", "CustomerSummaryCard", UiKind.Component)
            }
        });

        // React with React Router: <Route> elements guarded by <RequireAuth>, a client object and an RTK Query hook,
        // <Link to> and navigate with a template literal, an MUI dialog, a child component.
        private static IReadOnlyList<ModelFact> ReactModel() => UiDrift.Facts(new UiModel
        {
            Components = new[]
            {
                Component("Layout", "Layout", UiKind.Layout, navigations: new[] { new UiLink("HomePage", ""), new UiLink("CustomerListPage", "customers") }),
                Component("Pages", "HomePage", UiKind.Page, route: "", navigations: new[] { new UiLink("CustomerListPage", "customers") }),
                Component("Customers", "CustomerListPage", UiKind.Page, route: "customers",
                    calls: new[] { new UiCall("GetCustomersQuery", ""), new UiCall("DeleteCustomerCommand", "") },
                    navigations: new[] { new UiLink("CustomerAddPage", "customers/add"), new UiLink("CustomerEditPage", "customers/{customerId}/edit") },
                    dialogs: new[] { new UiLink("DeleteCustomerDialog") },
                    children: new[] { new UiLink("CustomerCard") }),
                Component("Customers", "CustomerAddPage", UiKind.Page, route: "customers/add", secured: true, roles: new[] { "Admin", "Sales" },
                    calls: new[] { new UiCall("CreateCustomerCommand", "") }, navigations: new[] { new UiLink("CustomerListPage", "customers") }),
                Component("Customers", "CustomerEditPage", UiKind.Page, route: "customers/{customerId}/edit", secured: true,
                    calls: new[] { new UiCall("GetCustomerByIdQuery", ""), new UiCall("UpdateCustomerCommand", "") }),
                Component("Customers", "DeleteCustomerDialog", UiKind.Dialog),
                Component("Components", "CustomerCard", UiKind.Component)
            }
        });

        [Fact]
        public void The_Blazor_recipe_file_reads_the_eShop_client_with_no_drift()
        {
            Check(Ui, BlazorRecipe, EShopModel(), EShop).ShouldBeClean();
        }

        [Fact]
        public void The_Blazor_recipe_file_reads_the_Blazor_Server_sample_with_no_drift()
        {
            Check(Ui, BlazorRecipe, BlazorServerModel(), BlazorServer).ShouldBeClean();
        }

        [Fact]
        public void The_Blazor_recipe_file_validates_and_reads_values_where_the_guide_says()
        {
            var eShop = ShouldValidate(Ui, BlazorRecipe, EShopModel(), EShop);
            eShop.Sources(UiDrift.ComponentKind).ShouldBe((5, 3, 0));
            eShop.Sources(UiDrift.PageRoute).ShouldBe((4, 0, 0));
            eShop.Sources(UiDrift.Security).ShouldBe((3, 1, 0));
            eShop.Sources(UiDrift.SecurityRoles).ShouldBe((1, 0, 0));
            eShop.Sources(UiDrift.IntentInvokes).ShouldBe((6, 0, 0));
            eShop.Sources(UiDrift.IntentNavigates).ShouldBe((6, 0, 0));
            eShop.Sources(UiDrift.IntentUses).ShouldBe((3, 0, 0));

            var server = ShouldValidate(Ui, BlazorRecipe, BlazorServerModel(), BlazorServer);
            server.Sources(UiDrift.ComponentKind).ShouldBe((6, 0, 0));
            server.Sources(UiDrift.Security).ShouldBe((1, 3, 0));
            server.Sources(UiDrift.SecurityPolicy).ShouldBe((1, 0, 0));
            server.Sources(UiDrift.IntentInvokes).ShouldBe((7, 0, 0));
            server.Sources(UiDrift.IntentNavigates).ShouldBe((3, 0, 0));
            server.Sources(UiDrift.IntentOpens).ShouldBe((2, 0, 0));
        }

        [Theory]
        [InlineData("src/eShop.Web.Client/Pages/Checkout/Checkout.razor", "@page \"/checkout\"", "@page \"/basket/checkout\"",
            "`Checkout` route: model `checkout`, code `basket/checkout`")]
        [InlineData("src/eShop.Web.Client/Pages/Checkout/Checkout.razor", "@attribute [Authorize]", "",
            "`Checkout` security: model `Secured`, code `None`")]
        [InlineData("src/eShop.Web.Client/Pages/User/Orders.razor", "Roles = \"Customer, Admin\"", "Roles = \"Admin\"",
            "`Orders` roles: model `Customer,Admin`, code `Admin`")]
        [InlineData("src/eShop.Web.Client/Pages/Checkout/Checkout.razor.cs", "await OrdersService.CreateOrderAsync(new CreateOrderCommand", "await OrdersService.SubmitAsync(new SubmitOrderRequest",
            "`Checkout` invokes: model `CreateOrderCommand`, code (none)")]
        [InlineData("src/eShop.Web.Client/Pages/Checkout/Checkout.razor.cs", "NavigationManager.NavigateTo(\"orders\");", "",
            "`Checkout` navigates to: model `Orders`, code (none)")]
        [InlineData("src/eShop.Web.Client/Layout/MainLayout.razor", "<FooterBar />", "",
            "`MainLayout` uses: model `FooterBar`, code (none)")]
        public void Each_planted_change_to_the_eShop_client_is_exactly_its_row(string path, string from, string to, string row)
        {
            Check(Ui, BlazorRecipe, EShopModel(), Mutate(EShop, path, from, to)).ShouldReport(row);
        }

        [Fact]
        public void A_dialog_the_Blazor_code_stops_opening_is_its_row()
        {
            Check(Ui, BlazorRecipe, BlazorServerModel(), Mutate(BlazorServer, "src/Acme.Server/Components/Pages/Brand/BrandListPage.razor.cs",
                    "DialogService.ShowAsync<BrandAddDialog>(\"Add Brand\")", "DialogService.ShowMessageBox(\"Add Brand\", \"Not yet\")"))
                .ShouldReport("`BrandListPage` opens: model `BrandAddDialog`, code (none)");
        }

        [Fact]
        public void A_new_Blazor_page_or_component_is_reported_as_not_modelled()
        {
            var files = new Dictionary<string, string>(EShop)
            {
                ["src/eShop.Web.Client/Pages/Promotions.razor"] = "@page \"/promotions\"\n<h1>Promotions</h1>\n",
                ["src/eShop.Web.Client/Components/Banner.razor"] = "<div class=\"banner\">@ChildContent</div>\n"
            };

            Check(Ui, BlazorRecipe, EShopModel(), files).ShouldReport(
                "page `src/eShop.Web.Client/Pages/Promotions.razor`: not modelled",
                "component `src/eShop.Web.Client/Components/Banner.razor`: not modelled");
        }

        [Fact]
        public void The_Angular_recipe_file_reads_the_sample_with_no_drift()
        {
            Check(Ui, AngularRecipe, AngularModel(), Angular).ShouldBeClean();
        }

        [Fact]
        public void The_Angular_recipe_file_validates_and_reads_values_where_the_guide_says()
        {
            var validation = ShouldValidate(Ui, AngularRecipe, AngularModel(), Angular);

            validation.Sources(UiDrift.ComponentKind).ShouldBe((7, 1, 0));
            validation.Sources(UiDrift.PageRoute).ShouldBe((5, 0, 0));
            validation.Sources(UiDrift.Security).ShouldBe((2, 3, 0));
            validation.Sources(UiDrift.SecurityRoles).ShouldBe((1, 0, 0));
            validation.Sources(UiDrift.IntentInvokes).ShouldBe((6, 0, 0));
            validation.Sources(UiDrift.IntentNavigates).ShouldBe((7, 0, 0));
            validation.Sources(UiDrift.IntentOpens).ShouldBe((1, 0, 0));
            validation.Sources(UiDrift.IntentUses).ShouldBe((1, 0, 0));
        }

        [Theory]
        [InlineData("src/app/app.routes.ts", "{ path: 'customer-list', component", "{ path: 'customers', component",
            "`CustomerListPage` route: model `customer-list`, code `customers`")]
        [InlineData("src/app/app.routes.ts", "component: CustomerEditPageComponent, canActivate: [authGuard] }", "component: CustomerEditPageComponent }",
            "`CustomerEditPage` security: model `Secured`, code `None`")]
        [InlineData("src/app/app.routes.ts", "roles: ['Admin', 'Sales']", "roles: ['Admin']",
            "`CustomerAddPage` roles: model `Admin,Sales`, code `Admin`")]
        [InlineData("src/app/pages/customer/customer-edit-page/customer-edit-page.component.ts", "this.customersService.updateCustomer(this.model.id, this.model).subscribe();", "",
            "`CustomerEditPage` invokes: model `UpdateCustomerCommand`, code (none)")]
        [InlineData("src/app/pages/customer/customer-list-page/customer-list-page.component.ts", "this.router.navigate(['/customer', 'edit', customerId]);", "",
            "`CustomerListPage` navigates to: model `CustomerEditPage`, code (none)")]
        [InlineData("src/app/pages/brand/brand-list-page/brand-list-page.component.ts", "this.dialog.open(BrandAddDialogComponent, {", "this.dialog.open(this.addDialog, {",
            "`BrandListPage` opens: model `BrandAddDialog`, code (none)")]
        [InlineData("src/app/pages/customer/customer-list-page/customer-list-page.component.html", "<app-customer-summary-card [count]=\"customersModels?.totalCount\"></app-customer-summary-card>", "",
            "`CustomerListPage` uses: model `CustomerSummaryCard`, code (none)")]
        public void Each_planted_change_to_the_Angular_sample_is_exactly_its_row(string path, string from, string to, string row)
        {
            Check(Ui, AngularRecipe, AngularModel(), Mutate(Angular, path, from, to)).ShouldReport(row);
        }

        [Fact]
        public void A_new_Angular_component_is_reported_as_not_modelled()
        {
            var files = new Dictionary<string, string>(Angular)
            {
                ["src/app/pages/promotions/promotions-page.component.ts"] =
                    "import { Component } from '@angular/core';\n\n@Component({\n  selector: 'app-promotions-page',\n  standalone: true,\n  templateUrl: 'promotions-page.component.html'\n})\nexport class PromotionsPageComponent {\n}\n"
            };

            Check(Ui, AngularRecipe, AngularModel(), files).ShouldReport("component `PromotionsPageComponent`: not modelled");
        }

        [Fact]
        public void The_React_recipe_file_reads_the_fixture_with_no_drift()
        {
            Check(Ui, ReactRecipe, ReactModel(), React).ShouldBeClean();
        }

        [Fact]
        public void The_React_recipe_file_validates_and_reads_values_where_the_guide_says()
        {
            var validation = ShouldValidate(Ui, ReactRecipe, ReactModel(), React);

            validation.Sources(UiDrift.ComponentKind).ShouldBe((6, 1, 0));
            validation.Sources(UiDrift.PageRoute).ShouldBe((4, 0, 0));
            validation.Sources(UiDrift.Security).ShouldBe((2, 2, 0));
            validation.Sources(UiDrift.SecurityRoles).ShouldBe((1, 0, 0));
            validation.Sources(UiDrift.IntentInvokes).ShouldBe((5, 0, 0));
            validation.Sources(UiDrift.IntentNavigates).ShouldBe((6, 0, 0));
            validation.Sources(UiDrift.IntentOpens).ShouldBe((1, 0, 0));
            validation.Sources(UiDrift.IntentUses).ShouldBe((1, 0, 0));
        }

        [Theory]
        [InlineData("src/App.tsx", "<Route path=\"/customers\" element", "<Route path=\"/clients\" element",
            "`CustomerListPage` route: model `customers`, code `clients`")]
        [InlineData("src/App.tsx", "element={<RequireAuth><CustomerEditPage /></RequireAuth>}", "element={<CustomerEditPage />}",
            "`CustomerEditPage` security: model `Secured`, code `None`")]
        [InlineData("src/pages/customers/CustomerAddPage.tsx", "useCreateCustomerMutation()", "useAddClientMutation()",
            "`CustomerAddPage` invokes: model `CreateCustomerCommand`, code (none)")]
        [InlineData("src/pages/customers/CustomerListPage.tsx", "<Link to=\"/customers/add\">Add customer</Link>", "",
            "`CustomerListPage` navigates to: model `CustomerAddPage`, code (none)")]
        [InlineData("src/pages/customers/CustomerListPage.tsx", "<DeleteCustomerDialog open", "<ConfirmDialog open",
            "`CustomerListPage` opens: model `DeleteCustomerDialog`, code (none)")]
        [InlineData("src/pages/customers/CustomerListPage.tsx", "<CustomerCard\n", "<ClientCard\n",
            "`CustomerListPage` uses: model `CustomerCard`, code (none)")]
        public void Each_planted_change_to_the_React_fixture_is_exactly_its_row(string path, string from, string to, string row)
        {
            Check(Ui, ReactRecipe, ReactModel(), Mutate(React, path, from, to)).ShouldReport(row);
        }

        [Fact]
        public void A_new_React_component_is_reported_as_not_modelled()
        {
            var files = new Dictionary<string, string>(React)
            {
                ["src/pages/PromotionsPage.tsx"] = "export function PromotionsPage() {\n  return <h1>Promotions</h1>;\n}\n"
            };

            Check(Ui, ReactRecipe, ReactModel(), files).ShouldReport("component `PromotionsPage`: not modelled");
        }

        private static UiComponent Component(string folder, string name, string kind, string route = null, bool secured = false,
            string[] roles = null, string[] policies = null, UiCall[] calls = null, UiLink[] navigations = null, UiLink[] dialogs = null, UiLink[] children = null)
        {
            return new UiComponent
            {
                Id = $"component:{name}",
                Name = name,
                Folder = folder,
                Kind = kind,
                Route = route,
                Secured = secured,
                Roles = roles ?? Array.Empty<string>(),
                Policies = policies ?? Array.Empty<string>(),
                Calls = calls ?? Array.Empty<UiCall>(),
                Navigations = navigations ?? Array.Empty<UiLink>(),
                Dialogs = dialogs ?? Array.Empty<UiLink>(),
                Children = children ?? Array.Empty<UiLink>()
            };
        }

        /// <summary>A fixture folder's files by their path inside it, with line breaks as <c>\n</c> whatever git checked out.</summary>
        private static Dictionary<string, string> FixtureFolder(string relativePath)
        {
            var root = Path.Combine(AppContext.BaseDirectory, "Fixtures", relativePath);
            return Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                .ToDictionary(x => Path.GetRelativePath(root, x).Replace('\\', '/'), x => File.ReadAllText(x).Replace("\r\n", "\n"));
        }
    }
}
