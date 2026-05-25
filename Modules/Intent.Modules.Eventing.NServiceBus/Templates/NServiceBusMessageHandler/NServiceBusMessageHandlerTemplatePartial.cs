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

            // When the application has no integration event handlers (or none whose subscriptions resolve
            // to this broker in composite mode), Model is empty. Fall back to the default class name so the
            // template still produces a valid (empty) class — emitting nothing would breach the SDK's
            // "at least one structural declaration" contract.
            var className = "EventMessageHandler";
            if (Model.Any())
            {
                var parentElement = Model.First().InternalElement.ParentElement;
                className = parentElement.SpecializationType == "Folder"
                    ? $"{parentElement.Name.ToPascalCase()}MessageHandler"
                    : "EventMessageHandler";
            }

            // Flatten all (handler, subscription) pairs across every handler in this folder group
            var handlerSubscriptions = Model
                .SelectMany(h => h.IntegrationEventSubscriptions()
                    .FilterMessagesForThisMessageBroker(ExecutionContext, BrokerStereotypeIds, x => x.TypeReference.Element.AsMessageModel()!)
                    .Select(sub => (Handler: h, Subscription: sub)))
                .ToList();

            CSharpFile = new CSharpFile(this.GetNamespace("MessageHandlers"), this.GetFolderPath("MessageHandlers"))
                .AddUsing("System.Threading.Tasks")
                .AddUsing("NServiceBus")
                .AddClass(className, @class =>
                {
                    foreach (var (_, sub) in handlerSubscriptions)
                    {
                        var messageTypeName = this.GetIntegrationEventMessageName(sub.TypeReference.Element.AsMessageModel());
                        @class.ImplementsInterface($"IHandleMessages<{messageTypeName}>");
                    }

                    @class.AddConstructor(ctor =>
                    {
                        foreach (var (_, sub) in handlerSubscriptions)
                        {
                            var messageTypeName = this.GetIntegrationEventMessageName(sub.TypeReference.Element.AsMessageModel());
                            var handlerInterface = $"{this.GetIntegrationEventHandlerInterfaceName()}<{messageTypeName}>";
                            var paramName = $"handler{sub.TypeReference.Element.Name.ToPascalCase()}";
                            ctor.AddParameter(handlerInterface, paramName, param =>
                                param.IntroduceReadonlyField());
                        }
                    });

                    foreach (var (_, sub) in handlerSubscriptions)
                    {
                        var messageTypeName = this.GetIntegrationEventMessageName(sub.TypeReference.Element.AsMessageModel());
                        var fieldName = $"_handler{sub.TypeReference.Element.Name.ToPascalCase()}";

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

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}