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

                var saveChangesRule = GetSaveChangesRule(application, skill.Id);
                skill.MarkdownFile.OnBuild(f => AddEFRepoGuidance(f, saveChangesRule));
            }
        }

        private static string GetSaveChangesRule(IApplication application, string skillId)
        {
            // The DbContext dispatches domain events inside SaveChangesAsync, before it writes, so a domain event
            // handler's changes are saved by the save that raised the event whatever the setting says.
            if (skillId.Contains("DomainEventHandler", StringComparison.Ordinal))
            {
                return "Do not call `SaveChangesAsync`: the handler runs inside the save that raised the event, and that save writes your changes too. Remove any such call you find in a handler you review.";
            }

            var automaticallyPersistUnitOfWork =
                application.GetSettings().GetUnitOfWorkSettings()?.AutomaticallyPersistUnitOfWork() ?? true;
            var isService = skillId.Contains("ServiceImplementation", StringComparison.Ordinal);
            var caller = isService ? "operation" : "handler";

            // With the setting off nothing commits after the handler, and the generated CRUD code saves in the
            // handler itself, so the rule flips rather than disappearing.
            if (!automaticallyPersistUnitOfWork)
            {
                return $"Call `SaveChangesAsync` yourself, once, after the {caller}'s last change: automatic unit-of-work saving is off in this application, so nothing else saves. Follow the generated {caller}s, which do the same.";
            }

            return isService
                ? "Do not call `SaveChangesAsync`: the unit of work commits after the operation returns. Call it, once and before returning, only when the operation returns a value the database generates, such as a new Id, a row version or a computed column. If unsure, leave it out, and remove any call you find that isn't needed."
                : "Do not call `SaveChangesAsync`: the pipeline's unit of work commits after the handler. Call it, once and before returning, only when the handler returns a value the database generates, such as a new Id, a row version or a computed column. If unsure, leave it out, and remove any call you find that isn't needed.";
        }

        private static void AddEFRepoGuidance(IMarkdownFile file, string saveChangesRule)
        {
            // Shares "Persistence (EF Core)" with Intent.EntityFrameworkCore's loading rule, which comes first.
            file.WithSection(PersistenceSectionTitle, section =>
            {
                section.WithListItem(saveChangesRule);

                section.WithListItem("Do not call the repository's `Update`. EF tracks the entities it loaded, so changing their properties is enough. Call `Add` or `Remove` only to insert or delete, and remove any `Update` call you find on an entity loaded from a repository.");
            });
        }
    }
}
