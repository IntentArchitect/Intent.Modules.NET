using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventHandlerInterface;
using Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.IntegrationEventHandlerImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class IntegrationEventHandlerImplementationTemplate : CSharpTemplateBase<MessageHandlerModel>
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.IntegrationEventHandlerImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationEventHandlerImplementationTemplate(IOutputTarget outputTarget, MessageHandlerModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.TypeReference.Element.Name.ToPascalCase()}EventHandler",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface(GetTemplate<IClassProvider>(IntegrationEventHandlerInterfaceTemplate.TemplateId, Model))
                .ForConcern("Application")
                .WithPriority(100));
        }

        private string GetMessageName()
        {
            return GetTypeName(Model.TypeReference);
        }
    }
}