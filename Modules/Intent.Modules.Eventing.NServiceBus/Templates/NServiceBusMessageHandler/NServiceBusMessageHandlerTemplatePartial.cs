using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.NServiceBus.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Eventing.NServiceBus.Templates.Constants;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusMessageHandler
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge)]
    public partial class NServiceBusMessageHandlerTemplate : CSharpTemplateBase<IList<IntegrationEventHandlerModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.NServiceBus.NServiceBusMessageHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NServiceBusMessageHandlerTemplate(IOutputTarget outputTarget, IList<IntegrationEventHandlerModel> model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
            AddTypeSource(IntegrationCommandTemplate.TemplateId);
            AddTypeSource(NServiceBusMessageBus.NServiceBusMessageBusTemplate.TemplateId);

            var outboxPattern = ExecutionContext.Settings.GetNServiceBusSettings().OutboxPattern();
            var hasOutbox = outboxPattern.IsSqlPersistence();

            SubscribedMessageModels = model
                .SelectMany(h => h.IntegrationEventSubscriptions()
                    .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, x => x.TypeReference.Element.AsMessageModel()!)
                    .Select(sub => sub.TypeReference.Element.AsMessageModel()!))
                .Distinct()
                .ToList();

            SubscribedCommandModels = model
                .SelectMany(h => h.IntegrationCommandSubscriptions()
                    .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, x => x.TypeReference.Element.AsIntegrationCommandModel()!)
                    .Select(sub => sub.TypeReference.Element.AsIntegrationCommandModel()!))
                .Distinct()
                .ToList();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddUsing("NServiceBus");

            if (hasOutbox)
            {
                CSharpFile
                    .AddUsing("Microsoft.EntityFrameworkCore")
                    .AddUsing("NServiceBus.Persistence.Sql");
            }

            // Generic base — all pipeline logic lives here. NServiceBus's assembly scanner skips
            // open generic types, so we also emit one sealed concrete subclass per subscribed
            // message type below. Those are discovered by the scanner and delegate to this base.
            CSharpFile.AddClass("NServiceBusMessageHandler", @class =>
            {
                @class.Internal();
                @class.AddGenericParameter("TMessage");
                @class.AddGenericTypeConstraint("TMessage", c => c.AddType("class"));
                @class.ImplementsInterface("IHandleMessages<TMessage>");

                @class.AddConstructor(ctor =>
                {
                    ctor.AddParameter(this.GetIntegrationEventHandlerInterfaceName() + "<TMessage>", "handler", p => p.IntroduceReadonlyField());
                    if (hasOutbox)
                    {
                        ctor.AddParameter(this.GetTypeName(TemplateRoles.Infrastructure.Data.DbContext), "dbContext", p => p.IntroduceReadonlyField());
                    }
                    ctor.AddParameter(this.GetTypeName(NServiceBusMessageBus.NServiceBusMessageBusTemplate.TemplateId), "messageBus", p => p.IntroduceReadonlyField());
                });

                @class.AddMethod("Task", "Handle", method =>
                {
                    method.Async();
                    method.AddParameter("TMessage", "message");
                    method.AddParameter("IMessageHandlerContext", "context");

                    method.AddStatement("_messageBus.ActiveContext = context;");

                    if (hasOutbox)
                    {
                        method.AddStatement("var sqlSession = context.SynchronizedStorageSession.SqlPersistenceSession();", s => s.SeparatedFromPrevious());
                        method.AddStatement("_dbContext.Database.SetDbConnection(sqlSession.Connection);");
                        method.AddStatement("await _dbContext.Database.UseTransactionAsync((System.Data.Common.DbTransaction)sqlSession.Transaction, context.CancellationToken);");
                        method.AddStatement("await _handler.HandleAsync(message, context.CancellationToken);", s => s.SeparatedFromPrevious());
                        method.AddStatement("await _dbContext.SaveChangesAsync(context.CancellationToken);");
                        method.AddStatement("await _messageBus.FlushAllAsync(context.CancellationToken);");
                    }
                    else
                    {
                        method.AddStatement("await _handler.HandleAsync(message, context.CancellationToken);", s => s.SeparatedFromPrevious());
                    }
                });
            });

            // Sealed concrete subclass per subscribed message type — non-generic, so the
            // NServiceBus assembly scanner can find and register them.
            // Deferred to AfterBuild so GetTypeName / UseType resolve correctly.
            CSharpFile.AfterBuild(file =>
            {
                var handlerInterfaceName = this.GetIntegrationEventHandlerInterfaceName();
                var messageBusTypeName = this.GetTypeName(NServiceBusMessageBus.NServiceBusMessageBusTemplate.TemplateId);
                var dbContextTypeName = hasOutbox ? this.GetTypeName(TemplateRoles.Infrastructure.Data.DbContext) : null;

                foreach (var msgModel in SubscribedMessageModels)
                {
                    var msgTypeName = GetTypeName(IntegrationEventMessageTemplate.TemplateId, msgModel);
                    AddConcreteHandlerClass(file, msgTypeName, handlerInterfaceName, messageBusTypeName, dbContextTypeName);
                }

                foreach (var cmdModel in SubscribedCommandModels)
                {
                    var cmdTypeName = GetTypeName(IntegrationCommandTemplate.TemplateId, cmdModel);
                    AddConcreteHandlerClass(file, cmdTypeName, handlerInterfaceName, messageBusTypeName, dbContextTypeName);
                }
            }, 100);
        }

        private static void AddConcreteHandlerClass(
            CSharpFile file,
            string messageTypeName,
            string handlerInterfaceName,
            string messageBusTypeName,
            string? dbContextTypeName)
        {
            file.AddClass($"NServiceBus{messageTypeName}Handler", @class =>
            {
                @class.Internal().Sealed();
                @class.WithBaseType($"NServiceBusMessageHandler<{messageTypeName}>");

                @class.AddConstructor(ctor =>
                {
                    ctor.AddParameter($"{handlerInterfaceName}<{messageTypeName}>", "handler");
                    if (dbContextTypeName is not null)
                    {
                        ctor.AddParameter(dbContextTypeName, "dbContext");
                    }
                    ctor.AddParameter(messageBusTypeName, "messageBus");

                    if (dbContextTypeName is not null)
                    {
                        ctor.CallsBase(b => b.AddArgument("handler").AddArgument("dbContext").AddArgument("messageBus"));
                    }
                    else
                    {
                        ctor.CallsBase(b => b.AddArgument("handler").AddArgument("messageBus"));
                    }
                });
            });
        }

        /// <summary>
        /// Integration event (Message) types this endpoint subscribes to for this broker.
        /// Read by <see cref="NServiceBusConfiguration.NServiceBusConfigurationTemplate"/> to emit
        /// direct NSB registry registration calls in <c>ConfigureEndpoint</c>.
        /// </summary>
        public IReadOnlyList<MessageModel> SubscribedMessageModels { get; }

        /// <summary>
        /// Integration command types this endpoint handles for this broker.
        /// Read by <see cref="NServiceBusConfiguration.NServiceBusConfigurationTemplate"/> to emit
        /// direct NSB registry registration calls in <c>ConfigureEndpoint</c>.
        /// </summary>
        public IReadOnlyList<IntegrationCommandModel> SubscribedCommandModels { get; }

        public override bool CanRunTemplate()
        {
            // Suppress output when no event or command subscriptions route to this broker
            return Model.Any(h =>
                h.IntegrationEventSubscriptions()
                    .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, x => x.TypeReference.Element.AsMessageModel()!)
                    .Any()
                || h.IntegrationCommandSubscriptions()
                    .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, x => x.TypeReference.Element.AsIntegrationCommandModel()!)
                    .Any());
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig(
            );
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}