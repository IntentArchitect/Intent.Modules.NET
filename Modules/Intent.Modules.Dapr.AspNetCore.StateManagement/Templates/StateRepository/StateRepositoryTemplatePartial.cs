using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.StateRepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.StateRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class StateRepositoryTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.StateManagement.StateRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public StateRepositoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.DaprClient);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"StateRepository",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(ClassName)
                .WithPerServiceCallLifeTime()
                .ForInterface(this.GetStateRepositoryInterfaceName())
                .WithPriority(6)
                .ForConcern("Infrastructure")
                .HasDependency(this)
                .HasDependency(ExecutionContext.FindTemplateInstance<ITemplate>(StateRepositoryInterfaceTemplate.TemplateId)));
        }
    }
}