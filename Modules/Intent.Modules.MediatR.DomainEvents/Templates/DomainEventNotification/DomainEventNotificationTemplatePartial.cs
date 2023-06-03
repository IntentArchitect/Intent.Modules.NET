using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Templates.DomainEventNotification
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DomainEventNotificationTemplate : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.MediatR.DomainEvents.DomainEventNotification";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DomainEventNotificationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DomainEventNotification",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string GetDomainEventBaseType()
        {
            return GetTypeName(DomainEventBaseTemplate.TemplateId);
        }
    }
}