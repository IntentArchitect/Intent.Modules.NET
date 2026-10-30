using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventHandlerSkill;
using Intent.Modules.Application.Mediator.DomainEvents.Templates.TypedDomainEventNotification;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.DomainEvents.Templates.DefaultDomainEventHandler
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DefaultDomainEventHandlerTemplate : CSharpTemplateBase<DomainEventModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.DomainEvents.DefaultDomainEventHandler";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DefaultDomainEventHandlerTemplate(IOutputTarget outputTarget, DomainEventModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddClass($"{Model.Name}Handler", @class =>
                {
                    var notificationType = GetTypeName(TypedDomainEventNotificationTemplate.TemplateId, Model);

                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.ImplementsInterface($"{UseType("Mediator.INotificationHandler")}<{notificationType}>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.ValueTask"), "Handle", method =>
                    {
                        method.Async(asValueTask: true);
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyMerge());
                        method.AddParameter(notificationType, "notification");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");
                        method.AddStatement("// IntentInitialGen");
                        method.AddStatement($"// TODO: Implement {method.Name} ({@class.Name}) functionality");
                        method.AddStatement("""throw new NotImplementedException("Your implementation here...");""");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig()
                .WithAISummary($"Default Mediator handler for the {Model.Name} domain event.")
                .WithAIContext(
                    $"""
                    Use the `{DomainEventHandlerSkillTemplate.SkillName}` skill when modifying this handler.
                    """);
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
