using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Intent.Engine;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
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
    public partial class DefaultDomainEventHandlerTemplate : CSharpTemplateBase<DomainEventModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.MediatR.DomainEvents.DefaultDomainEventHandler";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public DefaultDomainEventHandlerTemplate(IOutputTarget outputTarget, DomainEventModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("MediatR")
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddClass($"{Model.Name}Handler", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.ImplementsInterface($"INotificationHandler<{GetDomainEventNotificationType()}<{GetDomainEventType()}>>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });

                    @class.AddMethod("Task", "Handle", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                        method.Async();
                        method.AddParameter($"{GetDomainEventNotificationType()}<{GetDomainEventType()}>", "notification");
                        method.AddParameter("CancellationToken", "cancellationToken");
                    });
                }).AfterBuild(file =>
                {
                    var handleMethod = file.Classes.First().FindMethod("Handle");
                    if (handleMethod?.Statements.Count == 0)
                    {
                        handleMethod.AddStatement($"// TODO: Implement {handleMethod.Name} {file.Classes.First().Name}) functionality");
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
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
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