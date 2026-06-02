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
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
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

            var allSubscriptions = model
                .SelectMany(h => h.IntegrationEventSubscriptions()
                    .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, x => x.TypeReference.Element.AsMessageModel()!)
                    .Select(sub => sub.TypeReference.Element.AsMessageModel()!))
                .Distinct()
                .ToList();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddUsing("NServiceBus")
                .AddClass("NServiceBusMessageHandlers", @class =>
                {
                    foreach (var messageModel in allSubscriptions)
                    {
                        var messageTypeName = this.GetIntegrationEventMessageName(messageModel);
                        @class.ImplementsInterface($"IHandleMessages<{messageTypeName}>");
                    }

                    @class.AddConstructor(ctor =>
                    {
                        foreach (var messageModel in allSubscriptions)
                        {
                            var messageTypeName = this.GetIntegrationEventMessageName(messageModel);
                            var handlerInterface = $"{this.GetIntegrationEventHandlerInterfaceName()}<{messageTypeName}>";
                            var paramName = $"handler{messageModel.Name.ToPascalCase()}";
                            ctor.AddParameter(handlerInterface, paramName, param =>
                                param.IntroduceReadonlyField());
                        }
                    });

                    foreach (var messageModel in allSubscriptions)
                    {
                        var messageTypeName = this.GetIntegrationEventMessageName(messageModel);
                        var fieldName = $"_handler{messageModel.Name.ToPascalCase()}";

                        @class.AddMethod("Task", "Handle", method =>
                        {
                            method.Async();
                            method.AddParameter(messageTypeName, "message");
                            method.AddParameter("IMessageHandlerContext", "context");
                            method.AddStatement($"await {fieldName}.HandleAsync(message, context.CancellationToken);");
                        });
                    }
                });
        }

        public override bool CanRunTemplate()
        {
            // Suppress output when no handler subscriptions route to this broker
            return Model.Any(h => h.IntegrationEventSubscriptions()
                .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, x => x.TypeReference.Element.AsMessageModel()!)
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