using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.HashiCorp.Vault.Templates.HashiCorpVaultOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class HashiCorpVaultOptionsTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "Intent.HashiCorp.Vault.HashiCorpVaultOptions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HashiCorpVaultOptionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"HashiCorpVaultOptions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public string Nullable => OutputTarget.GetProject().NullableEnabled ? "?" : string.Empty;
    }
}