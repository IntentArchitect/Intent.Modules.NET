using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEvent;
using Intent.Modules.MediatR.DomainEvents.Templates.DomainEventNotification;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Templates.DomainEventHandler
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DomainEventHandlerTemplate : CSharpTemplateBase<DomainEventModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.MediatR.DomainEvents.DomainEventHandler";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DomainEventHandlerTemplate(IOutputTarget outputTarget, DomainEventModel model) : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Handler",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string GetDomainEventNotificationType()
        {
            return GetTypeName(DomainEventNotificationTemplate.TemplateId);
        }


        private string GetDomainEventType()
        {
            return GetTypeName(DomainEventTemplate.TemplateId, Model);
        }
    }
}