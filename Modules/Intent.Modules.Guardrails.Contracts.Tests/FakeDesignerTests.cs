using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Drift.Templates;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The drift module knows no designer by name: a third designer, contributed the way the Guardrails modules contribute
    /// theirs, gets the recipe file, the check, the module task and the guidance from it with nothing but its own kinds,
    /// model reader and text.
    /// </summary>
    public sealed class FakeDesignerTests : IDisposable
    {
        private const string ApplicationId = "acme-app";

        private static readonly FactKind Label = new("ui.label", "Ui", "label",
            new[] { new FactPlaceholder("Component", "the component's name") },
            "the label exactly as modelled",
            "A component's `Label` → `Text`.",
            new[] { "Component" });

        private static readonly DriftDesigner Ui = new(
            Name: "Ui",
            DesignerId: "ui-designer",
            Order: 30,
            Kinds: new[] { Label },
            ReadFacts: (_, _) => new[] { SaveButtonLabel },
            ReadElements: (_, _) => new Dictionary<string, IElement>(),
            ValidateTaskId: "Acme.Ui.ValidateDriftRecipes",
            Guidance: new DriftGuidance(
                FactsText: "component labels",
                RuleRows: "| A component's label | Its `Label` |\n\n",
                GuideRows: "| Ui | Razor components | `.razor` files | [references/razor.md](references/razor.md) |\n\n",
                IntentLinks: "none",
                NewIntent: "nothing"));

        private static readonly ModelFact SaveButtonLabel = new()
        {
            Kind = Label,
            Element = "SaveButton",
            Value = "Save",
            Placeholders = new Dictionary<string, string> { ["Component"] = "SaveButton" }
        };

        private const string Recipe = """
        {
          "files": "src/**/*.cs",
          "facts": { "ui.label": { "find": "class:{Component}", "read": [{ "match": "Label=\"([^\"]*)\"" }] } }
        }
        """;

        private const string Code = "class SaveButton\n{\n    string Label = \"Store\";\n}\n";

        private readonly string _root = Path.Combine(Path.GetTempPath(), $"drift-fake-designer-{Guid.NewGuid():N}");

        public void Dispose()
        {
            if (Directory.Exists(_root))
            {
                Directory.Delete(_root, recursive: true);
            }
        }

        [Fact]
        public void Its_recipe_file_states_its_own_kinds_and_no_one_elses()
        {
            var recipes = RecipeFile.Parse("""{ "facts": { "table.name": { "find": "class:{Entity}" } } }""", Ui.Name, Ui.Kinds);

            recipes.Problems.ShouldHaveSingleItem().Message.ShouldBe("isn't a fact the Ui designer states (its facts: ui.label)");
        }

        [Fact]
        public void The_check_reports_its_drift()
        {
            var report = DriftCheck.Run(new DriftCheckInput
            {
                Designer = Ui.Name,
                Kinds = Ui.Kinds,
                RecipePath = "guardrails/ui.drift.json",
                RecipeJson = Recipe,
                Code = new CodeFiles(new Dictionary<string, string> { ["src/SaveButton.cs"] = Code }),
                Facts = new[] { SaveButtonLabel },
                Generated = Array.Empty<GeneratedFile>()
            });

            report.Designer.ShouldBe("Ui");
            report.WrongLines.ShouldHaveSingleItem().ShouldStartWith("`SaveButton` label: model `Save`, code `Store`");
        }

        [Fact]
        public void Its_module_task_validates_its_recipes()
        {
            Directory.CreateDirectory(Path.Combine(_root, "guardrails"));
            Directory.CreateDirectory(Path.Combine(_root, "src"));
            File.WriteAllText(Path.Combine(_root, "guardrails", "ui.drift.json"), Recipe);
            File.WriteAllText(Path.Combine(_root, "src", "SaveButton.cs"), Code);
            var config = Substitute.For<IApplicationConfig>();
            config.DirectoryPath.Returns(_root);
            config.OutputLocation.Returns(".");
            var solution = Substitute.For<ISolutionConfig>();
            solution.GetApplicationConfig(ApplicationId).Returns(config);
            var designer = Substitute.For<IDesigner>();
            var metadata = Substitute.For<IMetadataManager>();
            metadata.GetDesigner(ApplicationId, "ui-designer").Returns(designer);

            using var result = JsonDocument.Parse(RecipeValidationTask.Execute(Ui, solution, metadata, JsonSerializer.Serialize(new { applicationId = ApplicationId })));
            var label = result.RootElement.GetProperty("facts").EnumerateArray().ShouldHaveSingleItem();
            label.GetProperty("kind").GetString().ShouldBe("ui.label");
            label.GetProperty("facts").GetInt32().ShouldBe(1);
            label.GetProperty("contradicted").GetProperty("count").GetInt32().ShouldBe(1);

            RecipeValidationTask.Execute(Ui, solution, metadata, JsonSerializer.Serialize(new { applicationId = ApplicationId, fact = "table.name" }))
                .ShouldBe("""{"errorMessage":"No Ui fact is called table.name. They are: ui.label."}""");
        }

        [Fact]
        public void The_rule_and_skills_cover_it_and_no_other_designer()
        {
            var solution = Substitute.For<ISolutionConfig>();
            solution.SolutionRootLocation.Returns(Path.Combine(Path.GetTempPath(), "Acme", "intent"));
            var application = Substitute.For<IApplicationConfig>();
            application.Name.Returns("Acme");
            var context = Substitute.For<ISoftwareFactoryExecutionContext>();
            context.GetSolutionConfig().Returns(solution);
            context.GetApplicationConfig().Returns(application);
            var designers = new[] { Ui };

            var configure = ModelSyncGuidance.ConfigureDriftCheckSkill(context, designers);
            configure.ShouldContain("### Ui: `guardrails/ui.drift.json`");
            configure.ShouldContain("| `ui.label` | label | `{Component}` | the label exactly as modelled | A component's `Label` → `Text`. |");
            configure.ShouldContain("- `{Component}`: the component's name.");
            configure.ShouldContain("| Ui | Razor components | `.razor` files | [references/razor.md](references/razor.md) |");
            configure.ShouldContain("`intent/.cache/guardrails/Acme/UiDriftReport.mdx`");
            configure.ShouldNotContain("domain.drift.json");
            configure.ShouldNotContain("services.drift.json");

            ModelSyncGuidance.Rule(context, designers).ShouldContain("| A component's label | Its `Label` |");
            ModelSyncGuidance.Skill(context, designers).ShouldContain("(Ui: component labels)");
            ModelSyncGuidance.ValidateDriftRecipesSkill(context, designers).ShouldContain("| Ui | `guardrails/ui.drift.json` | `Acme.Ui.ValidateDriftRecipes` |");
        }
    }
}
