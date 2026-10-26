using System.Collections.Generic;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>A fact the check can't read is reported as wrong, never passed.</summary>
    public class UnverifiableTests
    {
        private const string Controller = """
        [Route("api/orders")]
        public class OrdersController
        {
            [HttpGet("{id}")]
            public IActionResult GetOrder(int id) => Ok();

            public IActionResult Helper() => Ok();

            [HttpGet]
            public IActionResult List() => Ok();

            [HttpGet("by-customer")]
            public IActionResult List(int customerId) => Ok();
        }
        """;

        private static readonly Dictionary<string, string> Code = new() { ["src/OrdersController.cs"] = Controller };

        private const string Recipe = """
        {
          "facts": {
            "operation.verb": {
              "find": "class:{Service} > method:{Operation}",
              "read": [{ "part": "declaration", "match": "\\[Http(Get|Post)\\b", "case": "upper" }]
            }
          }
        }
        """;

        [Fact]
        public void Code_the_find_does_not_locate_cannot_be_verified()
        {
            var report = Check("Services", Recipe, new List<ModelFact> { Operation(FactKinds.OperationVerb, "OrdersController", "Cancel", "POST") }, Code);

            report.Wrong(DriftSection.Unverifiable).ShouldBe(new[]
            {
                "`OrdersController.Cancel` verb: no code found (find `class:OrdersController > method:Cancel`)"
            });
        }

        [Fact]
        public void A_pattern_that_matches_nothing_without_a_default_cannot_be_verified()
        {
            var report = Check("Services", Recipe, new List<ModelFact> { Operation(FactKinds.OperationVerb, "OrdersController", "Helper", "GET") }, Code);

            report.Wrong(DriftSection.Unverifiable).ShouldBe(new[]
            {
                "`OrdersController.Helper` verb: no read case matches, and the recipe has no default. Location: `src/OrdersController.cs:7`"
            });

            // The row's Code file column shows the location, so its text doesn't repeat it.
            report.ToMarkdown().ShouldContain(
                "| `OrdersController.Helper` | <FileRef path=\"src/OrdersController.cs\" line={7} /> | `OrdersController.Helper` verb can't be verified: no read case matches, and the recipe has no default |");
        }

        [Fact]
        public void A_problem_in_the_recipe_file_links_the_recipe_file()
        {
            var report = DriftCheck.Run(new DriftCheckInput
            {
                Designer = "Services",
                RecipePath = "guardrails/services.drift.json",
                RecipeFile = "D:/Acme/intent/Acme/guardrails/services.drift.json",
                RecipeJson = "{ \"facts\": ",
                Code = new CodeFiles(Code),
                Facts = new List<ModelFact>(),
                Generated = new GeneratedFile[0]
            });

            report.ToMarkdown().ShouldContain("| — | <FileRef path=\"D:/Acme/intent/Acme/guardrails/services.drift.json\" /> | The recipe file is wrong: `guardrails/services.drift.json` isn't valid JSON:");
        }

        [Fact]
        public void A_find_that_matches_several_places_cannot_be_verified_unless_the_parameter_names_choose()
        {
            // Without parameter names (as for a Command or Query endpoint), two overloads are ambiguous.
            var ambiguous = Check("Services", Recipe, new List<ModelFact>
            {
                Fact(FactKinds.OperationVerb, "OrdersController.List", "GET", ("Service", "OrdersController"), ("Operation", "List"), ("Folder", ""))
            }, Code);
            ambiguous.Wrong(DriftSection.Unverifiable).ShouldBe(new[]
            {
                "`OrdersController.List` verb: find `class:OrdersController > method:List` matches 2 places (`src/OrdersController.cs:9`, `src/OrdersController.cs:12`); narrow the find"
            });

            var chosen = Check("Services", Recipe, new List<ModelFact> { Operation(FactKinds.OperationVerb, "OrdersController", "List", "GET", "customerId") }, Code);
            chosen.WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void A_missing_recipe_file_turns_the_check_off()
        {
            // A fact the code contradicts and a generated type missing from the code, and neither is reported.
            var report = Check("Services", null, new List<ModelFact> { Operation(FactKinds.OperationVerb, "OrdersController", "GetOrder", "DELETE") }, Code,
                new[] { new GeneratedFile { RelativePath = "src/Api/Missing.cs", Text = "public class Missing { }" } });

            report.IsOff.ShouldBeTrue();
            report.WrongLines.ShouldBeEmpty();
            report.ToMarkdown().ShouldBe("# Services drift report\n\n## Warnings\n\n| Warning | Detail |\n| --- | --- |\n| The check is off | `guardrails/services.drift.json` doesn't exist. Load the configure-drift-check skill to write it. |\n");
        }

        [Fact]
        public void A_fact_kind_without_a_recipe_is_not_checked_and_does_not_fail_the_run()
        {
            // A module upgrade that adds a kind of fact must not fail every run until the recipe file catches up.
            var report = Check("Services", Recipe, new List<ModelFact>
            {
                Operation(FactKinds.OperationRoute, "OrdersController", "GetOrder", "{id}"),
                Operation(FactKinds.OperationRoute, "OrdersController", "Helper", "")
            }, Code);

            report.WrongLines.ShouldBeEmpty();
            report.NotChecked.ShouldBe(new[] { "`operation.route` (2 facts)" });
            report.ToMarkdown().ShouldBe("# Services drift report\n\n## Warnings\n\n| Warning | Detail |\n| --- | --- |\n| No recipe | `operation.route` (2 facts): `guardrails/services.drift.json` has no recipe for it, so it isn't checked. Load the configure-drift-check skill to add one. |\n");
            report.NotCheckedMessage("report.md").ShouldBe(
                "The Services drift check doesn't check 1 kind of fact, which guardrails/services.drift.json has no recipe for: operation.route (2 facts). Load the configure-drift-check skill to add them. Report: report.md");
        }

        [Fact]
        public void A_recipe_file_that_is_not_JSON_is_reported()
        {
            var report = Check("Services", "{ \"facts\": ", new List<ModelFact> { Operation(FactKinds.OperationVerb, "OrdersController", "GetOrder", "GET") }, Code);

            report.Wrong(DriftSection.Unverifiable).ShouldHaveSingleItem().ShouldStartWith("`guardrails/services.drift.json` isn't valid JSON:");
            report.NotChecked.ShouldBe(new[] { "`operation.verb` (1 fact)" });
        }

        [Fact]
        public void Every_problem_in_a_recipe_is_reported_and_its_facts_are_not_checked_one_by_one()
        {
            const string recipe = """
            {
              "facts": {
                "operation.verb": {
                  "find": "class:{Service} > method:{Operation}",
                  "read": [{ "match": "[Http(Get" }]
                },
                "operation.route": {
                  "find": "kind:{Service}",
                  "read": [{ "match": "x" }]
                },
                "security": {
                  "find": "class:{Entity}",
                  "read": [{ "match": "x", "colour": "red" }],
                  "defualt": "None"
                },
                "security.roles": {
                  "find": "class:{Service} > method:{Operation}",
                  "read": [{ "match": "Roles=(\\w+)", "value": "$1", "unreadable": true }]
                },
                "column.textType": { "find": "class:X", "read": [{ "match": "x" }] }
              },
              "unexpected": true
            }
            """;
            var facts = new List<ModelFact>
            {
                Operation(FactKinds.OperationVerb, "OrdersController", "GetOrder", "GET"),
                Operation(FactKinds.OperationRoute, "OrdersController", "GetOrder", "{id}"),
                Operation(FactKinds.Security, "OrdersController", "GetOrder", "Secured")
            };

            var lines = Check("Services", recipe, facts, Code).Wrong(DriftSection.Unverifiable);

            lines.ShouldContain(x => x.StartsWith("`guardrails/services.drift.json` `operation.verb`: '[Http(Get' isn't a valid regular expression:"));
            lines.ShouldContain("`guardrails/services.drift.json` `operation.route`: find 'kind:{Service}': 'kind:{Service}' doesn't start with one of file, class, method, property, parameter, statement, chain");
            lines.ShouldContain("`guardrails/services.drift.json` `security`: {Entity} isn't available here (available: {Service}, {Operation}, {Folder}); write a literal brace as \\{");
            lines.ShouldContain("`guardrails/services.drift.json` `security`: a read case has an unknown property 'colour' (expected match, value, case, in, part, unreadable or note)");
            lines.ShouldContain("`guardrails/services.drift.json` `security.roles`: a read case can't have both unreadable and value: an unreadable case reads no value");
            lines.ShouldContain("`guardrails/services.drift.json` `security`: has an unknown property 'defualt' (expected find, read, default, absent, unstated, inherit, replace, case, files or note)");
            lines.ShouldContain(x => x.StartsWith("`guardrails/services.drift.json` `column.textType`: isn't a fact the Services designer states"));
            lines.ShouldContain("`guardrails/services.drift.json` has an unknown property 'unexpected' (expected files, facts, own, exceptions or note)");
            lines.ShouldNotContain(x => x.StartsWith("`OrdersController.GetOrder`"));
        }
    }
}
