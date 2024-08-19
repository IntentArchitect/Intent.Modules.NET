using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Azure.BlobStorage.Templates.BlobStorageExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class BlobStorageExtensionsTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.Azure.BlobStorage.BlobStorageExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BlobStorageExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"BlobStorageExtensions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetReadToEndMethodCall()
        {
            return OutputTarget switch
            {
                _ when OutputTarget.GetProject().IsNetApp(5) => "ReadToEndAsync()",
                _ when OutputTarget.GetProject().IsNetApp(6) => "ReadToEndAsync()",
                _ when OutputTarget.GetProject().TargetFramework().StartsWith("netstandard") => "ReadToEndAsync()",
                _ => "ReadToEndAsync(cancellationToken)"
            };
        }
    }
}