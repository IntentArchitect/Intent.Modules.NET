using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Drift;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// A fact about a type or member the Software Factory is adding this run, or about a method whose parameters it is
    /// changing, is read from the code as it will be once those changes are applied. The pending change itself is never
    /// a row; a gap that remains, such as an action with no route, is. Every other fact is read from the disk.
    /// </summary>
    public class PendingFactTests
    {
        private const string Path = "src/Api/Controllers/MillsController.cs";

        private const string Recipe = """
        {
          "files": "src/Api/**/*.cs",
          "facts": {
            "service.route": {
              "find": "class:{Service}",
              "read": [{ "part": "declaration", "match": "\\[Route\\(\"([^\"]*)\"" }],
              "replace": [{ "match": "\\[controller\\]", "with": "{Service|trimEnd:Controller}" }],
              "default": "",
              "unstated": "no [Route] on the controller"
            },
            "operation.route": {
              "find": "class:{Service} > method:{Operation}",
              "read": [{ "part": "declaration", "match": "\\[Http\\w+\\(\"([^\"]*)\"" }],
              "default": "",
              "unstated": "no route template on the action"
            },
            "parameter.source": {
              "find": "class:{Service} > method:{Operation} > parameter:{Parameter}",
              "read": [
                { "match": "\\[From(Body|Route|Query)\\b", "value": "From $1" },
                { "in": "method", "part": "declaration", "match": "(?i)\\{{Parameter}(:[^}]*)?\\??\\}", "value": "From Route" },
                { "match": "^(?:string|int|long|bool|Guid)\\??\\s", "value": "From Query" }
              ],
              "default": "From Body"
            }
          },
          "own": [
            { "kind": "endpoint", "find": "class with \\[ApiController\\] > method with \\[Http(Get|Post|Put|Delete|Patch)\\b", "name": "{Class}.{Method}" }
          ]
        }
        """;

        private const string Create = "public IActionResult Create(CreateMillDto dto) { }";
        private const string GetMill = "public IActionResult GetMill(int id) { }";
        private const string Ping = "public IActionResult Ping() { }";

        private static readonly Dictionary<string, string> Ids = new()
        {
            ["MillsController"] = "mills-id",
            ["MillsController.Create"] = "create-id",
            ["MillsController.GetMill"] = "getmill-id",
            ["MillsController.Ping"] = "ping-id"
        };

        /// <summary>The controller on disk: line 9 is Create's <c>[HttpPost]</c>, 10 its declaration, 12 GetMill's <c>[HttpGet]</c>.</summary>
        private static string Controller(string extra = "") => $$"""
            using Microsoft.AspNetCore.Mvc;

            namespace Acme.Api.Controllers
            {
                [ApiController]
                [Route("api/[controller]")]
                public class MillsController : ControllerBase
                {
                    [HttpPost]
                    public IActionResult Create([FromQuery] CreateMillDto dto) => Ok();

                    [HttpGet("{id}")]
                    public IActionResult GetMill(int id) => Ok();
            {{extra}}
                }
            }
            """;

        /// <summary>What the Services contract template writes for the controller.</summary>
        private static string Generated(params string[] members) =>
            $"namespace Acme.Api.Controllers\n{{\n    public class MillsController\n    {{\n{string.Join("\n", members.Select(x => $"        {x}"))}\n    }}\n}}\n";

        private static ModelFact Fact(FactKind kind, string element, string value, string operation = null, string parameter = null)
        {
            var placeholders = new Dictionary<string, string> { ["Service"] = "MillsController", ["Folder"] = string.Empty };
            var ids = new List<string> { "mills-id" };
            if (operation != null)
            {
                placeholders["Operation"] = operation;
                ids.Add(Ids[$"MillsController.{operation}"]);
            }

            if (parameter != null)
            {
                placeholders["Parameter"] = parameter;
                ids.Add($"{parameter}-id");
            }

            return new ModelFact { Kind = kind, Element = element, ElementIds = ids, Value = value, Placeholders = placeholders };
        }

        private static List<ModelFact> Facts(string createParameter = "dto", string createBinding = "From Query") => new()
        {
            Fact(FactKinds.ServiceRoute, "MillsController", "api/Mills"),
            Fact(FactKinds.OperationRoute, "MillsController.Create", "", "Create"),
            Fact(FactKinds.ParameterSource, $"MillsController.Create({createParameter})", createBinding, "Create", createParameter),
            Fact(FactKinds.OperationRoute, "MillsController.GetMill", "{id}", "GetMill"),
            Fact(FactKinds.ParameterSource, "MillsController.GetMill(id)", "From Route", "GetMill", "id")
        };

        private static ModelFact PingRoute => Fact(FactKinds.OperationRoute, "MillsController.Ping", "Ping", "Ping");

        private static DriftReport Run(string model, string code, ModelBaseline baseline, IReadOnlyList<ModelFact> facts)
        {
            var files = new Dictionary<string, string>();
            if (code != null)
            {
                files[Path] = code;
            }

            return DriftCheck.Run(new DriftCheckInput
            {
                Designer = "Services",
                Kinds = FactKinds.For(FactKinds.Services),
                RecipePath = "guardrails/services.drift.json",
                RecipeJson = Recipe,
                Code = new CodeFiles(files),
                Facts = facts,
                Generated = model == null ? Array.Empty<GeneratedFile>() : new[] { new GeneratedFile { RelativePath = Path, Text = model, ElementIds = Ids } },
                Application = "Acme",
                Baseline = baseline
            });
        }

        private static ModelBaseline Agreed() => Run(Generated(Create, GetMill), Controller(), baseline: null, Facts()).Baseline;

        [Fact]
        public void The_controller_agrees_with_the_model()
        {
            Run(Generated(Create, GetMill), Controller(), baseline: null, Facts()).WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void An_action_the_model_adds_is_checked_on_the_run_that_adds_it()
        {
            var model = Generated(Create, GetMill, Ping);
            var facts = Facts().Append(PingRoute).ToList();

            // The action is the Software Factory's to write, so it isn't a row; the route it has no template for is. It is
            // after the place the action is inserted, which isn't on disk yet, so the row points at the file alone. The
            // controller's other actions are still read from the disk, and its endpoint convention still finds them modelled.
            var staging = Run(model, Controller(), Agreed(), facts);
            staging.PendingCount.ShouldBe(1);
            staging.WrongLines.ShouldBe(new[]
            {
                $"`MillsController.Ping` route: model `Ping`, code (none) (no route template on the action). Location: `{Path}`"
            });
            staging.ToMarkdown().ShouldContain(
                $"| <FileRef path=\"{Path}\" /> | `MillsController.Ping` route: model `Ping`, code (none) (no route template on the action) |");

            // Applied: the same gap, now with its line, until the code is fixed.
            Run(model, Controller($"\n        {Ping}"), staging.Baseline, facts).WrongLines.ShouldBe(new[]
            {
                $"`MillsController.Ping` route: model `Ping`, code (none) (no route template on the action). Location: `{Path}:15`"
            });
        }

        [Fact]
        public void A_controller_the_model_adds_is_checked_on_the_run_that_adds_it()
        {
            var nothing = Run(model: null, code: null, baseline: null, new List<ModelFact>()).Baseline;

            var report = Run(Generated(Ping), code: null, nothing, new[] { Facts()[0], PingRoute });

            report.PendingCount.ShouldBe(1);
            report.WrongLines.ShouldBe(new[]
            {
                $"`MillsController.Ping` route: model `Ping`, code (none) (no route template on the action). Location: `{Path}`",
                $"`MillsController` route: model `api/Mills`, code (none) (no [Route] on the controller). Location: `{Path}`"
            });
            report.ToMarkdown().ShouldContain($"| <FileRef path=\"{Path}\" change=\"added\" /> | `MillsController` route: model `api/Mills`, code (none) (no [Route] on the controller) |");
        }

        [Fact]
        public void A_parameter_the_model_renames_is_read_with_its_new_name_and_without_its_attributes()
        {
            // On disk, Create has no parameter named command, so its binding couldn't be verified. RoslynWeaver matches
            // parameters by name, so applying the parameter list drops dto's [FromQuery]: command is bound from the body, as
            // modelled. Create, read from the view, still counts as modelled.
            var report = Run(Generated(Create.Replace("dto", "command"), GetMill), Controller(), Agreed(), Facts("command", "From Body"));

            report.PendingCount.ShouldBe(1);
            report.Wrong(DriftSection.Unverifiable).ShouldBeEmpty();
            report.WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void A_parameter_that_keeps_its_name_keeps_its_attributes()
        {
            // The model adds notify to Create. RoslynWeaver matches parameters by name, so dto keeps [FromQuery].
            var facts = Facts();
            facts.Add(Fact(FactKinds.ParameterSource, "MillsController.Create(notify)", "From Query", "Create", "notify"));

            var report = Run(Generated(Create.Replace("dto)", "dto, bool notify)"), GetMill), Controller(), Agreed(), facts);

            report.PendingCount.ShouldBe(1);
            report.WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void A_row_keeps_its_line_when_its_code_is_on_disk()
        {
            // Create's parameter list (line 10) is pending. A fact about Create read before it keeps its line, and so does
            // a fact about GetMill, which is read from the disk.
            var facts = Facts("command", "From Body");
            facts[1] = Fact(FactKinds.OperationRoute, "MillsController.Create", "new", "Create");
            facts[3] = Fact(FactKinds.OperationRoute, "MillsController.GetMill", "mills/{id}", "GetMill");

            var report = Run(Generated(Create.Replace("dto", "command"), GetMill), Controller(), Agreed(), facts);

            report.WrongLines.ShouldBe(new[]
            {
                $"`MillsController.Create` route: model `new`, code (none) (no route template on the action). Location: `{Path}:9`",
                $"`MillsController.GetMill` route: model `mills/{{id}}`, code `{{id}}`. Location: `{Path}:12`"
            });
        }
    }
}
