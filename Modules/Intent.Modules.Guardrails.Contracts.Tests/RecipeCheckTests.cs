using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Intent.Modules.Common.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// A recipe file is validated on demand, by the ValidateDriftRecipes module task, from the recipe file, the model's
    /// facts and the code alone: the probe changes, in memory, the code each read case matched, and warns when what the
    /// recipe reads doesn't change with it; per kind of fact the validation counts where each value came from, and per
    /// convention what it finds, with samples. It returns data only, and the drift report no longer checks the recipes.
    /// </summary>
    public class RecipeCheckTests
    {
        private const string Controller = """
        [ApiController]
        [Route("api/orders")]
        [Authorize]
        public class OrdersController : ControllerBase
        {
            [HttpGet("{id}")]
            public IActionResult GetOrder(int id) => Ok();

            [HttpPost]
            [AllowAnonymous]
            public IActionResult Create(OrderDto order) => Ok();
        }
        """;

        private static readonly Dictionary<string, string> Code = new() { ["src/Api/OrdersController.cs"] = Controller };

        private const string Recipe = """
        {
          "files": "src/Api/**/*.cs",
          "facts": {
            "operation.verb": {
              "find": "class:{Service} > method:{Operation}",
              "read": [{ "part": "declaration", "match": "\\[Http(Get|Post)\\b", "case": "upper" }]
            },
            "operation.route": {
              "find": "class:{Service} > method:{Operation}",
              "read": [{ "part": "declaration", "match": "\\[Http\\w+\\(\"([^\"]*)\"" }],
              "default": "",
              "unstated": "no route template on the action"
            },
            "security": {
              "find": "class:{Service} > method:{Operation}",
              "read": [
                { "part": "declaration", "match": "\\[AllowAnonymous\\b", "value": "Unsecured" },
                { "part": "declaration", "match": "\\[Authorize\\b", "value": "Secured" }
              ],
              "inherit": "class",
              "default": "None",
              "unstated": "no `[Authorize]` or `[AllowAnonymous]` on the action or its controller"
            }
          }
        }
        """;

        private static readonly List<ModelFact> Model = new()
        {
            Operation(FactKinds.OperationVerb, "OrdersController", "GetOrder", "GET"),
            Operation(FactKinds.OperationVerb, "OrdersController", "Create", "POST"),
            Operation(FactKinds.OperationRoute, "OrdersController", "GetOrder", "{id}"),
            Operation(FactKinds.OperationRoute, "OrdersController", "Create", ""),
            Operation(FactKinds.Security, "OrdersController", "GetOrder", "Secured"),
            Operation(FactKinds.Security, "OrdersController", "Create", "Unsecured")
        };

        private static RecipeValidation Validate(string recipe, IReadOnlyList<ModelFact> facts, IReadOnlyDictionary<string, string> code = null, RecipeFilter filter = null, string designer = FactKinds.Services)
        {
            return RecipeValidation.Run(new RecipeValidationInput
            {
                Designer = designer,
                Kinds = FactKinds.For(designer),
                RecipePath = $"guardrails/{designer.ToLowerInvariant()}.drift.json",
                RecipeJson = recipe,
                Code = new CodeFiles(code ?? Code),
                Facts = facts,
                Filter = filter
            });
        }

        private static KindValidation Kind(RecipeValidation validation, FactKind kind) => validation.Facts.Single(x => x.Kind == kind.Key);

        [Fact]
        public void A_recipe_that_reads_the_code_passes_the_probe_and_shows_where_each_value_came_from()
        {
            // A redundant attribute on the controller, and a default equal to a case's value, still count as read.
            var validation = Validate(Recipe, Model);

            validation.RecipeFileExists.ShouldBeTrue();
            validation.Problems.ShouldBeEmpty();
            validation.ProbeWarnings.ShouldBeEmpty();
            // Each action's own attributes, and the controller's [Authorize] inherited by both actions.
            validation.WrongElementWarnings.ShouldBeEmpty();
            var route = Kind(validation, FactKinds.OperationRoute);
            route.Facts.ShouldBe(2);
            route.ReadFromCode.Count.ShouldBe(1);
            route.ReadFromCode.Samples.ShouldBe(new[] { new FactSample("OrdersController.GetOrder", "{id}", "{id}", "src/Api/OrdersController.cs:6", null, null) });
            route.FromDefault.Count.ShouldBe(1);
            route.FromDefault.Samples.Single().Location.ShouldBe("src/Api/OrdersController.cs:9");
            route.Contradicted.Count.ShouldBe(0);
            Kind(validation, FactKinds.OperationVerb).ReadFromCode.Count.ShouldBe(2);
            Kind(validation, FactKinds.Security).ReadFromCode.Count.ShouldBe(2);

            // Data for an AI to read: camel case, and nothing it doesn't have.
            var json = JsonDocument.Parse(validation.ToJson()).RootElement;
            json.GetProperty("recipePath").GetString().ShouldBe("guardrails/services.drift.json");
            json.GetProperty("facts")[0].GetProperty("readFromCode").GetProperty("samples")[0].GetProperty("location").GetString().ShouldNotBeNull();
            json.GetProperty("facts")[0].GetProperty("readFromCode").GetProperty("samples")[0].TryGetProperty("problem", out _).ShouldBeFalse();
        }

        [Fact]
        public void A_read_case_whose_reading_does_not_change_with_the_code_is_warned_about()
        {
            // The case reads GET wherever an action has an Http attribute, whatever its verb.
            var broken = Recipe.Replace("""{ "part": "declaration", "match": "\\[Http(Get|Post)\\b", "case": "upper" }""", """{ "part": "declaration", "match": "\\[Http", "value": "GET" }""");
            var getOnly = new List<ModelFact> { Operation(FactKinds.OperationVerb, "OrdersController", "GetOrder", "GET") };

            Validate(broken, getOnly).ProbeWarnings.ShouldBe(new[]
            {
                "`operation.verb` (read case 1): changing `[Http` at `src/Api/OrdersController.cs:6` doesn't change what the recipe reads"
            });
            Validate(Recipe, getOnly).ProbeWarnings.ShouldBeEmpty();
        }

        [Fact]
        public void A_case_that_reads_the_whole_declaration_instead_of_its_value_is_warned_about_and_contradicts_the_model()
        {
            // The route's capture removed: the case reads the attribute up to the template's closing quote.
            var broken = Recipe.Replace("""([^\"]*)""", """[^\"]*""");

            var validation = Validate(broken, Model);

            validation.ProbeWarnings.ShouldContain("`operation.route` (read case 1): changing `[HttpGet(\"{id}\"` at `src/Api/OrdersController.cs:6` doesn't change what the recipe reads");
            var contradicted = Kind(validation, FactKinds.OperationRoute).Contradicted;
            contradicted.Count.ShouldBe(1);
            contradicted.Samples.Single().Code.ShouldBe("[HttpGet(\"{id}\"");
        }

        [Fact]
        public void A_fact_read_from_a_default_is_counted_not_probed()
        {
            const string recipe = """
            {
              "files": "src/Api/**/*.cs",
              "facts": {
                "operation.route": { "find": "class:{Service} > method:{Operation}", "default": "", "unstated": "no route template on the action" }
              }
            }
            """;

            var validation = Validate(recipe, new List<ModelFact> { Operation(FactKinds.OperationRoute, "OrdersController", "Create", "") });

            validation.ProbeWarnings.ShouldBeEmpty();
            var route = Kind(validation, FactKinds.OperationRoute);
            route.ReadFromCode.Count.ShouldBe(0);
            route.FromDefault.Samples.Single().Location.ShouldBe("src/Api/OrdersController.cs:9");
        }

        [Fact]
        public void It_shows_read_cases_that_matched_nothing_and_what_each_convention_finds()
        {
            var code = new Dictionary<string, string>
            {
                ["src/Api/OrdersController.cs"] = Controller.Replace("    public IActionResult Create(OrderDto order) => Ok();", "    public IActionResult Create(OrderDto order) => Ok();\n\n    [HttpGet(\"ping\")]\n    public IActionResult Ping() => Ok();")
            };
            var recipe = Recipe
                .Replace("""{ "part": "declaration", "match": "\\[Authorize\\b", "value": "Secured" }""", """{ "part": "declaration", "match": "\\[Authorize\\b", "value": "Secured" }, { "match": "\\[RequireRole\\b", "value": "Secured" }""")
                .Replace("""  "facts": {""", """  "own": [{ "kind": "endpoint", "find": "class with \\[ApiController\\] > method with \\[Http\\w+", "name": "{Class}.{Method}" }], "facts": {""");

            var validation = Validate(recipe, Model, code);

            Kind(validation, FactKinds.Security).ReadCasesMatchingNothing.ShouldBe(new[] { "read case 3: \\[RequireRole\\b" });
            var endpoint = validation.Conventions.Single();
            (endpoint.Kind, endpoint.Found).ShouldBe(("endpoint", 3));
            endpoint.Samples.ShouldBe(new[]
            {
                "OrdersController.GetOrder (src/Api/OrdersController.cs:6)",
                "OrdersController.Create (src/Api/OrdersController.cs:9)",
                "OrdersController.Ping (src/Api/OrdersController.cs:13)"
            });
        }

        [Fact]
        public void A_fact_read_from_code_that_names_another_element_and_not_its_own_is_warned_about()
        {
            var code = new Dictionary<string, string>
            {
                ["src/AppDbContext.cs"] = """
                class AppDbContext
                {
                    void OnModelCreating(ModelBuilder modelBuilder)
                    {
                        modelBuilder.HasDefaultSchema("dbo");
                        modelBuilder.Entity<Order>(e =>
                        {
                            e.Property(x => x.Reference).HasMaxLength(20);
                            e.Property(x => x.Notes).HasMaxLength(500);
                            e.Property(x => x.Status);
                        });
                        modelBuilder.Entity<Customer>(e => e.Property(x => x.Name).HasMaxLength(100));
                    }
                }
                """
            };
            var facts = new List<ModelFact>
            {
                Column(FactKinds.ColumnMaxLength, "Order", "Reference", "20"),
                Column(FactKinds.ColumnMaxLength, "Order", "Notes", "500"),
                Column(FactKinds.ColumnMaxLength, "Order", "Status", ""),
                Column(FactKinds.ColumnMaxLength, "Customer", "Name", "100"),
                Table(FactKinds.TableSchema, "Order", "dbo"),
                Table(FactKinds.TableSchema, "Customer", "dbo")
            };
            // The configure-drift-check example finds each attribute's own chain. Without that, the case reads the first
            // HasMaxLength in the class's configuration for every attribute: Reference's, for Notes and for Status, which has
            // none of its own. The schema is one global setting that names no class, so sharing it is fine.
            const string recipe = """
            {
              "files": "src/**/*.cs",
              "facts": {
                "column.maxLength": {
                  "find": "statement:Entity<{Entity}> > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)",
                  "read": [{ "match": "HasMaxLength\\((\\d+)\\)" }],
                  "default": "",
                  "absent": ""
                },
                "table.schema": { "find": "statement:HasDefaultSchema", "read": [{ "match": "HasDefaultSchema\\(\"(\\w+)\"" }] }
              }
            }
            """;
            var unanchored = recipe.Replace(""" > chain with Property\\(\\w+=>\\w+\\.{Attribute}\\)""", "");

            var anchored = Validate(recipe, facts, code, designer: FactKinds.Domain);
            anchored.Problems.ShouldBeEmpty();
            anchored.WrongElementWarnings.ShouldBeEmpty();
            var validation = Validate(unanchored, facts, code, designer: FactKinds.Domain);

            validation.WrongElementWarnings.ShouldBe(new[]
            {
                "`column.maxLength` (read case 1): 2 facts read code that names another element and not their own, e.g. `Order.Notes` read `HasMaxLength(20)` at `src/AppDbContext.cs:8`, in code that names `Reference`"
            });
            // The probe can't tell: changing Reference's HasMaxLength(20) changes what Notes reads.
            validation.ProbeWarnings.ShouldBeEmpty();
        }

        [Fact]
        public void A_filter_lists_every_reading_it_selects_and_leaves_the_rest_out()
        {
            var byFact = Validate(Recipe, Model, filter: RecipeFilter.Of("operation.route", null, null));

            byFact.Facts.Select(x => x.Kind).ShouldBe(new[] { "operation.route" });
            byFact.Conventions.ShouldBeEmpty();
            byFact.Readings.ShouldBe(new[]
            {
                new ReadingDetail("operation.route", "OrdersController.GetOrder", "{id}", "{id}", "code", false, 1, 1, 0, "[HttpGet(\"{id}\"", "src/Api/OrdersController.cs:6", null, null),
                new ReadingDetail("operation.route", "OrdersController.Create", "", "", "default", false, 1, null, null, null, "src/Api/OrdersController.cs:9", null, null)
            });

            // An element selects its own facts and those of the elements it holds; security is inherited from the controller.
            var byElement = Validate(Recipe, Model, filter: RecipeFilter.Of(null, "OrdersController.GetOrder", null));
            byElement.Readings.Select(x => (x.Kind, x.From, x.Level, x.Matched)).ShouldBe(new (string, string, int?, string)[]
            {
                ("operation.route", "code", 0, "[HttpGet(\"{id}\""),
                ("operation.verb", "code", 0, "[HttpGet"),
                ("security", "code", 1, "[Authorize")
            });
            Validate(Recipe, Model, filter: RecipeFilter.Of(null, "OrdersController", null)).Readings.Count.ShouldBe(6);

            // A file by its path or the end of it.
            Validate(Recipe, Model, filter: RecipeFilter.Of(null, null, "OrdersController.cs")).Readings.Count.ShouldBe(6);
            Validate(Recipe, Model, filter: RecipeFilter.Of(null, null, "src/Api/Other.cs")).Readings.ShouldBeEmpty();

            // Without a filter, no readings; with one, the filter is echoed.
            JsonDocument.Parse(Validate(Recipe, Model).ToJson()).RootElement.TryGetProperty("readings", out _).ShouldBeFalse();
            var json = JsonDocument.Parse(byFact.ToJson()).RootElement;
            json.GetProperty("filter").GetProperty("fact").GetString().ShouldBe("operation.route");
            json.GetProperty("readings")[0].GetProperty("findPath").GetInt32().ShouldBe(1);
            RecipeFilter.Of(" ", null, "").ShouldBeNull();
        }

        [Fact]
        public void It_reports_a_missing_or_broken_recipe_file_and_kinds_of_fact_with_no_recipe()
        {
            Validate(null, Model).RecipeFileExists.ShouldBeFalse();
            Validate("{ not json", Model).Problems.ShouldNotBeEmpty();
            Validate("{}", Model).NotChecked.ShouldBe(new[] { "operation.route (2 facts)", "operation.verb (2 facts)", "security (2 facts)" });
        }
    }
}
