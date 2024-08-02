using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreGenericRepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreGenericRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class DaprStateStoreGenericRepositoryTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreGenericRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprStateStoreGenericRepositoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.DaprClient(outputTarget));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"DaprStateStoreGenericRepository",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(ClassName)
                .WithPerServiceCallLifeTime()
                .ForInterface(this.GetDaprStateStoreGenericRepositoryInterfaceName())
                .WithPriority(6)
                .ForConcern("Infrastructure")
                .HasDependency(this)
                .HasDependency(ExecutionContext.FindTemplateInstance<ITemplate>(DaprStateStoreGenericRepositoryInterfaceTemplate.TemplateId)));
        }
    }
}