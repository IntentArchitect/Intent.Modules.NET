using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Clients.Templates.DtoContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class DtoContractTemplate : CSharpTemplateBase<ServiceProxyDTOModel>
    {
        public const string TemplateId = "Intent.Application.Contracts.Clients.DtoContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DtoContractTemplate(IOutputTarget outputTarget, ServiceProxyDTOModel model) : base(TemplateId, outputTarget, model)
        {
            AddAssemblyReference(new GacAssemblyReference("System.Runtime.Serialization"));
            AddTypeSource(TemplateId, "List<{0}>");
            AddTypeSource("Domain.Enum", "List<{0}>");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public string GenericTypes => Model.GenericTypes.Any() ? $"<{string.Join(", ", Model.GenericTypes)}>" : "";

        public string ConstructorParameters()
        {
            var parameters = new List<string>();

            foreach (var field in Model.Fields)
            {
                parameters.Add($"{GetTypeName(field.TypeReference)} {field.Name.ToParameterName()}");
            }

            const string newLine = @"
            ";
            return newLine + string.Join($",{newLine}", parameters);
        }
    }
}