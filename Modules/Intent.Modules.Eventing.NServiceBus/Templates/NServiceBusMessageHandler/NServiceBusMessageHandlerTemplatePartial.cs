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

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus.Templates.NServiceBusMessageHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class NServiceBusMessageHandlerTemplate : CSharpTemplateBase<IntegrationEventHandlerModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.NServiceBus.NServiceBusMessageHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public NServiceBusMessageHandlerTemplate(IOutputTarget outputTarget, IntegrationEventHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading.Tasks")
                .AddUsing("NServiceBus")
                .AddClass($"{Model.Name.ToPascalCase()}MessageHandler", @class =>
                {
                    var subscriptions = Model.IntegrationEventSubscriptions().ToList();
                    var multipleSubscriptions = subscriptions.Count > 1;

                    foreach (var subscription in subscriptions)
                    {
                        var messageTypeName = this.GetIntegrationEventMessageName(subscription.TypeReference.Element.AsMessageModel());
                        @class.ImplementsInterface($"IHandleMessages<{messageTypeName}>");
                    }

                    @class.AddConstructor(ctor =>
                    {
                        foreach (var subscription in subscriptions)
                        {
                            var messageTypeName = this.GetIntegrationEventMessageName(subscription.TypeReference.Element.AsMessageModel());
                            var handlerInterface = $"{this.GetIntegrationEventHandlerInterfaceName()}<{messageTypeName}>";
                            var paramName = multipleSubscriptions
                                ? $"handler{subscription.TypeReference.Element.Name.ToPascalCase()}"
                                : "handler";
                            ctor.AddParameter(handlerInterface, paramName, param =>
                                param.IntroduceReadonlyField());
                        }
                    });

                    foreach (var subscription in subscriptions)
                    {
                        var messageTypeName = this.GetIntegrationEventMessageName(subscription.TypeReference.Element.AsMessageModel());
                        var fieldName = multipleSubscriptions
                            ? $"_handler{subscription.TypeReference.Element.Name.ToPascalCase()}"
                            : "_handler";

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