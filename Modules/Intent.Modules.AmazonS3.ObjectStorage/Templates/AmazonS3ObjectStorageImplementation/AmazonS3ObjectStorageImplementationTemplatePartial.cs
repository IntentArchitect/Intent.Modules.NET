using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.AmazonS3.ObjectStorage.Templates.ObjectStorageInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AmazonS3.ObjectStorage.Templates.AmazonS3ObjectStorageImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class AmazonS3ObjectStorageImplementationTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.AmazonS3.ObjectStorage.AmazonS3ObjectStorageImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AmazonS3ObjectStorageImplementationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.AWSSDKS3(outputTarget));
            AddNugetDependency(NugetPackages.AWSSDKExtensionsNETCoreSetup(outputTarget));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AmazonS3ObjectStorage",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        // public override void BeforeTemplateExecution()
        // {
        //     ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
        //         .ForInterface(GetTemplate<IClassProvider>(ObjectStorageInterfaceTemplate.TemplateId))
        //         .ForConcern("Infrastructure"));
        // }
    }
}