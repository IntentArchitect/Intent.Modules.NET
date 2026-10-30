// The legacy IEventBus role is still honoured for applications on the old Eventing interface name.
#pragma warning disable CS0618
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

namespace Intent.Modules.Application.Mediator.Templates.MessageBusPublishBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageBusPublishBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.MessageBusPublishBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageBusPublishBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"MessageBusPublishBehaviour", @class =>
                {
                    @class.AddGenericParameter("TRequest", out var tRequest);
                    @class.AddGenericParameter("TResponse", out var tResponse);
                    @class.ImplementsInterface(UseType($"Mediator.IPipelineBehavior<{tRequest}, {tResponse}>"));
                    // Qualified because the Infrastructure project can also import NServiceBus.IMessage (a global using).
                    @class.AddGenericTypeConstraint(tRequest, c => c.AddType("global::Mediator.IMessage"));

                    var busVariableName = GetBusVariableName();

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(GetMessageBusInterfaceName(), busVariableName, param => param.IntroduceReadonlyField());
                        ctor.AddParameter(this.GetMessageBusFlushOwnerName(), "flushOwner", param => param.IntroduceReadonlyField());
                    });

                    @class.AddMethod($"{UseType("System.Threading.Tasks.ValueTask")}<{tResponse}>", "Handle", method =>
                    {
                        method.Async(asValueTask: true);
                        method.AddParameter(tRequest, "request");
                        method.AddParameter(UseType($"Mediator.MessageHandlerDelegate<{tRequest}, {tResponse}>"), "next");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddStatement("var isOwner = _flushOwner.Enter();");
                        method.AddTryBlock(tryBlock =>
                        {
                            tryBlock.AddStatement("var response = await next(request, cancellationToken);", s => s.SeparatedFromNext());
                            tryBlock.AddIfStatement("isOwner", @if =>
                            {
                                @if.AddStatement($"await _{busVariableName}.FlushAllAsync(cancellationToken);");
                            });
                            tryBlock.AddStatement("return response;", s => s.SeparatedFromPrevious());
                        });
                        method.AddFinallyBlock(finallyBlock =>
                        {
                            finallyBlock.AddStatement("_flushOwner.Exit();");
                        });
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   (TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out _) ||
                    TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out _));
        }

        private string GetBusVariableName()
        {
            // Legacy support first since both interfaces have the MessageBusInterface role assigned
            if (TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out _))
            {
                return "eventBus";
            }

            if (TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out _))
            {
                return "messageBus";
            }

            throw new InvalidOperationException(
                $"Could not find Message Bus interface with template role '{TemplateRoles.Application.Eventing.MessageBusInterface}' (or older legacy template with role '{TemplateRoles.Application.Eventing.EventBusInterface}').");
        }

        private string GetMessageBusInterfaceName()
        {
            // Legacy support first since both interfaces have the MessageBusInterface role assigned
            if (TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out var typeName))
            {
                return typeName;
            }

            if (TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out typeName))
            {
                return typeName;
            }

            throw new InvalidOperationException(
                $"Could not find Message Bus interface with template role '{TemplateRoles.Application.Eventing.MessageBusInterface}' (or older legacy template with role '{TemplateRoles.Application.Eventing.EventBusInterface}').");
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
