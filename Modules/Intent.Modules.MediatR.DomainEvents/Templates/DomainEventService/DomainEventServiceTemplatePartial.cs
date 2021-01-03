using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.MediatR.DomainEvents.Templates.DomainEventNotification;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Templates.DomainEventService
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DomainEventServiceTemplate : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.MediatR.DomainEvents.DomainEventService";

        public DomainEventServiceTemplate(IOutputTarget outputTarget, object model) : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DomainEventService",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface(GetTemplate<IClassProvider>(DomainEventServiceInterfaceTemplate.TemplateId))
                .ForConcern("Infrastructure")
                .WithPerServiceCallLifeTime());
        }

        private string GetDomainEventBaseType()
        {
            return GetTypeName(DomainEventBaseTemplate.TemplateId);
        }

        private string GetInterfaceType()
        {
            return GetTypeName(DomainEventServiceInterfaceTemplate.TemplateId);
        }

        private string GetDomainEventNotificationType()
        {
            return GetTypeName(DomainEventNotificationTemplate.TemplateId);
        }
    }
}