using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Intent.Modules.Domain.Guardrails.Contracts.Templates;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>The rule and skills the Guardrails modules write, rendered as the templates render them.</summary>
    public class GuidanceTests
    {
        [Fact]
        public void Every_JSON_example_in_configure_drift_check_is_a_valid_recipe()
        {
            var skill = Render(ModelSyncGuidance.ConfigureDriftCheckSkill(Context("Intent.Domain.Guardrails.Contracts", "Intent.Services.Guardrails.Contracts")));

            var examples = Regex.Matches(skill, "```json\n(.*?)\n```", RegexOptions.Singleline).Select(x => x.Groups[1].Value)
                .Where(x => !x.Contains("\"...\"") && !x.Contains("<fact>"))
                .ToList();
            examples.Count.ShouldBeGreaterThan(8);
            foreach (var example in examples)
            {
                var json = example.TrimStart().StartsWith("{")
                    ? example
                    : example.TrimStart().StartsWith("\"own\"") || example.TrimStart().StartsWith("\"exceptions\"") ? $"{{{example}}}" : $"{{\"facts\":{{{example}}}}}";
                var problems = new[] { FactKinds.Domain, FactKinds.Services }.Select(x => RecipeFile.Parse(json, x).Problems).ToList();
                problems.ShouldContain(x => x.Count == 0, $"Example isn't a valid recipe for either designer:\n{example}\n{string.Join("\n", problems.SelectMany(x => x).Select(x => $"{x.Subject}: {x.Message}"))}");
            }
        }

        [Fact]
        public void The_skill_lists_every_fact_of_each_installed_designer()
        {
            var both = Render(ModelSyncGuidance.ConfigureDriftCheckSkill(Context("Intent.Domain.Guardrails.Contracts", "Intent.Services.Guardrails.Contracts")));
            foreach (var kind in FactKinds.All)
            {
                both.ShouldContain($"| `{kind.Key}` | {kind.Label} |");
            }

            var servicesOnly = Render(ModelSyncGuidance.ConfigureDriftCheckSkill(Context("Intent.Services.Guardrails.Contracts")));
            servicesOnly.ShouldContain("`guardrails/services.drift.json`");
            servicesOnly.ShouldNotContain("| `column.textType` |");
            servicesOnly.ShouldContain("`intent/.cache/guardrails/Acme/ServicesDriftReport.mdx`");
            servicesOnly.ShouldNotContain("DomainDriftReport");
        }

        [Fact]
        public void The_rule_and_skills_render_as_written()
        {
            var context = Context("Intent.Domain.Guardrails.Contracts", "Intent.Services.Guardrails.Contracts");
            var documents = new Dictionary<string, string>
            {
                ["guardrails-model-sync.md"] = ModelSyncGuidance.Rule(context),
                ["sync-model.md"] = ModelSyncGuidance.Skill(context),
                ["configure-drift-check.md"] = ModelSyncGuidance.ConfigureDriftCheckSkill(context)
            };

            var folder = Path.Combine(Path.GetTempPath(), "guardrails-guidance");
            Directory.CreateDirectory(folder);
            foreach (var (name, markdown) in documents)
            {
                var rendered = Render(markdown);
                File.WriteAllText(Path.Combine(folder, name), rendered);
                rendered.ShouldStartWith("---\ndescription: ");
                rendered.ShouldNotContain("|| ");
                rendered.ShouldContain("`intent/.cache/guardrails/Acme/DomainDriftReport.mdx` and `intent/.cache/guardrails/Acme/ServicesDriftReport.mdx`, in the `.cache` folder beside the solution's `.isln` file");
                rendered.ShouldNotContain("root of the application's output");
                rendered.ShouldNotContain(".gitignore");
            }
        }

        [Fact]
        public void The_reverse_engineer_skills_name_their_reports_actual_path()
        {
            var context = Context("Intent.Domain.Guardrails.Contracts", "Intent.Services.Guardrails.Contracts");
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.ExecutionContext.Returns(context);
            outputTarget.Metadata.Returns(new Dictionary<string, object>());

            var domain = new Domain.Guardrails.Contracts.Templates.ReverseEngineerDomainSkill.ReverseEngineerDomainSkillTemplate(outputTarget).MarkdownFile.ToString();
            var services = new Services.Guardrails.Contracts.Templates.ReverseEngineerServicesSkill.ReverseEngineerServicesSkillTemplate(outputTarget).MarkdownFile.ToString();

            domain.ShouldContain("The adoption isn't finished until a run finds no drift and no warnings, so it writes no `intent/.cache/guardrails/Acme/DomainDriftReport.mdx`, and the user has approved the model changes");
            services.ShouldContain("The adoption isn't finished until a run finds no drift and no warnings, so it writes no `intent/.cache/guardrails/Acme/ServicesDriftReport.mdx`, and the user has approved the model changes");
            foreach (var skill in new[] { domain, services })
            {
                skill.ShouldNotContain("<<DriftReport>>");
                skill.ShouldNotContain(".gitignore");
                skill.ShouldNotContain("Recipe check");
            }
        }

        [Fact]
        public void The_validate_drift_recipes_skill_runs_each_installed_designers_task()
        {
            var context = Context("Intent.Domain.Guardrails.Contracts", "Intent.Services.Guardrails.Contracts");
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.ExecutionContext.Returns(context);
            outputTarget.Metadata.Returns(new Dictionary<string, object>());

            var both = new Domain.Guardrails.Contracts.Templates.ValidateDriftRecipesSkill.ValidateDriftRecipesSkillTemplate(outputTarget).MarkdownFile.ToString().Replace("\r\n", "\n");

            both.ShouldStartWith("---\ndescription: ");
            both.ShouldContain("| Domain | `guardrails/domain.drift.json` | `Intent.Domain.Guardrails.Contracts.ValidateDriftRecipes` |\n| Services | `guardrails/services.drift.json` | `Intent.Services.Guardrails.Contracts.ValidateDriftRecipes` |\n");
            both.ShouldContain("await executeModuleTask(");
            foreach (var key in new[] { "errorMessage", "recipeFileExists", "problems", "notChecked", "readFromCode", "fromDefault", "fromAbsent", "unverifiable", "contradicted", "readCasesMatchingNothing", "conventions", "found", "samples", "probeWarnings" })
            {
                both.ShouldContain($"`{key}`", customMessage: "the skill describes every field the task returns");
            }

            var servicesOnly = Render(ModelSyncGuidance.ValidateDriftRecipesSkill(Context("Intent.Services.Guardrails.Contracts")));
            servicesOnly.ShouldContain("`Intent.Services.Guardrails.Contracts.ValidateDriftRecipes`");
            servicesOnly.ShouldNotContain("Intent.Domain.Guardrails.Contracts.ValidateDriftRecipes");
        }

        [Fact]
        public void No_guidance_mentions_the_recipe_check_table_any_more()
        {
            var context = Context("Intent.Domain.Guardrails.Contracts", "Intent.Services.Guardrails.Contracts");
            foreach (var markdown in new[] { ModelSyncGuidance.Rule(context), ModelSyncGuidance.Skill(context), ModelSyncGuidance.ConfigureDriftCheckSkill(context) })
            {
                markdown.ShouldNotContain("Recipe check");
                markdown.ShouldContain("validate-drift-recipes");
            }
        }

        private static string Render(string markdown)
        {
            return new MarkdownFile("SKILL").FromMarkdown(markdown).ToString().Replace("\r\n", "\n");
        }

        private static ISoftwareFactoryExecutionContext Context(params string[] installedModules)
        {
            var modules = installedModules.Select(id =>
            {
                var module = Substitute.For<IIntentInstalledModule>();
                module.ModuleId.Returns(id);
                return module;
            }).ToList();
            var solution = Substitute.For<ISolutionConfig>();
            // A path as this OS writes it, as Intent Architect gives it: on the Linux CI agent a backslash isn't a separator.
            solution.SolutionRootLocation.Returns(Path.Combine(Path.GetPathRoot(Path.GetTempPath()), "Repos", "Acme", "intent") + Path.DirectorySeparatorChar);
            var application = Substitute.For<IApplicationConfig>();
            application.Name.Returns("Acme");
            var context = Substitute.For<ISoftwareFactoryExecutionContext>();
            context.InstalledModules.Returns(modules);
            context.GetSolutionConfig().Returns(solution);
            context.GetApplicationConfig().Returns(application);
            return context;
        }
    }
}
