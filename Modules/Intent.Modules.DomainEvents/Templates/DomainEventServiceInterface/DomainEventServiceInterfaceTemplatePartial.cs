using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class DomainEventServiceInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.DomainEvents.DomainEventServiceInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEventServiceInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"IDomainEventService", @interface =>
                {
                    @interface.AddMethod("Task", "Publish", method =>
                    {
                        method
                            .AddParameter(GetDomainEventBaseType(), "domainEvent")
                            .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"))
                            ;
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IDomainEventService",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private string GetDomainEventBaseType()
        {
            return GetTypeName(DomainEventBaseTemplate.TemplateId);
        }
    }
}