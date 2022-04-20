using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEvent;
using Intent.Modules.MediatR.DomainEvents.Templates.DomainEventNotification;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Templates.DefaultDomainEventHandler
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DefaultDomainEventHandlerTemplate : CSharpTemplateBase<DomainEventModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.MediatR.DomainEvents.DefaultDomainEventHandler";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DefaultDomainEventHandlerTemplate(IOutputTarget outputTarget, DomainEventModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
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