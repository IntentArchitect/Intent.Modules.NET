using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Eventing.NServiceBus.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class NServiceBusMessageBusInteropExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.NServiceBus.NServiceBusMessageBusInteropExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 500;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!IsOutboxActive(application))
            {
                return;
            }

            SuppressUnitOfWorkSaveChanges(application);
        }

        private static void SuppressUnitOfWorkSaveChanges(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                "Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour");
            if (template == null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var handleMethod = file.Classes.First().FindMethod("Handle");
                if (handleMethod == null)
                {
                    return;
                }

                // Remove the SaveChangesAsync call so that domain entity changes stay pending in the
                // EF ChangeTracker. FlushAllAsync (invoked by MessageBusPublishBehaviour after the
                // pipeline) opens the ITransactionalSession, joins EF to NSB's connection+transaction,
                // and commits domain entities + outbox records in one atomic operation.
                //
                // The SaveChangesAsync statement is tagged with metadata "transaction" = "save-changes"
                // by the UnitOfWork framework (PersistenceUnitOfWork.cs). Using metadata lookup is more
                // reliable than text matching and correctly locates the statement even when it is nested
                // inside a TransactionScope using block.
                var saveStatement = handleMethod.FindStatement(
                    s => s.HasMetadata("transaction") && s.GetMetadata<string>("transaction") == "save-changes");
                saveStatement?.Remove();

                // Also remove the explanatory comment immediately preceding SaveChangesAsync if it exists.
                // It references "SaveChanges" and becomes misleading when the call is suppressed.
                var saveComment = handleMethod.FindStatement(
                    s => s.HasMetadata("transaction") && s.GetMetadata<string>("transaction") == "save-changes-comment");
                saveComment?.Remove();
            }, 1000);
        }

        private static bool IsOutboxActive(IApplication application) =>
            application.Settings.GetNServiceBusSettings()?.OutboxPattern()?.IsSqlPersistence() == true;
    }
}
