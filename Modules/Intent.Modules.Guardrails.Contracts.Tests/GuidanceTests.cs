using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Drift.Templates;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Intent.Modules.Services.Guardrails.Contracts.Drift;
using Intent.Templates;
using NSubstitute;
using DomainGuides = Intent.Modules.Domain.Guardrails.Contracts.Templates.DriftRecipeGuides;
using ServicesGuides = Intent.Modules.Services.Guardrails.Contracts.Templates.DriftRecipeGuides;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>The rule and skills the drift module writes for the Guardrails designers, rendered as the templates render them.</summary>
    public class GuidanceTests
    {
        private static readonly IReadOnlyList<DriftDesigner> Both = new[] { DomainDrift.Designer, ServicesDrift.Designer };
        private static readonly IReadOnlyList<DriftDesigner> DomainOnly = new[] { DomainDrift.Designer };
        private static readonly IReadOnlyList<DriftDesigner> ServicesOnly = new[] { ServicesDrift.Designer };

        [Fact]
        public void Every_JSON_example_in_configure_drift_check_is_a_valid_recipe()
        {
            var skill = Render(ModelSyncGuidance.ConfigureDriftCheckSkill(Context(), Both));

            var examples = Regex.Matches(skill, "```json\n(.*?)\n```", RegexOptions.Singleline).Select(x => x.Groups[1].Value)
                .Where(x => !x.Contains("\"...\"") && !x.Contains("<fact>"))
                .ToList();
            examples.Count.ShouldBeGreaterThan(2);
            foreach (var example in examples)
            {
                var json = example.TrimStart().StartsWith("{")
                    ? example
                    : example.TrimStart().StartsWith("\"own\"") || example.TrimStart().StartsWith("\"exceptions\"") ? $"{{{example}}}" : $"{{\"facts\":{{{example}}}}}";
                var problems = new[] { FactKinds.Domain, FactKinds.Services }.Select(x => RecipeFile.Parse(json, x, FactKinds.For(x)).Problems).ToList();
                problems.ShouldContain(x => x.Count == 0, $"Example isn't a valid recipe for either designer:\n{example}\n{string.Join("\n", problems.SelectMany(x => x).Select(x => $"{x.Subject}: {x.Message}"))}");
            }
        }

        [Fact]
        public void Every_JSON_block_in_the_technology_guides_is_a_valid_recipe_for_its_designer()
        {
            foreach (var (designer, guide) in new[]
            {
                (FactKinds.Domain, DomainGuides.EfCore), (FactKinds.Domain, DomainGuides.MongoDb), (FactKinds.Domain, DomainGuides.CosmosDb),
                (FactKinds.Domain, DomainGuides.DomainEvents),
                (FactKinds.Services, ServicesGuides.AspNetCoreControllers), (FactKinds.Services, ServicesGuides.MinimalApis), (FactKinds.Services, ServicesGuides.FastEndpoints),
                (FactKinds.Services, ServicesGuides.HandlerIntent)
            })
            {
                var blocks = Guides.JsonBlocks(guide);
                blocks.Count(x => x.TrimStart().StartsWith("{")).ShouldBe(1, "each guide has one recipe file");
                foreach (var block in blocks)
                {
                    var json = block.TrimStart().StartsWith("{") ? block : $"{{\"facts\":{{{block}}}}}";
                    var problems = RecipeFile.Parse(json, designer, FactKinds.For(designer)).Problems;
                    problems.ShouldBeEmpty($"{string.Join("\n", problems.Select(x => $"{x.Subject}: {x.Message}"))}\n{block}");
                }
            }
        }

        [Fact]
        public void The_skill_indexes_the_guides_of_each_contributed_designer()
        {
            var both = Render(ModelSyncGuidance.ConfigureDriftCheckSkill(Context(), Both));
            foreach (var guide in new[] { "ef-core", "mongodb", "cosmos-db", "domain-events", "aspnetcore-controllers", "minimal-apis", "fastendpoints", "handler-intent" })
            {
                both.ShouldContain($"[references/{guide}.md](references/{guide}.md)");
            }

            var servicesOnly = Render(ModelSyncGuidance.ConfigureDriftCheckSkill(Context(), ServicesOnly));
            servicesOnly.ShouldContain("[references/minimal-apis.md](references/minimal-apis.md)");
            servicesOnly.ShouldContain("[references/handler-intent.md](references/handler-intent.md)");
            servicesOnly.ShouldNotContain("references/ef-core.md");
            servicesOnly.ShouldNotContain("references/domain-events.md");
        }

        [Fact]
        public void Each_technology_guide_is_written_beside_the_configure_drift_check_skill()
        {
            var context = Context();
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.ExecutionContext.Returns(context);
            outputTarget.Metadata.Returns(new Dictionary<string, object>());
            var templates = new (string File, string Guide, MarkdownBaseTemplate<object> Template)[]
            {
                ("ef-core", DomainGuides.EfCore, new Domain.Guardrails.Contracts.Templates.EfCoreRecipeGuide.EfCoreRecipeGuideTemplate(outputTarget)),
                ("mongodb", DomainGuides.MongoDb, new Domain.Guardrails.Contracts.Templates.MongoDbRecipeGuide.MongoDbRecipeGuideTemplate(outputTarget)),
                ("cosmos-db", DomainGuides.CosmosDb, new Domain.Guardrails.Contracts.Templates.CosmosDbRecipeGuide.CosmosDbRecipeGuideTemplate(outputTarget)),
                ("domain-events", DomainGuides.DomainEvents, new Domain.Guardrails.Contracts.Templates.DomainEventsRecipeGuide.DomainEventsRecipeGuideTemplate(outputTarget)),
                ("aspnetcore-controllers", ServicesGuides.AspNetCoreControllers, new Services.Guardrails.Contracts.Templates.AspNetCoreControllersRecipeGuide.AspNetCoreControllersRecipeGuideTemplate(outputTarget)),
                ("minimal-apis", ServicesGuides.MinimalApis, new Services.Guardrails.Contracts.Templates.MinimalApisRecipeGuide.MinimalApisRecipeGuideTemplate(outputTarget)),
                ("fastendpoints", ServicesGuides.FastEndpoints, new Services.Guardrails.Contracts.Templates.FastEndpointsRecipeGuide.FastEndpointsRecipeGuideTemplate(outputTarget)),
                ("handler-intent", ServicesGuides.HandlerIntent, new Services.Guardrails.Contracts.Templates.HandlerIntentRecipeGuide.HandlerIntentRecipeGuideTemplate(outputTarget))
            };

            foreach (var (file, guide, template) in templates)
            {
                var config = template.GetTemplateFileConfig();
                config.FileName.ShouldBe(file);
                config.FileExtension.ShouldBe("md");
                config.LocationInProject.Replace('\\', '/').ShouldBe("configure-drift-check/references");
                template.MarkdownFile.ToString().Replace("\r\n", "\n").ShouldBe(Guides.Render(guide));
            }
        }

        [Fact]
        public void The_skill_lists_every_fact_of_each_contributed_designer()
        {
            var both = Render(ModelSyncGuidance.ConfigureDriftCheckSkill(Context(), Both));
            foreach (var kind in FactKinds.All)
            {
                both.ShouldContain($"| `{kind.Key}` | {kind.Label} |");
            }

            var servicesOnly = Render(ModelSyncGuidance.ConfigureDriftCheckSkill(Context(), ServicesOnly));
            servicesOnly.ShouldContain("`guardrails/services.drift.json`");
            servicesOnly.ShouldNotContain("| `column.textType` |");
            servicesOnly.ShouldContain("`intent/.cache/guardrails/Acme/ServicesDriftReport.mdx`");
            servicesOnly.ShouldNotContain("DomainDriftReport");
        }

        [Fact]
        public void The_rule_and_skills_render_as_written()
        {
            var context = Context();
            var documents = new Dictionary<string, string>
            {
                ["guardrails-model-sync.md"] = ModelSyncGuidance.Rule(context, Both),
                ["sync-model.md"] = ModelSyncGuidance.Skill(context, Both),
                ["configure-drift-check.md"] = ModelSyncGuidance.ConfigureDriftCheckSkill(context, Both)
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
            var context = Context();
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
        public void The_validate_drift_recipes_skill_runs_each_contributed_designers_task()
        {
            var both = Render(ModelSyncGuidance.ValidateDriftRecipesSkill(Context(), Both));

            both.ShouldStartWith("---\ndescription: ");
            both.ShouldContain("| Domain | `guardrails/domain.drift.json` | `Intent.Domain.Guardrails.Contracts.ValidateDriftRecipes` |\n| Services | `guardrails/services.drift.json` | `Intent.Services.Guardrails.Contracts.ValidateDriftRecipes` |\n");
            both.ShouldContain("await executeModuleTask(");
            foreach (var key in new[]
            {
                "errorMessage", "recipeFileExists", "problems", "notChecked", "readFromCode", "fromDefault", "fromAbsent", "unverifiable", "contradicted",
                "readCasesMatchingNothing", "conventions", "found", "samples", "probeWarnings", "wrongElementWarnings",
                "fact", "element", "file", "readings", "from", "findPath", "readCase", "level", "matched"
            })
            {
                both.ShouldContain($"`{key}`", customMessage: "the skill describes every field the task returns");
            }

            var servicesOnly = Render(ModelSyncGuidance.ValidateDriftRecipesSkill(Context(), ServicesOnly));
            servicesOnly.ShouldContain("`Intent.Services.Guardrails.Contracts.ValidateDriftRecipes`");
            servicesOnly.ShouldNotContain("Intent.Domain.Guardrails.Contracts.ValidateDriftRecipes");
        }

        [Fact]
        public void The_intent_drift_reference_is_written_beside_the_sync_model_skill_which_points_its_intent_rows_at_it()
        {
            var context = Context();
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.ExecutionContext.Returns(context);
            outputTarget.Metadata.Returns(new Dictionary<string, object>());

            var config = new Common.Drift.Templates.IntentDriftReference.IntentDriftReferenceTemplate(outputTarget).GetTemplateFileConfig();
            config.FileName.ShouldBe("intent-drift");
            config.FileExtension.ShouldBe("md");
            config.LocationInProject.Replace('\\', '/').ShouldBe("sync-model/references");

            var skill = Render(ModelSyncGuidance.Skill(context, Both));
            skill.ShouldContain("\n| `<element>` *raises*, *creates*, *updates*, *deletes*, *queries*, *invokes*, *publishes*, *sends*: model `…`, code (none) (…), or can't be verified | ");
            skill.ShouldContain("follow [references/intent-drift.md](references/intent-drift.md) before changing either side |\n| `<element>` *fact*: model `…`, code `…` |");

            var reference = Render(ModelSyncGuidance.IntentDrift(context, Both));
            reference.ShouldStartWith("# Resolving intent drift\n\nAn intent row (*raises*, *creates*, *updates*, *deletes*, *queries*, *invokes*, *publishes*, *sends*) is about one association the model gives an element: a `Domain Event Origin`, from a constructor or operation to a Domain Event it raises; an entity action");
            reference.ShouldContain("modelling that element first if the model doesn't have it. If this element now calls that one (sends its Command, calls its Service operation), add a `Perform Invocation` to it too. The next run checks it there, which confirms the move |\n");
            reference.ShouldContain("such as a domain event a constructor or operation starts raising, or something a handler starts doing (an entity it acts on, a call, an event it publishes or a command it sends). When the work moved");

            var domainOnly = Render(ModelSyncGuidance.IntentDrift(context, DomainOnly));
            domainOnly.ShouldContain("An intent row (*raises*) is about one association the model gives an element: a `Domain Event Origin`, from a constructor or operation to a Domain Event it raises. It says");
            domainOnly.ShouldContain("modelling that element first if the model doesn't have it. The next run checks it there");
            domainOnly.ShouldNotContain("Perform Invocation");
            domainOnly.ShouldNotContain("handler starts doing");
        }

        [Fact]
        public void The_rule_and_skills_are_written_once_some_module_contributes_a_designer()
        {
            var context = Context();
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.ExecutionContext.Returns(context);
            outputTarget.Metadata.Returns(new Dictionary<string, object>());
            context.OutputTargets.Returns(new[] { outputTarget });
            var rule = new Common.Drift.Templates.ModelSyncRule.ModelSyncRuleTemplate(outputTarget);

            outputTarget.TemplateInstances.Returns(new ITemplate[] { rule });
            rule.CanRunTemplate().ShouldBeFalse();
            ModelSyncGuidance.Designers(context).ShouldBeEmpty();

            var services = Substitute.For<ITemplate, IDriftDesignerSource>();
            ((IDriftDesignerSource)services).Designer.Returns(ServicesDrift.Designer);
            var domain = Substitute.For<ITemplate, IDriftDesignerSource>();
            ((IDriftDesignerSource)domain).Designer.Returns(DomainDrift.Designer);
            outputTarget.TemplateInstances.Returns(new[] { services, rule, domain });
            rule.CanRunTemplate().ShouldBeTrue();
            ModelSyncGuidance.Designers(context).ShouldBe(Both, "one per designer, in their order");
        }

        [Fact]
        public void The_rule_says_a_new_interaction_is_never_reported()
        {
            var both = Render(ModelSyncGuidance.Rule(Context(), Both));
            both.ShouldContain("| Its entity actions, `Perform Invocation`, `Publish Integration Event` and `Send Integration Command` |\n\nThe check reports a modelled interaction the code no longer performs, but never a new one, such as a domain event a constructor or operation starts raising, or something a handler starts doing (an entity it acts on, a call, an event it publishes or a command it sends): add it to the model as you write the code.\n\n## The Software Factory checks it");
            both.ShouldContain("1. Check that the model shows what the code you changed now does, including anything new it does: nothing reports that.\n2. Save every designer");

            var servicesOnly = Render(ModelSyncGuidance.Rule(Context(), ServicesOnly));
            servicesOnly.ShouldContain("but never a new one, such as something a handler starts doing (an entity it acts on, a call, an event it publishes or a command it sends): add it");
            servicesOnly.ShouldNotContain("Domain Event Origin");
        }

        [Fact]
        public void No_guidance_mentions_the_recipe_check_table_any_more()
        {
            var context = Context();
            foreach (var markdown in new[] { ModelSyncGuidance.Rule(context, Both), ModelSyncGuidance.Skill(context, Both), ModelSyncGuidance.ConfigureDriftCheckSkill(context, Both) })
            {
                markdown.ShouldNotContain("Recipe check");
                markdown.ShouldContain("validate-drift-recipes");
            }
        }

        private static string Render(string markdown)
        {
            return new MarkdownFile("SKILL").FromMarkdown(markdown).ToString().Replace("\r\n", "\n");
        }

        private static ISoftwareFactoryExecutionContext Context()
        {
            var solution = Substitute.For<ISolutionConfig>();
            // A path as this OS writes it, as Intent Architect gives it: on the Linux CI agent a backslash isn't a separator.
            solution.SolutionRootLocation.Returns(Path.Combine(Path.GetPathRoot(Path.GetTempPath()), "Repos", "Acme", "intent") + Path.DirectorySeparatorChar);
            var application = Substitute.For<IApplicationConfig>();
            application.Name.Returns("Acme");
            var context = Substitute.For<ISoftwareFactoryExecutionContext>();
            context.GetSolutionConfig().Returns(solution);
            context.GetApplicationConfig().Returns(application);
            return context;
        }
    }
}
