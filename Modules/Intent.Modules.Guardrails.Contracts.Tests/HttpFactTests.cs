using System.Collections.Generic;
using Intent.Modules.Common.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>HTTP and security facts read from ASP.NET Core controllers, with a recipe written as the AI would.</summary>
    public class HttpFactTests
    {
        private const string Recipe = """
        {
          "files": "src/Api/**/*.cs",
          "facts": {
            "service.route": {
              "find": "class:{Service}",
              "read": [{ "part": "declaration", "match": "\\[Route\\(\"([^\"]*)\"" }],
              "replace": [{ "match": "\\[controller\\]", "with": "{Service|trimEnd:Controller}" }],
              "default": ""
            },
            "operation.verb": {
              "find": "class:{Service} > method:{Operation}",
              "read": [{ "part": "declaration", "match": "\\[Http(Get|Post|Put|Delete|Patch)\\b", "case": "upper" }]
            },
            "operation.route": {
              "find": "class:{Service} > method:{Operation}",
              "read": [
                { "part": "declaration", "match": "\\[Route\\(\"([^\"]*)\"" },
                { "part": "declaration", "match": "\\[Http\\w+\\(\"([^\"]*)\"" }
              ],
              "replace": [{ "match": "\\[action\\]", "with": "{Operation}" }],
              "default": ""
            },
            "security": {
              "find": "class:{Service} > method:{Operation}",
              "read": [
                { "part": "declaration", "match": "\\[AllowAnonymous\\b", "value": "Unsecured" },
                { "part": "declaration", "match": "\\[(Authorize|Authorization)\\b", "value": "Secured" }
              ],
              "inherit": "class",
              "default": "None"
            },
            "security.roles": {
              "find": "class:{Service} > method:{Operation}",
              "read": [{ "part": "declaration", "match": "Roles=\"([^\"]*)\"" }],
              "replace": [{ "match": "\\s", "with": "" }],
              "inherit": "class"
            },
            "security.policy": {
              "find": "class:{Service} > method:{Operation}",
              "read": [{ "part": "declaration", "match": "Policy=\"([^\"]*)\"" }],
              "inherit": "class"
            },
            "parameter.source": {
              "find": "class:{Service} > method:{Operation} > parameter:{Parameter}",
              "read": [
                { "match": "\\[From(Body|Route|Query|Form|Header)\\b", "value": "From $1" },
                { "in": "method", "part": "declaration", "match": "(?i)\\{{Parameter}(:[^}]*)?\\??\\}", "value": "From Route" },
                { "match": "^(?:string|int|long|bool|decimal|double|DateTime|Guid)\\??\\s", "value": "From Query" }
              ],
              "default": "From Body"
            }
          },
          "own": [
            { "kind": "endpoint", "find": "class with \\[ApiController\\] > method with \\[Http(Get|Post|Put|Delete|Patch)\\b", "name": "{Class}.{Method}" }
          ]
        }
        """;

        private const string Controller = """
        using Microsoft.AspNetCore.Authorization;
        using Microsoft.AspNetCore.Mvc;

        namespace Acme.Api.Controllers
        {
            [Authorize]
            [ApiController]
            [Route("api/[controller]")]
            public class OrdersController : ControllerBase
            {
                [HttpGet("{id}")]
                public IActionResult GetOrder(int id) => Ok();

                [HttpGet("[action]")]
                public IActionResult Search([FromQuery] string text, int page) => Ok();

                [HttpPost]
                [AllowAnonymous]
                public IActionResult Create([FromBody] CreateOrderDto dto) => Ok();

                [HttpPut("{id}")]
                [Authorize(Roles = "Admin, Manager", Policy = "CanEdit")]
                public IActionResult Update(int id, UpdateOrderDto dto) => Ok();

                [Route("Archive/{OrderId}")]
                [HttpDelete]
                [Authorization]
                public IActionResult Archive(string orderId) => Ok();
            }
        }
        """;

        private static Dictionary<string, string> Code(string controller = Controller) => new() { ["src/Api/Controllers/OrdersController.cs"] = controller };

        private static List<ModelFact> Model() => new()
        {
            Fact(FactKinds.ServiceRoute, "OrdersController", "api/Orders", ("Service", "OrdersController")),
            Operation(FactKinds.OperationVerb, "OrdersController", "GetOrder", "GET", "id"),
            Operation(FactKinds.OperationRoute, "OrdersController", "GetOrder", "{id}", "id"),
            Operation(FactKinds.Security, "OrdersController", "GetOrder", "Secured", "id"),
            Parameter("OrdersController", "GetOrder", "id", "From Route"),
            Operation(FactKinds.OperationVerb, "OrdersController", "Search", "GET", "text", "page"),
            Operation(FactKinds.OperationRoute, "OrdersController", "Search", "Search", "text", "page"),
            Operation(FactKinds.Security, "OrdersController", "Search", "Secured", "text", "page"),
            Parameter("OrdersController", "Search", "text", "From Query"),
            Parameter("OrdersController", "Search", "page", "From Query"),
            Operation(FactKinds.OperationVerb, "OrdersController", "Create", "POST", "dto"),
            Operation(FactKinds.OperationRoute, "OrdersController", "Create", "", "dto"),
            Operation(FactKinds.Security, "OrdersController", "Create", "Unsecured", "dto"),
            Parameter("OrdersController", "Create", "dto", "From Body"),
            Operation(FactKinds.OperationVerb, "OrdersController", "Update", "PUT", "id", "dto"),
            Operation(FactKinds.OperationRoute, "OrdersController", "Update", "{id}", "id", "dto"),
            Operation(FactKinds.Security, "OrdersController", "Update", "Secured", "id", "dto"),
            Operation(FactKinds.SecurityRoles, "OrdersController", "Update", "Admin,Manager", "id", "dto"),
            Operation(FactKinds.SecurityPolicy, "OrdersController", "Update", "CanEdit", "id", "dto"),
            Parameter("OrdersController", "Update", "id", "From Route"),
            Parameter("OrdersController", "Update", "dto", "From Body"),
            Operation(FactKinds.OperationVerb, "OrdersController", "Archive", "DELETE", "orderId"),
            Operation(FactKinds.OperationRoute, "OrdersController", "Archive", "Archive/{OrderId}", "orderId"),
            Operation(FactKinds.Security, "OrdersController", "Archive", "Secured", "orderId"),
            Parameter("OrdersController", "Archive", "orderId", "From Route")
        };

        [Fact]
        public void A_model_that_agrees_with_the_code_has_no_drift()
        {
            var report = Check("Services", Recipe, Model(), Code());

            report.WrongLines.ShouldBeEmpty();
            report.NeedsAttention.ShouldBeFalse("a report with nothing to act on isn't written");
        }

        [Fact]
        public void A_class_route_with_a_controller_token_is_compared_with_the_service_route()
        {
            var model = Model();
            model[0] = Fact(FactKinds.ServiceRoute, "OrdersController", "v2/Orders", ("Service", "OrdersController"));

            var report = Check("Services", Recipe, model, Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`OrdersController` route: model `v2/Orders`, code `api/Orders`. Location: `src/Api/Controllers/OrdersController.cs:8`"
            });
        }

        [Fact]
        public void An_action_route_with_an_action_token_reads_the_operation_name()
        {
            var code = Controller.Replace("[HttpGet(\"[action]\")]", "[HttpGet(\"[action]/all\")]");

            var report = Check("Services", Recipe, Model(), Code(code));

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`OrdersController.Search` route: model `Search`, code `Search/all`. Location: `src/Api/Controllers/OrdersController.cs:14`"
            });
        }

        [Fact]
        public void A_changed_verb_contradicts_the_model()
        {
            var code = Controller.Replace("[HttpPut(\"{id}\")]", "[HttpPatch(\"{id}\")]");

            var report = Check("Services", Recipe, Model(), Code(code));

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`OrdersController.Update` verb: model `PUT`, code `PATCH`. Location: `src/Api/Controllers/OrdersController.cs:21`"
            });
        }

        [Fact]
        public void Explicit_and_inferred_binding_are_read()
        {
            var model = Model();
            model[model.FindIndex(x => x.Element == "OrdersController.Search(page)")] = Parameter("OrdersController", "Search", "page", "From Route");
            model[model.FindIndex(x => x.Element == "OrdersController.Search(text)")] = Parameter("OrdersController", "Search", "text", "From Body");

            var report = Check("Services", Recipe, model, Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`OrdersController.Search(page)` binding: model `From Route`, code `From Query`. Location: `src/Api/Controllers/OrdersController.cs:15`",
                "`OrdersController.Search(text)` binding: model `From Body`, code `From Query`. Location: `src/Api/Controllers/OrdersController.cs:15`"
            });
        }

        [Fact]
        public void A_route_parameter_is_matched_ignoring_case_when_the_recipe_says_so()
        {
            // Archive(string orderId) is bound from {OrderId}: ASP.NET matches route values ignoring case.
            var report = Check("Services", Recipe, Model(), Code());

            report.WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void Security_is_inherited_from_the_class_and_overridden_by_an_action()
        {
            var code = Controller
                .Replace("        [Authorize]\n        [ApiController]", "        [ApiController]")
                .Replace("    [Authorize]\r\n    [ApiController]", "    [ApiController]")
                .Replace("    [Authorize]\n    [ApiController]", "    [ApiController]");

            var report = Check("Services", Recipe, Model(), Code(code));

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`OrdersController.GetOrder` security: model `Secured`, code `None` (the code doesn't state it). Location: `src/Api/Controllers/OrdersController.cs:10`",
                "`OrdersController.Search` security: model `Secured`, code `None` (the code doesn't state it). Location: `src/Api/Controllers/OrdersController.cs:13`"
            });
        }

        [Fact]
        public void An_authorization_attribute_on_an_action_secures_it()
        {
            var model = Model();
            model[model.FindIndex(x => x.Element == "OrdersController.Archive" && x.Kind == FactKinds.Security)] =
                Operation(FactKinds.Security, "OrdersController", "Archive", "Unsecured", "orderId");

            var report = Check("Services", Recipe, model, Code());

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`OrdersController.Archive` security: model `Unsecured`, code `Secured`. Location: `src/Api/Controllers/OrdersController.cs:27`"
            });
        }

        [Fact]
        public void Roles_and_policy_are_compared()
        {
            var code = Controller.Replace("Roles = \"Admin, Manager\", Policy = \"CanEdit\"", "Roles = \"Admin\", Policy = \"CanDelete\"");

            var report = Check("Services", Recipe, Model(), Code(code));

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`OrdersController.Update` policy: model `CanEdit`, code `CanDelete`. Location: `src/Api/Controllers/OrdersController.cs:22`",
                "`OrdersController.Update` roles: model `Admin,Manager`, code `Admin`. Location: `src/Api/Controllers/OrdersController.cs:22`"
            });
        }

        [Fact]
        public void An_unmodelled_action_is_code_the_model_does_not_describe()
        {
            var code = Controller.Replace("    }\n}", "        [HttpGet(\"count\")]\n        public IActionResult Count() => Ok();\n    }\n}")
                .Replace("    }\r\n}", "        [HttpGet(\"count\")]\r\n        public IActionResult Count() => Ok();\r\n    }\r\n}");

            var report = Check("Services", Recipe, Model(), Code(code));

            report.Wrong(DriftSection.Undescribed).ShouldBe(new[]
            {
                "endpoint `OrdersController.Count`: not modelled. Location: `src/Api/Controllers/OrdersController.cs:29`"
            });
        }
    }
}
