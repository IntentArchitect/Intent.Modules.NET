using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours.Templates.MessageBusPublishBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageBusPublishBehaviourTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.Behaviours.MessageBusPublishBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageBusPublishBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("MessageBusPublishBehaviour", @class =>
                {
                    @class.AddGenericParameter("TRequest", out var tRequest);
                    @class.AddGenericParameter("TResponse", out var tResponse);
                    @class.AddGenericTypeConstraint(tRequest, c => c.AddType("notnull"));
                    @class.ImplementsInterface($"MediatR.IPipelineBehavior<{tRequest}, {tResponse}>");
            
                    var busVariableName = GetBusVariableName(this);
                    
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(GetMessageBusInterfaceName(this), busVariableName, param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    @class.AddMethod(UseType("System.Threading.Tasks.Task") + $"<{tResponse}>", "Handle", method =>
                    {
                        method.Async();
                        method.AddParameter(tRequest, "request");
                        method.AddParameter($"MediatR.RequestHandlerDelegate<{tResponse}>", "next");
                        method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken");

                        method.AddStatement($"var response = await next({GetCancellationTokenArgument()});", cfg => cfg.SeparatedFromNext());
                        method.AddStatement($"await _{busVariableName}.FlushAllAsync(cancellationToken);", cfg => cfg.SeparatedFromNext());
                        method.AddReturn("response");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        private string GetCancellationTokenArgument()
        {
            return Project.TryGetMaxNetAppVersion(out var version) &&
                   version.Major is <= 2 or > 6
                ? "cancellationToken"
                : string.Empty;
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

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate())
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister($"typeof({ClassName}<,>)")
                .ForInterface("typeof(IPipelineBehavior<,>)")
                .WithPriority(4)
                .ForConcern("MediatR")
                .HasDependency(this));
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}