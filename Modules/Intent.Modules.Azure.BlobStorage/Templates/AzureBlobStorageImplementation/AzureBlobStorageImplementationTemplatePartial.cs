using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Azure.BlobStorage.Templates.BlobStorageInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Azure.BlobStorage.Templates.AzureBlobStorageImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class AzureBlobStorageImplementationTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Azure.BlobStorage.AzureBlobStorageImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureBlobStorageImplementationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AzureStorageBlobs(OutputTarget));
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("AzureBlobStorage", "UseDevelopmentStorage=true"));
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface(GetTemplate<IClassProvider>(BlobStorageInterfaceTemplate.TemplateId))
                .ForConcern("Infrastructure"));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AzureBlobStorage",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}