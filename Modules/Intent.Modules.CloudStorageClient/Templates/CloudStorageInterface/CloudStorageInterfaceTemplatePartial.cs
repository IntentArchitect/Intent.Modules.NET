using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.CloudStorageClient.Templates.CloudStorageInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class CloudStorageInterfaceTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.CloudStorageClient.CloudStorageInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CloudStorageInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("AzureBlobStorage", "UseDevelopmentStorage=true"));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ICloudStorage",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}