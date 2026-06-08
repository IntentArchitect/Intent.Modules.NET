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
    /// <summary>
    /// Adjusts the MediatR pipeline when the NServiceBus EF outbox pattern is active.
    ///
    /// Without this extension, the pipeline runs:
    ///   UnitOfWorkBehaviour → SaveChangesAsync (EF default tx)
    ///   MessageBusPublishBehaviour → FlushAllAsync (opens ITransactionalSession — but changes already committed!)
    ///
    /// With this extension, UoW's SaveChangesAsync is suppressed so that domain entity changes remain
    /// pending in the EF ChangeTracker when FlushAllAsync runs. FlushAllAsync then opens the
    /// ITransactionalSession, joins EF to NSB's connection+transaction, calls SaveChangesAsync inside
    /// that session, and commits everything atomically (domain data + outbox records in one transaction).
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    internal class NServiceBusMessageBusInteropExtension : FactoryExtensionBase
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
                var saveStatement = handleMethod.Statements
                    .FirstOrDefault(s => s.GetText("").Contains("SaveChangesAsync"));
                saveStatement?.Remove();
            }, 1000);
        }

        private static bool IsOutboxActive(IApplication application) =>
            application.Settings.GetNServiceBusSettings()?.OutboxPattern()?.IsEntityFramework() == true;
    }
}
