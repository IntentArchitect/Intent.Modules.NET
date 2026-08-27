using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts;
using Intent.Modules.Eventing.Wolverine.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineMessageBusInteropExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.Wolverine.WolverineMessageBusInteropExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterMetadataLoad(IApplication application)
        {
            const string WolverineMessageBusId = "d87ae50c-5ac0-4d95-a45c-e6b185b7675a";
            MessageBusRegistry.Register(WolverineMessageBusId, Templates.Constants.BrokerStereotypeIds);
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            InstallMessageBusForServiceContractDispatch(application);
            InstallMessageBusForWolverineDispatch(application);
            InstallMessageBusForMediatRDispatch(application);
        }

        /// <summary>
        /// R6.3: Wolverine's own AutoApplyTransactions/durable-outbox policies already dispatch
        /// messages on SaveChanges when the Transactional Outbox setting is Durable, so the
        /// application layer's explicit `_messageBus.FlushAllAsync(...)` call becomes redundant
        /// and would double-flush if left in place. Mirrors
        /// Intent.Modules.Eventing.MassTransit/FactoryExtensions/MessageBusInteropExtension.cs.
        /// </summary>
        private void InstallMessageBusForServiceContractDispatch(IApplication application)
        {
            if (!IsTransactionalOutboxPatternSelected(application))
            {
                return;
            }

            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.AspNetCore.Controllers.Controller"));
            foreach (var template in templates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    foreach (var method in priClass.Methods)
                    {
                        var statementToRemove = method.FindStatement(stmt => stmt.HasMetadata("eventbus-flush"));
                        statementToRemove?.Remove();
                    }
                }, 1000);
            }
        }

        private void InstallMessageBusForWolverineDispatch(IApplication application)
        {
            if (!IsTransactionalOutboxPatternSelected(application))
            {
                return;
            }

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.Wolverine.ApplicationHandlerPolicy");
            if (template == null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                var method = priClass.FindMethod("Apply");
                var statementsToRemove = method.FindStatements(stmt => stmt.HasMetadata("eventbus-flush")).ToList();
                foreach (var statement in statementsToRemove)
                {
                    statement.Remove();
                }
            }, 1000);
        }

        private void InstallMessageBusForMediatRDispatch(IApplication application)
        {
            if (!IsTransactionalOutboxPatternSelected(application))
            {
                return;
            }

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.DependencyInjection.DependencyInjection");
            if (template == null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                var method = priClass.FindMethod("AddApplication");
                var mediatrConfigLambda = (CSharpInvocationStatement)method.FindStatement(stmt => stmt.HasMetadata("mediatr-config"));
                if (mediatrConfigLambda == null)
                {
                    return;
                }

                var mediatorConfig = (CSharpLambdaBlock)mediatrConfigLambda.Statements.FirstOrDefault();
                var statementToRemove = mediatorConfig?.Statements.FirstOrDefault(stmt => stmt.HasMetadata("eventbus-flush"));
                statementToRemove?.Remove();
            }, 1000);
        }

        private static bool IsTransactionalOutboxPatternSelected(IApplication application)
        {
            return application.Settings.GetWolverineMessageBusSettings()?.TransactionalOutbox()?.IsDurable() == true;
        }
    }
}
