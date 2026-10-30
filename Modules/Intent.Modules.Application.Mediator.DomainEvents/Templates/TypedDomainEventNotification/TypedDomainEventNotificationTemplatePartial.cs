using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventNotification;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.DomainEvents.Templates.TypedDomainEventNotification
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TypedDomainEventNotificationTemplate : CSharpTemplateBase<DomainEventModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.DomainEvents.TypedDomainEventNotification";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TypedDomainEventNotificationTemplate(IOutputTarget outputTarget, DomainEventModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateRoles.Domain.Events);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}Notification", @class =>
                {
                    var domainEventType = GetTypeName(TemplateRoles.Domain.Events, Model);

                    @class.Sealed();
                    @class.WithBaseType($"{GetTypeName(DomainEventNotificationTemplate.TemplateId)}<{domainEventType}>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(domainEventType, "domainEvent");
                        ctor.CallsBase(callsBase => callsBase.AddArgument("domainEvent"));
                    });
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
