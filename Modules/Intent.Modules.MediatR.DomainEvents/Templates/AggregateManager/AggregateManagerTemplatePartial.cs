using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Templates.AggregateManager
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class AggregateManagerTemplate : CSharpTemplateBase<ClassModel>
    {
        public const string TemplateId = "Intent.MediatR.DomainEvents.AggregateManager";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AggregateManagerTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Manager",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetInterfaces()
        {
            return string.Join(", ", GetDomainEventModels().Select(e => $"INotificationHandler<{this.GetDomainEventNotificationName()}<{this.GetDomainEventName(e)}>>"));
        }

        private IEnumerable<DomainEventModel> GetDomainEventModels()
        {
            return Model.AssociatedDomainEvents().Select(x => x.Element.AsDomainEventModel());
        }
    }
}