using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.GenericStateStoreRepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.GenericStateStoreRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class GenericStateStoreRepositoryTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.StateManagement.GenericStateStoreRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public GenericStateStoreRepositoryTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.DaprClient);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"StateStoreRepository",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(ClassName)
                .WithPerServiceCallLifeTime()
                .ForInterface(this.GetGenericStateStoreRepositoryInterfaceName())
                .WithPriority(6)
                .ForConcern("Infrastructure")
                .HasDependency(this)
                .HasDependency(ExecutionContext.FindTemplateInstance<ITemplate>(GenericStateStoreRepositoryInterfaceTemplate.TemplateId)));
        }
    }
}