using System;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Repositories.Api;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AISkillsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Repositories.AISkillsExtension";

        private const string PersistenceSectionTitle = "Persistence (EF Core)";

        // Runs after Intent.EntityFrameworkCore's AISkillGuidanceForServiceImplementations (Order 0), so its
        // loading rule leads the shared "Persistence (EF Core)" section.
        [IntentManaged(Mode.Ignore)]
        public override int Order => 1;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // Target only handler-specific skill files (command/query handlers) so EF repository
            // guidance doesn't pollute unrelated skill files (domain-entity, fluent-validation, etc.).
            foreach (var skill in application.FindTemplateInstances<IMarkdownFileBuilderTemplate>(TemplateRoles.AI.Context.SkillsHandler))
            {
                // A query changes no state, so the save and Update rules only go to skills that write.
                if (skill.Id.Contains("QueryHandler", StringComparison.Ordinal))
                {
                    continue;
                }

                var saveChangesRule = skill.Id.Contains("DomainEventHandler", StringComparison.Ordinal)
                    ? "Do not call `SaveChangesAsync`: the handler runs inside the unit of work that raised the event."
                    : skill.Id.Contains("ServiceImplementation", StringComparison.Ordinal)
                        ? "Do not call `SaveChangesAsync`: the unit of work commits after the operation returns. Call it only when the operation returns a value the database generates, such as a new Id."
                        : "Do not call `SaveChangesAsync`: the pipeline's unit of work commits after the handler. Call it only when the handler returns a value the database generates, such as a new Id.";
                skill.MarkdownFile.OnBuild(f => AddEFRepoGuidance(application, f, saveChangesRule));
            }
        }

        private static void AddEFRepoGuidance(IApplication application, IMarkdownFile file, string saveChangesRule)
        {
            var automaticallyPersistUnitOfWork =
                application.GetSettings().GetUnitOfWorkSettings()?.AutomaticallyPersistUnitOfWork() ?? true;

            // Shares "Persistence (EF Core)" with Intent.EntityFrameworkCore's loading rule, which comes first.
            file.WithSection(PersistenceSectionTitle, section =>
            {
                if (automaticallyPersistUnitOfWork)
                {
                    section.WithListItem(saveChangesRule);
                }

                section.WithListItem("Do not call the repository's `Update`. EF tracks the entities it loaded, so changing their properties is enough. Call `Add` or `Remove` only to insert or delete.");
            });
        }
    }
}
