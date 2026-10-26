using System.Collections.Generic;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>Code that the recipes own must be modelled, unless an exception covers it.</summary>
    public class OwnershipTests
    {
        private const string Controllers = """
        [ApiController]
        public class OrdersController : ControllerBase
        {
            [HttpGet("{id}")]
            public IActionResult GetOrder(int id) => Ok();

            [HttpGet("count")]
            public IActionResult Count() => Ok();
        }

        [ApiController]
        public class HealthController : ControllerBase
        {
            [HttpGet]
            public IActionResult Ping() => Ok();
        }
        """;

        private const string Context = """
        public class AppDbContext : DbContext
        {
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Order>(entity => entity.ToTable("Orders"));
                modelBuilder.Entity<AuditEntry>(entity => entity.ToTable("Audit"));
            }
        }
        """;

        private static readonly Dictionary<string, string> Code = new()
        {
            ["src/Api/Controllers.cs"] = Controllers,
            ["src/Infrastructure/AppDbContext.cs"] = Context
        };

        private static string ServicesRecipe(string exceptions = "") => $$"""
        {
          "files": "src/Api/**/*.cs",
          "facts": {
            "operation.verb": {
              "find": "class:{Service} > method:{Operation}",
              "read": [{ "part": "declaration", "match": "\\[Http(Get|Post)\\b", "case": "upper" }]
            }
          },
          "own": [
            { "kind": "endpoint", "find": "class with \\[ApiController\\] > method with \\[Http\\w+\\b", "name": "{Class}.{Method}" }
          ],
          "exceptions": [{{exceptions}}]
        }
        """;

        private static string DomainRecipe(string exceptions = "") => $$"""
        {
          "files": "src/Infrastructure/**/*.cs",
          "facts": {
            "table.name": {
              "find": "statement:Entity<{Entity}>",
              "read": [{ "match": "ToTable\\(\"([^\"]+)\"" }]
            }
          },
          "own": [
            { "kind": "entity configuration", "find": "statement with modelBuilder\\.Entity<(\\w+)>", "name": "$1" }
          ],
          "exceptions": [{{exceptions}}]
        }
        """;

        private static readonly List<ModelFact> ServicesModel = new() { Operation(FactKinds.OperationVerb, "OrdersController", "GetOrder", "GET", "id") };

        private static readonly List<ModelFact> DomainModel = new() { Table(FactKinds.TableName, "Order", "Orders") };

        [Fact]
        public void Unmodelled_owned_code_is_reported()
        {
            var services = Check("Services", ServicesRecipe(), ServicesModel, Code);
            services.Wrong(DriftSection.Undescribed).ShouldBe(new[]
            {
                "endpoint `HealthController.Ping`: not modelled. Location: `src/Api/Controllers.cs:14`",
                "endpoint `OrdersController.Count`: not modelled. Location: `src/Api/Controllers.cs:7`"
            });

            var domain = Check("Domain", DomainRecipe(), DomainModel, Code);
            domain.Wrong(DriftSection.Undescribed).ShouldBe(new[]
            {
                "entity configuration `AuditEntry`: not modelled. Location: `src/Infrastructure/AppDbContext.cs:6`"
            });
        }

        [Fact]
        public void Exceptions_cover_a_type_or_a_member_and_a_stale_one_is_listed()
        {
            var services = Check("Services", ServicesRecipe("""
                { "match": "HealthController", "reason": "load-balancer probe" },
                "OrdersController.Count",
                "OrdersController.Gone"
                """), ServicesModel, Code);

            services.WrongLines.ShouldBeEmpty();
            services.UnusedExceptions.ShouldBe(new[] { "`OrdersController.Gone`" });
            services.ToMarkdown().ShouldBe("# Services drift report\n\n## Warnings\n\n| Warning | Detail |\n| --- | --- |\n| Exception matches nothing | `OrdersController.Gone`: remove it from `guardrails/services.drift.json`. |\n");
        }

        [Fact]
        public void Exceptions_cover_a_file_pattern_or_an_ownership_name()
        {
            var byFile = Check("Services", ServicesRecipe("\"src/Api/**/*.cs\""), ServicesModel, Code);
            byFile.WrongLines.ShouldBeEmpty();
            byFile.UnusedExceptions.ShouldBeEmpty();

            var byName = Check("Domain", DomainRecipe("\"AuditEntry\""), DomainModel, Code);
            byName.WrongLines.ShouldBeEmpty();
            byName.UnusedExceptions.ShouldBeEmpty();
        }

        [Fact]
        public void An_exception_never_covers_a_contradicted_or_unverifiable_fact()
        {
            var report = Check("Services", ServicesRecipe("\"src/Api/**/*.cs\", \"OrdersController\""), new List<ModelFact>
            {
                Operation(FactKinds.OperationVerb, "OrdersController", "GetOrder", "POST", "id"),
                Operation(FactKinds.OperationVerb, "OrdersController", "Cancel", "POST")
            }, Code);

            report.Wrong(DriftSection.Contradicted).ShouldHaveSingleItem();
            report.Wrong(DriftSection.Unverifiable).ShouldHaveSingleItem();
        }

        [Fact]
        public void A_type_or_member_exception_does_not_cover_code_a_convention_finds_inside_a_member()
        {
            // C8b: every minimal API endpoint is mapped inside one method, so exempting that method, or its type, must not
            // exempt the endpoints. Only their own name or a file pattern does.
            const string endpoints = """
            public static class MillsEndpoints
            {
                public static void MapMillsEndpoints(IEndpointRouteBuilder app)
                {
                    var group = app.MapGroup("v1/Mills");
                    group.MapGet("Ping", Ping);
                    group.MapDelete("Cache", ClearCache).AllowAnonymous();
                }

                private static string Ping() => "pong";

                private static void ClearCache()
                {
                }
            }
            """;
            static string Recipe(string exceptions) => $$"""
            {
              "files": "src/Api/**/*.cs",
              "facts": {
                "operation.route": {
                  "find": "statement with \\.Map\\w+\\(\"[^\"]*\",{Operation}\\)",
                  "read": [{ "match": "\\.Map\\w+\\(\"([^\"]*)\"" }]
                }
              },
              "own": [{ "kind": "endpoint (minimal API)", "find": "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"(\\w+)\"", "name": "$1" }],
              "exceptions": [{{exceptions}}]
            }
            """;
            var code = new Dictionary<string, string> { ["src/Api/MillsEndpoints.cs"] = endpoints };
            var model = new List<ModelFact> { Operation(FactKinds.OperationRoute, "MillsEndpoints", "Ping", "Ping") };

            var byDeclaration = Check("Services", Recipe("\"MillsEndpoints.MapMillsEndpoints\", \"MillsEndpoints\""), model, code);
            byDeclaration.WrongLines.ShouldBe(new[] { "endpoint (minimal API) `Cache`: not modelled. Location: `src/Api/MillsEndpoints.cs:7`" });
            byDeclaration.UnusedExceptions.ShouldBe(new[] { "`MillsEndpoints.MapMillsEndpoints`", "`MillsEndpoints`" });

            var byName = Check("Services", Recipe("\"Cache\""), model, code);
            byName.WrongLines.ShouldBeEmpty();
            byName.UnusedExceptions.ShouldBeEmpty();
        }
    }
}
