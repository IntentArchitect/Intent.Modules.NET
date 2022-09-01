using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CloudStorageClient.Templates.AzureBlobStorageImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class AzureBlobStorageImplementationTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.CloudStorageClient.AzureBlobStorageImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureBlobStorageImplementationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.AzureStorageBlobs);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"AzureBlobStorageImplementation",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}