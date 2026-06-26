using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.DomainEvents.Templates.DefaultDomainEventHandler
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class DefaultDomainEventHandlerTemplate : CSharpTemplateBase<DomainEventModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.DomainEvents.DefaultDomainEventHandler";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public DefaultDomainEventHandlerTemplate(IOutputTarget outputTarget, DomainEventModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateRoles.Domain.Events);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name}Handler", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });

                    @class.AddMethod("Task", "Handle", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                        method.Async();
                        method.AddParameter(GetTypeName(TemplateRoles.Domain.Events, Model), "domainEvent");
                        method.AddParameter("CancellationToken", "cancellationToken");
                    });
                })
                .AfterBuild(file =>
                {
                    var handleMethod = file.Classes.First().FindMethod("Handle");
                    if (handleMethod?.Statements.Count == 0)
                    {
                        handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyMerge();
                        handleMethod.AddStatement("// IntentInitialGen");
                        handleMethod.AddStatement($"// TODO: Implement Handle ({file.Classes.First().Name}) functionality");
                        handleMethod.AddStatement("throw new NotImplementedException(\"Implement your handler logic here...\");");
                    }
                }, 1000);
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Handler",
                @namespace: this.GetNamespace(),
                relativeLocation: this.GetFolderPath());
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
