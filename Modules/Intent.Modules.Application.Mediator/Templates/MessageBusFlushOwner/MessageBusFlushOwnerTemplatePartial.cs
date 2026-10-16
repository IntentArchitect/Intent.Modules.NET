// The legacy IEventBus role is still honoured for applications on the old Eventing interface name.
#pragma warning disable CS0618
using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.Templates.MessageBusFlushOwner
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessageBusFlushOwnerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Mediator.MessageBusFlushOwner";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageBusFlushOwnerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatorAbstractions(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"MessageBusFlushOwner", @class =>
                {
                    var messageBusInterface = TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out var legacyBus)
                        ? legacyBus
                        : GetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface);

                    @class.Sealed();
                    @class.WithComments(new[]
                    {
                        "/// <summary>",
                        $"/// Decides who flushes the scoped <see cref=\"{messageBusInterface}\"/> buffer. Dispatches nest: a",
                        "/// domain-event handler or an integration consumer sends further commands into the same DI",
                        "/// scope, and they share one buffer. Only the outermost owner may flush, once its unit of work",
                        "/// has committed. A nested dispatch that flushed would send the outer work's messages before",
                        "/// the outer commit.",
                        "/// </summary>",
                    });

                    @class.AddField("int", "_depth");

                    @class.AddMethod("bool", "Enter", method =>
                    {
                        method.WithComments("/// <summary>Returns true when the caller is the outermost owner and must flush.</summary>");
                        method.WithExpressionBody($"{UseType("System.Threading.Interlocked")}.Increment(ref _depth) == 1");
                    });

                    @class.AddMethod("void", "Exit", method =>
                    {
                        method.WithExpressionBody($"{UseType("System.Threading.Interlocked")}.Decrement(ref _depth)");
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() &&
                   (TryGetTypeName(TemplateRoles.Application.Eventing.EventBusInterface, out _) ||
                    TryGetTypeName(TemplateRoles.Application.Eventing.MessageBusInterface, out _));
        }

        public override void BeforeTemplateExecution()
        {
            if (!CanRunTemplate())
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Application")
                .WithPerServiceCallLifeTime());
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
