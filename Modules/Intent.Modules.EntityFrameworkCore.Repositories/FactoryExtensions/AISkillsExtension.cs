using System;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Plugins;
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

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

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
            RegisterEFRepositoryGuidance(
                application,
                "Intent.Application.MediatR.CommandHandlerSkillTemplate");
            RegisterEFRepositoryGuidance(
                application,
                "Intent.Application.ServiceImplementations.ServiceImplementationSkillTemplate");
        }

        private static void RegisterEFRepositoryGuidance(
            IApplication application,
            string templateId)
        {
            var skill = application.FindTemplateInstance<IMarkdownFileBuilderTemplate>(templateId);

            skill?.MarkdownFile.OnBuild(f => AddEFRepoGuidance(application, f));
        }

        private static void AddEFRepoGuidance(IApplication application, IMarkdownFile file)
        {
            var automaticallyPersistUnitOfWork =
                application.GetSettings().GetUnitOfWorkSettings()?.AutomaticallyPersistUnitOfWork() ?? true;

            if (automaticallyPersistUnitOfWork)
            {
                file.BeforeSection("Output expectations", "Unit of Work guidance", section =>
                {
                    section.WithListItems("""
                            - SaveChanges rule (STRICT): Do not call UnitOfWork.SaveChangesAsync(...) / SaveChangesAsync(...) in a handler/service method unless the operation returns a payload that requires DB-generated values, such as a generated Id, surrogate key, RowVersion/concurrency token, DB-generated timestamp, or computed column.
                            - If the operation returns Unit, void, Task, or IRequest with no result: do not call SaveChangesAsync.
                            - If the operation returns an identifier or DTO that needs generated fields: call SaveChangesAsync before returning.
                            - If unsure, omit SaveChangesAsync and assume an outer unit-of-work/pipeline commit.
                            - When reviewing code, remove SaveChangesAsync unless there is a clear generated-value or immediate-commit requirement.
                            """);
                });
            }

            file.BeforeSection("Output expectations", "Entity Framework repository guidance", section =>
            {
                section.WithListItems("""
                            - Repository update rule (STRICT): Do not call repository.Update(...) / repo.Update(...) when using EF repositories.
                            - EF tracks loaded entities automatically. Modify the entity properties directly and let the Unit of Work persist the tracked changes.
                            - Only call Add/Create/Delete operations when inserting or removing entities.
                            - When reviewing code, remove unnecessary Update calls for entities loaded from an EF repository.
                            """);
            });
        }
    }
}