using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Clients.Templates.EnumContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class EnumContractTemplate : CSharpTemplateBase<ServiceProxyEnumModel>
    {
        public const string TemplateId = "Intent.Application.Contracts.Clients.EnumContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EnumContractTemplate(IOutputTarget outputTarget, ServiceProxyEnumModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{((IntentTemplateBase)this).GetNamespace(Model.ServiceProxy.Name.ToPascalCase())}",
                relativeLocation: $"{((IntentTemplateBase)this).GetFolderPath(Model.ServiceProxy.Name.ToPascalCase())}");
        }
    }
}