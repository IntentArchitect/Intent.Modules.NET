using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventHandlerInterface;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventHandler;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventHandlerImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class IntegrationEventHandlerImplementationTemplate : CSharpTemplateBase<MessageSubscribeAssocationTargetEndModel>
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.IntegrationEventHandlerImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationEventHandlerImplementationTemplate(IOutputTarget outputTarget, MessageSubscribeAssocationTargetEndModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.TypeReference.Element.Name.RemoveSuffix("Event").ToPascalCase()}EventHandler",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface($"{this.GetIntegrationEventHandlerInterfaceName()}<{GetMessageName()}>")
                .ForConcern("Application")
                .WithPriority(100)
                .HasDependency(GetTemplate<IClassProvider>(IntegrationEventHandlerInterfaceTemplate.TemplateId))
                .HasDependency(GetTemplate<IClassProvider>(IntegrationEventMessageTemplate.TemplateId, Model.TypeReference.Element)));
        }

        private string GetMessageName()
        {
            return GetTypeName("Intent.Eventing.Contracts.IntegrationEventMessage", Model.TypeReference.Element);
        }
    }
}