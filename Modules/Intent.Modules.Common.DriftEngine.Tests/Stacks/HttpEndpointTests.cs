using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// How a controller's and a Spring controller's code state an endpoint's security and whether an Operation is an endpoint,
/// beyond the Minimal API fixtures of the acceptance cases: an action's <c>[AllowAnonymous]</c> under a secured controller,
/// roles a constant names on a Web API 2 controller's own attribute deriving from <c>AuthorizeAttribute</c>, a public method
/// the model doesn't give Http Settings, and a Spring controller's roles stated on its class.
/// </summary>
public class HttpEndpointTests
{
    private static readonly JsonNode Model = JsonNode.Parse("""
        {
          "designers": {
            "Services": {
              "packages": [{
                "id": "services", "name": "Shop.Api",
                "elements": [
                  { "id": "orders", "type": "Service", "name": "OrdersController", "stereotypes": { "Secured": { "Roles": "Reader" } },
                    "children": [
                      { "id": "get", "type": "Operation", "name": "Get", "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{id}" } },
                        "children": [ { "id": "get-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } } ] },
                      { "id": "ping", "type": "Operation", "name": "Ping", "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "ping" }, "Unsecured": {} } },
                      { "id": "helper", "type": "Operation", "name": "Helper" },
                      { "id": "internal", "type": "Operation", "name": "Internal" }
                    ] }
                ]
              }]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
            }
          }
        }
        """)!;

    private const string Roles = """
        namespace Shop.Api;

        public static class AuthorizationRoles
        {
            public const string Reader = "Reader";
        }

        public class ShopAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
        {
        }
        """;

    private const string Controller = """
        using System.Web.Http;

        namespace Shop.Api;

        [ShopAuthorize(Roles = AuthorizationRoles.Reader)]
        public class OrdersController : ApiController
        {
            [HttpGet]
            [Route("{id}")]
            public IHttpActionResult Get(int id) => Ok(id);

            [HttpGet]
            [Route("ping")]
            [AllowAnonymous]
            public IHttpActionResult Ping() => Ok();

            public IHttpActionResult Helper() => Ok();

            [NonAction]
            public IHttpActionResult Internal() => Ok();
        }
        """;

    private const string SpringController = """
        package com.shop.api;

        import org.springframework.security.access.prepost.PreAuthorize;
        import org.springframework.web.bind.annotation.*;

        @RestController
        @PreAuthorize("hasAnyRole('ROLE_Reader')")
        public class OrdersController {
            @GetMapping("/{id}")
            public int get(@PathVariable int id) { return id; }

            @GetMapping("/ping")
            @PreAuthorize("permitAll()")
            public void ping() { }

            @GetMapping("/helper")
            public void helper() { }

            public void internal() { }
        }
        """;

    [Fact]
    public void A_controller_secures_its_actions_with_its_own_attribute_and_an_action_opts_out()
    {
        var result = Run(("src/Shop.Api/Roles.cs", Roles), ("src/Shop.Api/OrdersController.cs", Controller));
        Claim(result, "security", "get").ShouldSatisfyAllConditions(x => x.Outcome.ShouldBe("verified"), x => x.Reading!.Value.ShouldBe("Secured"));
        Claim(result, "security.roles", "get").ShouldSatisfyAllConditions(x => x.Outcome.ShouldBe("verified"), x => x.Reading!.Value.ShouldBe("Reader"));
        Claim(result, "security", "ping").ShouldSatisfyAllConditions(x => x.Outcome.ShouldBe("verified"), x => x.Reading!.Value.ShouldBe("Unsecured"));
        result.Claims.ShouldNotContain(x => x.Kind == "security.roles" && x.Element == "ping");
    }

    [Fact]
    public void A_public_method_is_an_action_unless_it_is_NonAction()
    {
        var result = Run(("src/Shop.Api/Roles.cs", Roles), ("src/Shop.Api/OrdersController.cs", Controller));
        Claim(result, "endpoint.exists", "get").Outcome.ShouldBe("verified");
        Claim(result, "endpoint.exists", "helper").ShouldSatisfyAllConditions(x => x.Outcome.ShouldBe("contradicted"), x => x.Reading!.Value.ShouldBe("present"));
        Claim(result, "endpoint.exists", "internal").ShouldSatisfyAllConditions(x => x.Outcome.ShouldBe("verified"), x => x.Expected.ShouldBe("absent"));
    }

    [Fact]
    public void A_derived_attribute_naming_no_roles_may_set_them_itself()
    {
        var result = Run(("src/Shop.Api/Roles.cs", Roles), ("src/Shop.Api/OrdersController.cs", Controller.Replace("[ShopAuthorize(Roles = AuthorizationRoles.Reader)]", "[ShopAuthorize]", StringComparison.Ordinal)));
        Claim(result, "security", "get").Outcome.ShouldBe("verified");
        Claim(result, "security.roles", "get").ShouldSatisfyAllConditions(x => x.Outcome.ShouldBe("cantVerify"), x => x.Reason.ShouldBe("unreadable"));
    }

    [Fact]
    public void A_Spring_handler_takes_its_class_security_unless_it_states_its_own()
    {
        var result = Run(("src/com/shop/api/OrdersController.java", SpringController));
        Claim(result, "security", "get").Reading!.Value.ShouldBe("Secured");
        Claim(result, "security.roles", "get").ShouldSatisfyAllConditions(x => x.Outcome.ShouldBe("verified"), x => x.Reading!.Value.ShouldBe("Reader"));
        Claim(result, "security", "ping").ShouldSatisfyAllConditions(x => x.Outcome.ShouldBe("verified"), x => x.Reading!.Value.ShouldBe("Unsecured"));
        Claim(result, "endpoint.exists", "helper").Outcome.ShouldBe("contradicted");
        Claim(result, "endpoint.exists", "internal").Outcome.ShouldBe("verified");
    }

    private static Claim Claim(RunResult result, string kind, string element) => result.Claims.Single(x => x.Kind == kind && x.Element == element);

    private static RunResult Run(params (string Path, string Source)[] files)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-http-").FullName;
        try
        {
            foreach (var (path, source) in files)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any())
                .Concat(new[] { "intent.aspnetcore", "intent.springmvc" }.Select(StockExtensions.Find).OfType<Intent.Modules.Common.DriftEngine.Extensions.IDriftExtension>())
                .ToList();
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(Model), root, [Pack.Stock("intent.services")!], extensions));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
