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

namespace Intent.Modules.Application.Wolverine.Templates.MessageBusFlushMiddleware
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageBusFlushMiddlewareTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.MessageBusFlushMiddleware";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageBusFlushMiddlewareTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("MessageBusFlushMiddleware", @class =>
                {
                    var busVariableName = GetBusVariableName(this);
                    var messageBusInterface = GetMessageBusInterfaceName(this);

                    @class.AddMethod(UseType("System.Threading.Tasks.Task"), "AfterAsync", method =>
                    {
                        method.Static().Async();
                        method.AddParameter(messageBusInterface, busVariableName);
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddStatement($"await {busVariableName}.FlushAllAsync(cancellationToken);");
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

        public override bool CanRunTemplate()
        {
            return TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out _) ||
                   TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out _);
        }

        private static string GetBusVariableName(IIntentTemplate template)
        {
            // Legacy support first since both interfaces have the MessageBusInterface role assigned
            if (template.TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out _))
            {
                return "eventBus";
            }

            if (template.TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out _))
            {
                return "messageBus";
            }

            throw new InvalidOperationException(
                $"Could not find Message Bus interface with template role '{TemplateRoles.Application.Eventing.MessageBusInterface}' (or older legacy template with role '{TemplateRoles.Application.Eventing.EventBusInterface}').");
        }

        private static string GetMessageBusInterfaceName(IIntentTemplate template)
        {
            // Legacy support first since both interfaces have the MessageBusInterface role assigned
            if (template.TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out var typeName))
            {
                return typeName;
            }

            if (template.TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out typeName))
            {
                return typeName;
            }

            throw new InvalidOperationException(
                $"Could not find Message Bus interface with template role '{TemplateRoles.Application.Eventing.MessageBusInterface}' (or older legacy template with role '{TemplateRoles.Application.Eventing.EventBusInterface}').");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}