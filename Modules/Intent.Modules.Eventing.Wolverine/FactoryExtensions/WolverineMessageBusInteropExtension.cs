using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Shared;
using Intent.Modules.Eventing.Contracts;
using Intent.Modules.Eventing.Contracts.Templates;
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

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallMessageBusForDbContextForTransactionalOutboxPattern(application);
        }

        /// <summary>
        /// R6.3: when the Transactional Outbox setting is Durable, the application layer's explicit
        /// `_messageBus.FlushAllAsync(...)` call is stripped out of the dispatch layer (controllers,
        /// the Wolverine handler policy, the MediatR config lambda) and replaced with a single splice
        /// into <c>ApplicationDbContext.SaveChanges</c>/<c>SaveChangesAsync</c> instead — see
        /// <see cref="InstallMessageBusForDbContextForTransactionalOutboxPattern"/>. The dispatch-layer
        /// flush is dispatcher-specific and would double-flush (or, without the splice, be discarded
        /// entirely with nothing left to dispatch the buffered messages) if left in place. Mirrors
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

        /// <summary>
        /// R6.3: the dispatch-layer flush strips above leave nothing to dispatch the messages
        /// <c>WolverineMessageBus</c> buffers onto its <c>_pendingActions</c> list — publishing only
        /// ever appends to that list; Wolverine's own outbox/durability policies never see a message
        /// until <c>FlushAllAsync</c> actually runs. Mirrors
        /// Intent.Modules.Eventing.MassTransit/FactoryExtensions/MessageBusInteropExtension.cs:
        /// splice the flush directly into <c>ApplicationDbContext.SaveChanges</c>/<c>SaveChangesAsync</c>
        /// instead, below the domain-event dispatch when one exists, otherwise directly above
        /// <c>base.SaveChanges...</c>. This is dispatcher-agnostic by design — see
        /// Intent.Eventing.Wolverine's CONTEXT.md, D5 carve-out.
        /// <para>
        /// Gated on the application actually publishing something, NOT merely on the outbox being
        /// Durable. <c>WolverineEventingRegistrationExtension.RegisterWolverineMessageBus</c>
        /// deliberately registers no bus for a subscribe-only application (no Wolverine-designated
        /// published messages and no sent Integration Commands), so injecting the bus interface into
        /// the DbContext there produces a constructor dependency nothing can satisfy - the host dies
        /// at startup on DI validation ("Unable to resolve service for type '...IMessageBus' while
        /// attempting to activate 'ApplicationDbContext'"), before a single message is handled. The
        /// guard is semantic, not just defensive: a subscribe-only application has no outgoing
        /// messages buffered on the bus, so there is nothing for a flush to dispatch in the first
        /// place. Note this is a deliberate divergence from
        /// Intent.Modules.Eventing.MassTransit/FactoryExtensions/MessageBusInteropExtension.cs, whose
        /// equivalent splice gates on the outbox setting alone - MassTransit has no subscribe-only
        /// registration skip for that guard to have to match.
        /// </para>
        /// </summary>
        private void InstallMessageBusForDbContextForTransactionalOutboxPattern(IApplication application)
        {
            if (!IsTransactionalOutboxPatternSelected(application))
            {
                return;
            }

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.DbContext);
            if (template is null)
            {
                return;
            }

            if (!PublishesAnyWolverineMessages(application, template))
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var busInterface = template.GetBusInterfaceName();
                var busVariableName = template.GetBusVariableName();
                var busFieldName = $"_{busVariableName}";

                var constructor = @class.Constructors.First();
                if (constructor.Parameters.All(p => p.Type != busInterface))
                {
                    constructor.AddParameter(busInterface, busVariableName, p => p.IntroduceReadonlyField());
                }

                var syncMethod = template.GetSaveChangesMethod();
                var syncStatement = syncMethod.FindStatement(stmt => stmt.GetText("") == "DispatchEventsAsync().GetAwaiter().GetResult();");
                if (syncStatement != null)
                {
                    syncStatement.InsertBelow((CSharpStatement)$"{busFieldName}.FlushAllAsync().GetAwaiter().GetResult();");
                }
                else
                {
                    syncStatement = syncMethod.FindStatement(stmt => stmt.GetText("").Contains("base.SaveChanges"));
                    syncStatement?.InsertAbove($"{busFieldName}.FlushAllAsync().GetAwaiter().GetResult();");
                }

                var asyncMethod = template.GetSaveChangesAsyncMethod();
                var asyncStatement = asyncMethod.FindStatement(stmt => stmt.GetText("") == "await DispatchEventsAsync(cancellationToken);");
                if (asyncStatement != null)
                {
                    asyncStatement.InsertBelow((CSharpStatement)$"await {busFieldName}.FlushAllAsync(cancellationToken);");
                }
                else
                {
                    asyncStatement = asyncMethod.FindStatement(stmt => stmt.GetText("").Contains("base.SaveChanges"));
                    asyncStatement?.InsertAbove($"await {busFieldName}.FlushAllAsync(cancellationToken);");
                }
            }, 10);
        }

        private static bool IsTransactionalOutboxPatternSelected(IApplication application)
        {
            return application.Settings.GetWolverineMessageBusSettings()?.TransactionalOutbox()?.IsDurable() == true;
        }

        /// <summary>
        /// Mirrors the publish-side test <c>WolverineEventingRegistrationExtension.RegisterWolverineMessageBus</c>
        /// uses to decide whether to register the bus at all: an application with no Wolverine-designated
        /// published Message and no Wolverine-designated sent Integration Command is subscribe-only, and gets
        /// no bus registration. Anything that injects the bus interface must agree with that test or it
        /// asks the container for a service that was never registered.
        /// </summary>
        private static bool PublishesAnyWolverineMessages(IApplication application, ICSharpFileBuilderTemplate template)
        {
            var metadataManager = template.ExecutionContext.MetadataManager;

            return metadataManager
                       .GetExplicitlyPublishedMessageModels(application)
                       .FilterMessagesForThisMessageBroker(application, Templates.Constants.BrokerStereotypeIds)
                       .Any()
                   || metadataManager
                       .GetExplicitlySentIntegrationCommandModels(application)
                       .FilterMessagesForThisMessageBroker(application, Templates.Constants.BrokerStereotypeIds)
                       .Any();
        }
    }
}
