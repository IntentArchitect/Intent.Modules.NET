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
    /// The Services guides' recipe files, run against code in each style a guide covers and a model that agrees with it:
    /// no drift, no warnings from validate-drift-recipes, values from where the guide says they come from, and each planted
    /// change reported as exactly its rows.
    /// </summary>
    public class ServicesGuideTests
    {
        private const string Services = FactKinds.Services;

        // ASP.NET Core controllers: a base controller that routes and secures, actions that override it, every binding.
        private static readonly Dictionary<string, string> Controllers = new()
        {
            ["src/Acme.Api/Controllers/ApiControllerBase.cs"] = """
                namespace Acme.Api.Controllers;

                [ApiController]
                [Route("api/[controller]")]
                [Authorize]
                public abstract class ApiControllerBase : ControllerBase
                {
                }
                """,
            ["src/Acme.Api/Controllers/OrdersController.cs"] = """
                namespace Acme.Api.Controllers;

                public class OrdersController : ApiControllerBase
                {
                    [HttpGet]
                    public IActionResult GetOrders([FromQuery] int page, string? status) => Ok();

                    [HttpGet("{id:guid}")]
                    public IActionResult GetOrder(Guid id) => Ok();

                    [HttpPost]
                    [Authorize(Roles = "Admin, Sales")]
                    public IActionResult CreateOrder(CreateOrderDto dto) => Ok();

                    [HttpPut("{id}")]
                    [Authorize(Policy = "CanEdit")]
                    public IActionResult UpdateOrder(Guid id, [FromBody] UpdateOrderDto dto) => Ok();

                    [HttpDelete("{id}"), AllowAnonymous]
                    public IActionResult DeleteOrder(Guid id) => Ok();

                    [HttpPost("upload")]
                    public IActionResult Upload(IFormFile file) => Ok();
                }
                """,
            ["src/Acme.Api/Controllers/HealthController.cs"] = """
                namespace Acme.Api.Controllers;

                [ApiController]
                [Route("api/v1/health")]
                public class HealthController : ControllerBase
                {
                    [HttpGet("~/ping")]
                    public IActionResult Ping([FromHeader(Name = "X-Probe")] string probe) => Ok();
                }
                """
        };

        private static List<ModelFact> ControllersModel()
        {
            var facts = new List<ModelFact>
            {
                Fact(FactKinds.ServiceRoute, "OrdersController", "api/Orders", ("Service", "OrdersController")),
                Fact(FactKinds.ServiceRoute, "HealthController", "api/v1/health", ("Service", "HealthController")),
                Endpoint(FactKinds.SecurityRoles, "OrdersController", "CreateOrder", "Admin,Sales"),
                Endpoint(FactKinds.SecurityPolicy, "OrdersController", "UpdateOrder", "CanEdit"),
                Binding("OrdersController", "GetOrders", "page", "From Query"),
                Binding("OrdersController", "GetOrders", "status", "From Query"),
                Binding("OrdersController", "GetOrder", "id", "From Route"),
                Binding("OrdersController", "CreateOrder", "dto", "From Body"),
                Binding("OrdersController", "UpdateOrder", "id", "From Route"),
                Binding("OrdersController", "UpdateOrder", "dto", "From Body"),
                Binding("OrdersController", "DeleteOrder", "id", "From Route"),
                Binding("OrdersController", "Upload", "file", "From Form"),
                Binding("HealthController", "Ping", "probe", "From Header")
            };
            foreach (var (service, operation, verb, route, security) in new[]
            {
                ("OrdersController", "GetOrders", "GET", "", "Secured"),
                ("OrdersController", "GetOrder", "GET", "{id:guid}", "Secured"),
                ("OrdersController", "CreateOrder", "POST", "", "Secured"),
                ("OrdersController", "UpdateOrder", "PUT", "{id}", "Secured"),
                ("OrdersController", "DeleteOrder", "DELETE", "{id}", "Unsecured"),
                ("OrdersController", "Upload", "POST", "upload", "Secured"),
                ("HealthController", "Ping", "GET", "~/ping", "None")
            })
            {
                facts.Add(Endpoint(FactKinds.OperationVerb, service, operation, verb));
                facts.Add(Endpoint(FactKinds.OperationRoute, service, operation, route));
                facts.Add(Endpoint(FactKinds.Security, service, operation, security));
            }

            return facts;
        }

        private static string ControllersRecipe => RecipeFile(ServicesGuides.AspNetCoreControllers);

        [Fact]
        public void The_controllers_recipe_file_reads_every_form_with_no_drift()
        {
            Check(Services, ControllersRecipe, ControllersModel(), Controllers).ShouldBeClean();
        }

        [Fact]
        public void The_controllers_recipe_file_validates_and_reads_values_where_the_guide_says()
        {
            var validation = ShouldValidate(Services, ControllersRecipe, ControllersModel(), Controllers);

            validation.Sources(FactKinds.ServiceRoute).ShouldBe((2, 0, 0));
            validation.Sources(FactKinds.OperationVerb).ShouldBe((7, 0, 0));
            validation.Sources(FactKinds.OperationRoute).ShouldBe((5, 2, 0));
            validation.Sources(FactKinds.Security).ShouldBe((6, 1, 0));
            validation.Sources(FactKinds.ParameterSource).ShouldBe((8, 1, 0));
        }

        [Theory]
        [InlineData("src/Acme.Api/Controllers/ApiControllerBase.cs", "[Route(\"api/[controller]\")]", "[Route(\"api/v2/[controller]\")]",
            "`OrdersController` route: model `api/Orders`, code `api/v2/Orders`")]
        [InlineData("src/Acme.Api/Controllers/OrdersController.cs", "[HttpPut(\"{id}\")]", "[HttpPatch(\"{id}\")]",
            "`OrdersController.UpdateOrder` verb: model `PUT`, code `PATCH`")]
        [InlineData("src/Acme.Api/Controllers/OrdersController.cs", "[HttpGet(\"{id:guid}\")]", "[HttpGet(\"{id:int}\")]",
            "`OrdersController.GetOrder` route: model `{id:guid}`, code `{id:int}`")]
        [InlineData("src/Acme.Api/Controllers/OrdersController.cs", "[HttpDelete(\"{id}\"), AllowAnonymous]", "[HttpDelete(\"{id}\")]",
            "`OrdersController.DeleteOrder` security: model `Unsecured`, code `Secured`")]
        [InlineData("src/Acme.Api/Controllers/OrdersController.cs", "Roles = \"Admin, Sales\"", "Roles = \"Admin\"",
            "`OrdersController.CreateOrder` roles: model `Admin,Sales`, code `Admin`")]
        [InlineData("src/Acme.Api/Controllers/OrdersController.cs", "Policy = \"CanEdit\"", "Policy = \"CanChange\"",
            "`OrdersController.UpdateOrder` policy: model `CanEdit`, code `CanChange`")]
        [InlineData("src/Acme.Api/Controllers/OrdersController.cs", "[FromQuery] int page", "[FromHeader] int page",
            "`OrdersController.GetOrders(page)` binding: model `From Query`, code `From Header`")]
        [InlineData("src/Acme.Api/Controllers/OrdersController.cs", "CreateOrder(CreateOrderDto dto)", "CreateOrder([FromForm] CreateOrderDto dto)",
            "`OrdersController.CreateOrder(dto)` binding: model `From Body`, code `From Form`")]
        [InlineData("src/Acme.Api/Controllers/HealthController.cs", "[HttpGet(\"~/ping\")]", "[HttpGet(\"~/healthz\")]",
            "`HealthController.Ping` route: model `~/ping`, code `~/healthz`")]
        [InlineData("src/Acme.Api/Controllers/HealthController.cs", "public class HealthController", "[Authorize]\npublic class HealthController",
            "`HealthController.Ping` security: model `None`, code `Secured`")]
        public void A_controller_change_is_reported_as_its_one_row(string path, string from, string to, string row)
        {
            Check(Services, ControllersRecipe, ControllersModel(), Mutate(Controllers, path, from, to)).ShouldReport(row);
        }

        [Fact]
        public void An_unmodelled_action_is_reported()
        {
            var files = Mutate(Controllers, "src/Acme.Api/Controllers/OrdersController.cs", "[HttpPost(\"upload\")]",
                "[HttpPost(\"{id}/cancel\")]\n    public IActionResult CancelOrder(Guid id) => Ok();\n\n    [HttpPost(\"upload\")]");

            Check(Services, ControllersRecipe, ControllersModel(), files).ShouldReport("endpoint `OrdersController.CancelOrder`: not modelled");
        }

        [Fact]
        public void A_route_from_a_constant_can_not_be_verified()
        {
            var files = Mutate(Controllers, "src/Acme.Api/Controllers/OrdersController.cs", "[HttpPost(\"upload\")]", "[HttpPost(Routes.Upload)]");

            Check(Services, ControllersRecipe, ControllersModel(), files).Wrong(DriftSection.Unverifiable).ShouldHaveSingleItem()
                .ShouldStartWith("`OrdersController.Upload` route: stated in a form the recipe can't read: `[HttpPost(Routes.Upload)]`");
        }

        // Minimal APIs: a class of handler methods in a secured group, a named lambda, and Commands and Queries in a
        // group of their own.
        private static readonly Dictionary<string, string> MinimalApis = new()
        {
            ["src/Acme.Api/Endpoints/OrderEndpoints.cs"] = """
                namespace Acme.Api.Endpoints;

                public static class OrderEndpoints
                {
                    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
                    {
                        var group = app.MapGroup("/api/orders").RequireAuthorization();

                        group.MapGet("/", GetOrders);
                        group.MapGet("/{id:guid}", GetOrder).WithName("GetOrder");
                        group.MapPost("/", CreateOrder).RequireAuthorization(p => p.RequireRole("Admin", "Sales"));
                        group.MapDelete("/{id:guid}", async (Guid id, IOrderService orders) => await orders.Delete(id))
                            .WithName("DeleteOrder")
                            .AllowAnonymous();

                        return app;
                    }

                    public static Task<IResult> GetOrders(string? status, int page, IOrderService orders) => orders.List(status, page);

                    public static Task<IResult> GetOrder(Guid id, IOrderService orders) => orders.Get(id);

                    [Authorize(Policy = "CanCreate")]
                    public static Task<IResult> CreateOrder(CreateOrderDto dto, IOrderService orders) => orders.Create(dto);
                }
                """,
            ["src/Acme.Api/Endpoints/InvoiceEndpoints.cs"] = """
                namespace Acme.Api.Endpoints;

                public static class InvoiceEndpoints
                {
                    public static void MapInvoiceEndpoints(this IEndpointRouteBuilder app)
                    {
                        var invoices = app.MapGroup("api/invoices");

                        invoices.MapPost("/", async (CreateInvoiceCommand command, ISender sender) => await sender.Send(command));
                        invoices.MapGet("/{id}", async ([AsParameters] GetInvoiceQuery query, ISender sender) => await sender.Send(query))
                            .RequireAuthorization("Billing");
                    }
                }
                """,
            ["src/Acme.Application/Invoices/Requests.cs"] = """
                namespace Acme.Application.Invoices;

                public class CreateInvoiceCommand
                {
                    public string Number { get; set; } = null!;
                    public decimal Amount { get; set; }
                }

                public class GetInvoiceQuery
                {
                    public Guid Id { get; set; }
                    public bool IncludeLines { get; set; }
                }
                """
        };

        private static List<ModelFact> MinimalApisModel()
        {
            var facts = new List<ModelFact>
            {
                Fact(FactKinds.ServiceRoute, "OrderEndpoints", "api/orders", ("Service", "OrderEndpoints")),
                Endpoint(FactKinds.SecurityRoles, "OrderEndpoints", "CreateOrder", "Admin,Sales"),
                Endpoint(FactKinds.SecurityPolicy, "OrderEndpoints", "CreateOrder", "CanCreate"),
                Endpoint(FactKinds.SecurityPolicy, "", "GetInvoiceQuery", "Billing"),
                Binding("OrderEndpoints", "GetOrders", "status", "From Query"),
                Binding("OrderEndpoints", "GetOrders", "page", "From Query"),
                Binding("OrderEndpoints", "GetOrder", "id", "From Route"),
                Binding("OrderEndpoints", "CreateOrder", "dto", "From Body"),
                Binding("OrderEndpoints", "DeleteOrder", "id", "From Route"),
                Binding("", "CreateInvoiceCommand", "Number", "From Body"),
                Binding("", "CreateInvoiceCommand", "Amount", "From Body"),
                Binding("", "GetInvoiceQuery", "Id", "From Route"),
                Binding("", "GetInvoiceQuery", "IncludeLines", "From Query")
            };
            foreach (var (service, operation, verb, route, security) in new[]
            {
                ("OrderEndpoints", "GetOrders", "GET", "", "Secured"),
                ("OrderEndpoints", "GetOrder", "GET", "{id:guid}", "Secured"),
                ("OrderEndpoints", "CreateOrder", "POST", "", "Secured"),
                ("OrderEndpoints", "DeleteOrder", "DELETE", "{id:guid}", "Unsecured"),
                ("", "CreateInvoiceCommand", "POST", "api/invoices", "None"),
                ("", "GetInvoiceQuery", "GET", "api/invoices/{id}", "Secured")
            })
            {
                facts.Add(Endpoint(FactKinds.OperationVerb, service, operation, verb));
                facts.Add(Endpoint(FactKinds.OperationRoute, service, operation, route));
                facts.Add(Endpoint(FactKinds.Security, service, operation, security));
            }

            return facts;
        }

        private static string MinimalApisRecipe => RecipeFile(ServicesGuides.MinimalApis);

        [Fact]
        public void The_minimal_APIs_recipe_file_reads_every_form_with_no_drift()
        {
            Check(Services, MinimalApisRecipe, MinimalApisModel(), MinimalApis).ShouldBeClean();
        }

        [Fact]
        public void The_minimal_APIs_recipe_file_validates_and_reads_values_where_the_guide_says()
        {
            var validation = ShouldValidate(Services, MinimalApisRecipe, MinimalApisModel(), MinimalApis);

            validation.Sources(FactKinds.ServiceRoute).ShouldBe((1, 0, 0));
            validation.Sources(FactKinds.OperationRoute).ShouldBe((6, 0, 0));
            validation.Sources(FactKinds.Security).ShouldBe((5, 1, 0));
            validation.Sources(FactKinds.ParameterSource).ShouldBe((8, 1, 0));
        }

        [Theory]
        [InlineData("src/Acme.Api/Endpoints/OrderEndpoints.cs", "MapGroup(\"/api/orders\")", "MapGroup(\"/api/sales-orders\")",
            "`OrderEndpoints` route: model `api/orders`, code `api/sales-orders`")]
        [InlineData("src/Acme.Api/Endpoints/OrderEndpoints.cs", "group.MapGet(\"/{id:guid}\", GetOrder)", "group.MapPut(\"/{id:guid}\", GetOrder)",
            "`OrderEndpoints.GetOrder` verb: model `GET`, code `PUT`")]
        [InlineData("src/Acme.Api/Endpoints/OrderEndpoints.cs", "group.MapGet(\"/{id:guid}\", GetOrder)", "group.MapGet(\"/{orderId:guid}\", GetOrder)",
            "`OrderEndpoints.GetOrder` route: model `{id:guid}`, code `{orderId:guid}`", "`OrderEndpoints.GetOrder(id)` binding: model `From Route`, code `From Query`")]
        [InlineData("src/Acme.Api/Endpoints/OrderEndpoints.cs", "\n            .AllowAnonymous();", ";",
            "`OrderEndpoints.DeleteOrder` security: model `Unsecured`, code `Secured`")]
        [InlineData("src/Acme.Api/Endpoints/OrderEndpoints.cs", "RequireRole(\"Admin\", \"Sales\")", "RequireRole(\"Admin\", \"Support\")",
            "`OrderEndpoints.CreateOrder` roles: model `Admin,Sales`, code `Admin,Support`")]
        [InlineData("src/Acme.Api/Endpoints/OrderEndpoints.cs", "Policy = \"CanCreate\"", "Policy = \"CanAdd\"",
            "`OrderEndpoints.CreateOrder` policy: model `CanCreate`, code `CanAdd`")]
        [InlineData("src/Acme.Api/Endpoints/OrderEndpoints.cs", "app.MapGroup(\"/api/orders\").RequireAuthorization()", "app.MapGroup(\"/api/orders\")",
            "`OrderEndpoints.GetOrder` security: model `Secured`, code `None`", "`OrderEndpoints.GetOrders` security: model `Secured`, code `None`")]
        [InlineData("src/Acme.Api/Endpoints/InvoiceEndpoints.cs", "MapGroup(\"api/invoices\")", "MapGroup(\"api/bills\")",
            "`CreateInvoiceCommand` route: model `api/invoices`, code `api/bills`", "`GetInvoiceQuery` route: model `api/invoices/{id}`, code `api/bills/{id}`")]
        [InlineData("src/Acme.Api/Endpoints/InvoiceEndpoints.cs", ".RequireAuthorization(\"Billing\")", ".RequireAuthorization(\"Finance\")",
            "`GetInvoiceQuery` policy: model `Billing`, code `Finance`")]
        [InlineData("src/Acme.Api/Endpoints/InvoiceEndpoints.cs", "[AsParameters] GetInvoiceQuery", "GetInvoiceQuery",
            "`GetInvoiceQuery(Id)` binding: model `From Route`, code `From Body`", "`GetInvoiceQuery(IncludeLines)` binding: model `From Query`, code `From Body`")]
        public void A_minimal_API_change_is_reported_as_its_rows(string path, string from, string to, params string[] rows)
        {
            Check(Services, MinimalApisRecipe, MinimalApisModel(), Mutate(MinimalApis, path, from, to)).ShouldReport(rows);
        }

        [Fact]
        public void An_unmodelled_minimal_API_endpoint_is_reported()
        {
            var files = Mutate(MinimalApis, "src/Acme.Api/Endpoints/OrderEndpoints.cs", "return app;", "group.MapPost(\"/{id:guid}/cancel\", CancelOrder);\n\n        return app;");

            Check(Services, MinimalApisRecipe, MinimalApisModel(), files).ShouldReport("endpoint `OrderEndpoints: MapPost /{id:guid}/cancel`: not modelled");
        }

        // FastEndpoints: a command endpoint, a query endpoint, and one with no request.
        private static readonly Dictionary<string, string> FastEndpoints = new()
        {
            ["src/Acme.Api/Orders/CreateOrderEndpoint.cs"] = """
                namespace Acme.Api.Orders;

                public class CreateOrderEndpoint : Endpoint<CreateOrderCommand, OrderDto>
                {
                    public override void Configure()
                    {
                        Post("/api/orders");
                        Roles("Admin", "Sales");
                    }

                    public override Task HandleAsync(CreateOrderCommand req, CancellationToken ct) => SendAsync(new OrderDto(), cancellation: ct);
                }
                """,
            ["src/Acme.Api/Orders/GetOrderEndpoint.cs"] = """
                namespace Acme.Api.Orders;

                public class GetOrderEndpoint : Endpoint<GetOrderQuery, OrderDto>
                {
                    public override void Configure()
                    {
                        Get("/api/orders/{id}");
                        AllowAnonymous();
                    }

                    public override Task HandleAsync(GetOrderQuery req, CancellationToken ct) => SendAsync(new OrderDto(), cancellation: ct);
                }
                """,
            ["src/Acme.Api/Orders/ListOrdersEndpoint.cs"] = """
                namespace Acme.Api.Orders;

                public class ListOrdersEndpoint : EndpointWithoutRequest<List<OrderDto>>
                {
                    public override void Configure()
                    {
                        Verbs(Http.GET);
                        Routes("/api/orders");
                        Policies("CanList");
                    }

                    public override Task HandleAsync(CancellationToken ct) => SendAsync(new List<OrderDto>(), cancellation: ct);
                }
                """,
            ["src/Acme.Api/Orders/Requests.cs"] = """
                namespace Acme.Api.Orders;

                public class CreateOrderCommand
                {
                    public string Reference { get; set; } = null!;

                    [FromHeader("X-Tenant")]
                    public string Tenant { get; set; } = null!;
                }

                public class GetOrderQuery
                {
                    public Guid Id { get; set; }

                    [QueryParam]
                    public bool IncludeLines { get; set; }

                    public string? Expand { get; set; }
                }
                """
        };

        private static List<ModelFact> FastEndpointsModel()
        {
            var facts = new List<ModelFact>
            {
                Endpoint(FactKinds.SecurityRoles, "", "CreateOrderCommand", "Admin,Sales"),
                Endpoint(FactKinds.SecurityPolicy, "", "ListOrders", "CanList"),
                Binding("", "CreateOrderCommand", "Reference", "From Body"),
                Binding("", "CreateOrderCommand", "Tenant", "From Header"),
                Binding("", "GetOrderQuery", "Id", "From Route"),
                Binding("", "GetOrderQuery", "IncludeLines", "From Query"),
                Binding("", "GetOrderQuery", "Expand", "From Query")
            };
            foreach (var (operation, verb, route, security) in new[]
            {
                ("CreateOrderCommand", "POST", "api/orders", "Secured"),
                ("GetOrderQuery", "GET", "api/orders/{id}", "Unsecured"),
                ("ListOrders", "GET", "api/orders", "Secured")
            })
            {
                facts.Add(Endpoint(FactKinds.OperationVerb, "", operation, verb));
                facts.Add(Endpoint(FactKinds.OperationRoute, "", operation, route));
                facts.Add(Endpoint(FactKinds.Security, "", operation, security));
            }

            return facts;
        }

        private static string FastEndpointsRecipe => RecipeFile(ServicesGuides.FastEndpoints);

        [Fact]
        public void The_FastEndpoints_recipe_file_reads_every_form_with_no_drift()
        {
            Check(Services, FastEndpointsRecipe, FastEndpointsModel(), FastEndpoints).ShouldBeClean();
        }

        [Fact]
        public void The_FastEndpoints_recipe_file_validates_and_reads_values_where_the_guide_says()
        {
            var validation = ShouldValidate(Services, FastEndpointsRecipe, FastEndpointsModel(), FastEndpoints);

            validation.Sources(FactKinds.OperationRoute).ShouldBe((3, 0, 0));
            validation.Sources(FactKinds.Security).ShouldBe((1, 2, 0));
            validation.Sources(FactKinds.ParameterSource).ShouldBe((4, 1, 0));
        }

        [Theory]
        [InlineData("src/Acme.Api/Orders/CreateOrderEndpoint.cs", "Post(\"/api/orders\")", "Put(\"/api/orders\")",
            "`CreateOrderCommand` verb: model `POST`, code `PUT`")]
        [InlineData("src/Acme.Api/Orders/CreateOrderEndpoint.cs", "Post(\"/api/orders\")", "Post(\"/api/orders/new\")",
            "`CreateOrderCommand` route: model `api/orders`, code `api/orders/new`")]
        [InlineData("src/Acme.Api/Orders/GetOrderEndpoint.cs", "\n        AllowAnonymous();", "",
            "`GetOrderQuery` security: model `Unsecured`, code `Secured`")]
        [InlineData("src/Acme.Api/Orders/CreateOrderEndpoint.cs", "Roles(\"Admin\", \"Sales\")", "Roles(\"Admin\")",
            "`CreateOrderCommand` roles: model `Admin,Sales`, code `Admin`")]
        [InlineData("src/Acme.Api/Orders/ListOrdersEndpoint.cs", "Policies(\"CanList\")", "Policies(\"CanRead\")",
            "`ListOrders` policy: model `CanList`, code `CanRead`")]
        [InlineData("src/Acme.Api/Orders/ListOrdersEndpoint.cs", "Verbs(Http.GET)", "Verbs(Http.POST)",
            "`ListOrders` verb: model `GET`, code `POST`")]
        [InlineData("src/Acme.Api/Orders/Requests.cs", "[FromHeader(\"X-Tenant\")]", "",
            "`CreateOrderCommand(Tenant)` binding: model `From Header`, code `From Body`")]
        [InlineData("src/Acme.Api/Orders/GetOrderEndpoint.cs", "Get(\"/api/orders/{id}\")", "Post(\"/api/orders/{id}\")",
            "`GetOrderQuery` verb: model `GET`, code `POST`", "`GetOrderQuery(Expand)` binding: model `From Query`, code `From Body`")]
        public void A_FastEndpoints_change_is_reported_as_its_rows(string path, string from, string to, params string[] rows)
        {
            Check(Services, FastEndpointsRecipe, FastEndpointsModel(), Mutate(FastEndpoints, path, from, to)).ShouldReport(rows);
        }

        [Fact]
        public void An_unmodelled_FastEndpoints_endpoint_is_reported()
        {
            var files = new Dictionary<string, string>(FastEndpoints)
            {
                ["src/Acme.Api/Orders/CancelOrderEndpoint.cs"] = """
                    public class CancelOrderEndpoint : Endpoint<CancelOrderCommand>
                    {
                        public override void Configure()
                        {
                            Post("/api/orders/{id}/cancel");
                        }
                    }
                    """
            };

            Check(Services, FastEndpointsRecipe, FastEndpointsModel(), files).ShouldReport("endpoint `CancelOrderEndpoint`: not modelled");
        }

        [Fact]
        public void A_FastEndpoints_route_in_a_group_can_not_be_verified()
        {
            var files = Mutate(FastEndpoints, "src/Acme.Api/Orders/CreateOrderEndpoint.cs", "Post(\"/api/orders\");", "Post(\"/\");\n        Group<OrdersGroup>();");

            Check(Services, FastEndpointsRecipe, FastEndpointsModel(), files).Wrong(DriftSection.Unverifiable).ShouldHaveSingleItem()
                .ShouldStartWith("`CreateOrderCommand` route: stated in a form the recipe can't read: `Group<OrdersGroup>()`");
        }
    }
}
