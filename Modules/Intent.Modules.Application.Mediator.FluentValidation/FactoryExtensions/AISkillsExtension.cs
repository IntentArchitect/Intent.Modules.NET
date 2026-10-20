using Intent.Engine;
using Intent.Modules.Application.Mediator.Templates.CommandHandlerSkill;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.FluentValidation.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AISkillsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Mediator.FluentValidation.AISkillsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // The core generates the skill; only this module knows a validator exists, so the guidance
            // for writing its rules is added here and is absent when this module is not installed.
            // One instance is generated per AI harness (.claude, .codex, .github, .opencode, ...).
            foreach (var skill in application.FindTemplateInstances<IMarkdownFileBuilderTemplate>(CommandHandlerSkillTemplate.TemplateId))
            {
                skill.MarkdownFile.OnBuild(AddValidatorGuidance);
            }
        }

        private static void AddValidatorGuidance(IMarkdownFile file)
        {
            file.BeforeSection("Workflow", "Validator", section =>
            {
                section.WithText("Every command has a generated validator (`*CommandValidator.cs`), and its rules are also yours to write. The generator emits only the rules modelled explicitly on the command's fields, plus `NotNull`, enum and nested DTO checks. It does not reliably derive rules from the entities the command touches, so write them when you write the handler.");
                section.WithListItems("""
                    - Load the `fluent-validation-custom-validation` skill before editing the validator, and follow its rules.
                    - Write the rules in `ConfigureValidationRules`. Derive them from the entity the command creates or updates, found through the Create Entity and Update Entity associations, and match each command field to an entity attribute by name and meaning, as for the handler.
                    - Take each rule from evidence on that entity: required attributes, maximum lengths, numeric ranges, formats such as email or regular expressions, and unique indexes. The evidence is the entity itself, its Domain Constraints stereotypes, and its persistence configuration (for example `IsRequired`, `HasMaxLength` or a unique `HasIndex`). Do not invent rules the entity does not carry.
                    - Check uniqueness with `MustAsync` through a repository abstraction, e.g. an `ExistsBy...Async` method, extending the repository if the method is missing. For an Update Entity command, exclude the entity being updated from the check.
                    - Keep the rules that are already generated, and do not add a second rule for a field that one of them already covers.
                    - ALWAYS, when you add rules, put `[IntentManaged(Mode.Fully, Body = Mode.Ignore)]` on `ConfigureValidationRules` so regeneration keeps them. From then on the generator no longer updates that method, so when you revisit the command, also re-read the validation stereotypes on its fields and bring the rules in line.
                    - A command that only deletes or calls other operations usually needs no rules beyond the generated ones. Leave its validator as generated.
                    - Before finishing, check that every rule in the validator is traceable to the target entity or a field stereotype.
                    """);
            });
        }
    }
}
