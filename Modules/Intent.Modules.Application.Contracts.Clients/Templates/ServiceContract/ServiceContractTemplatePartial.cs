using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.EnumContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ServiceContractTemplate : CSharpTemplateBase<ServiceProxyModel>
    {
        public const string TemplateId = "Intent.Application.Contracts.Clients.ServiceContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceContractTemplate(IOutputTarget outputTarget, ServiceProxyModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(EnumContractTemplate.TemplateId, "List<{0}>");
            AddTypeSource(DtoContractTemplate.TemplateId).WithCollectionFormatter(CSharpCollectionFormatter.CreateList());
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name.RemoveSuffix("RestController", "Controller", "Service", "Client")}Client",
                @namespace: $"{this.GetNamespace(Model.Name.ToPascalCase())}",
                relativeLocation: $"{this.GetFolderPath(Model.Name.ToPascalCase())}");
        }

        private string GetOperationDefinitionParameters(OperationModel o)
        {
            var parameters = new List<string>();

            parameters.AddRange(o.Parameters.Select(s => $"{GetTypeName(s.TypeReference)} {s.Name}"));
            parameters.Add("CancellationToken cancellationToken = default");

            return string.Join(", ", parameters);
        }

        private string GetOperationReturnType(OperationModel o)
        {
            if (o.ReturnType == null)
            {
                return "Task";
            }

            return $"Task<{GetTypeName(o.ReturnType)}>";
        }

        private string GetOperationName(OperationModel operation)
        {
            return $"{operation.Name.ToPascalCase().RemoveSuffix("Async")}Async";
        }
    }
}