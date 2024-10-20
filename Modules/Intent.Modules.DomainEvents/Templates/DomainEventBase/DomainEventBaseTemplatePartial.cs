using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using JetBrains.Annotations;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.DomainEvents.Templates.DomainEventBase
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class DomainEventBaseTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.DomainEvents.DomainEventBase";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEventBaseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddClass($"DomainEvent", @class =>
                {
                    @class.Abstract();
                    @class.AddConstructor(ctor =>
                    {
                        ctor.Protected();
                        ctor.AddStatement("DateOccurred = DateTimeOffset.UtcNow;");
                    });

                    @class.AddProperty("bool", "IsPublished");
                    @class.AddProperty("DateTimeOffset", "DateOccurred", p => p.Setter.Protected());
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DomainEvent",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

    }
}