using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.EnumContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class EnumContractTemplate : CSharpTemplateBase<EnumModel>
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.EnumContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EnumContractTemplate(IOutputTarget outputTarget, EnumModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}