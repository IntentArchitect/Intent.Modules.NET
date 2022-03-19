using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.DomainEvents.Templates.DomainEvent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DomainEventTemplate : CSharpTemplateBase<DomainEventModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.DomainEvents.DomainEvent";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DomainEventTemplate(IOutputTarget outputTarget, DomainEventModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(DomainEventTemplate.TemplateId);
            AddTypeSource("Domain.Entity");
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string GetBaseClass()
        {
            return GetTypeName(DomainEventBaseTemplate.TemplateId);
        }

        private string GetConstructorParameters()
        {
            return this.GetMethodParameters(Model.Properties);
        }
    }
}