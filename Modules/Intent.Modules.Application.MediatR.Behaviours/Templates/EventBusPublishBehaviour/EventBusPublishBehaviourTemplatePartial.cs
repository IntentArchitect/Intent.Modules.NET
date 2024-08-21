using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Behaviours.Templates.EventBusPublishBehaviour
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class EventBusPublishBehaviourTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Application.MediatR.Behaviours.EventBusPublishBehaviour";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EventBusPublishBehaviourTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"EventBusPublishBehaviour",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.FindTemplateInstance<IClassProvider>(TemplateRoles.Application.Eventing.EventBusInterface) != null;
        }

        private string GetEventBusInterfaceName()
        {
            return GetTypeName(TemplateRoles.Application.Eventing.EventBusInterface);
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
                .RequiresUsingNamespaces("MediatR")
                .HasDependency(this));
        }
    }
}