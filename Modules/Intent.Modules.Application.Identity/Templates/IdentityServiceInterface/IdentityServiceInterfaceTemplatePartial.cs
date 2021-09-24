using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates.ResultModel;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Identity.Templates.IdentityServiceInterface
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class IdentityServiceInterfaceTemplate : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Identity.IdentityServiceInterface";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public IdentityServiceInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"IIdentityService",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string GetResultModel()
        {
            return GetTypeName(ResultModelTemplate.TemplateId);
        }
    }
}