using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventNotification
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainEventNotificationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.DomainEvents.DomainEventNotification";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEventNotificationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("DomainEventNotification", @class =>
                {
                    @class.Abstract();
                    @class.AddGenericParameter("TDomainEvent", out var domainEventParameter);
                    @class.AddGenericTypeConstraint(domainEventParameter, constraint => constraint.AddType(GetTypeName(TemplateRoles.Domain.Common.EventBase)));
                    @class.ImplementsInterface(UseType("Mediator.INotification"));

                    @class.AddConstructor(ctor =>
                    {
                        ctor.Protected();
                        ctor.AddParameter("TDomainEvent", "domainEvent");
                        ctor.AddStatement("DomainEvent = domainEvent;");
                    });

                    @class.AddProperty("TDomainEvent", "DomainEvent", property => property.WithoutSetter());
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