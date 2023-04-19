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
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ServiceContractTemplate : CSharpTemplateBase<ServiceProxyModel>
    {
        public const string TemplateId = "Intent.Application.Contracts.Clients.ServiceContract";
        private readonly IReadOnlyCollection<IHttpEndpointModel> _endpoints;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceContractTemplate(IOutputTarget outputTarget, ServiceProxyModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(EnumContractTemplate.TemplateId, "List<{0}>");
            AddTypeSource(DtoContractTemplate.TemplateId).WithCollectionFormatter(CSharpCollectionFormatter.CreateList());
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());

            _endpoints = model.GetMappedEndpoints().ToArray();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name.RemoveSuffix("RestController", "Controller", "Service", "Client")}Client",
                @namespace: $"{this.GetNamespace(Model.Name.ToPascalCase())}",
                relativeLocation: $"{this.GetFolderPath(Model.Name.ToPascalCase())}");
        }

        private string GetOperationDefinitionParameters(IHttpEndpointModel endpoint)
        {
            var parameters = endpoint.Inputs
                .Select(s => $"{GetTypeName(s.TypeReference)} {s.Name}")
                .Append("CancellationToken cancellationToken = default");

            return string.Join(", ", parameters);
        }

        private string GetOperationReturnType(IHttpEndpointModel endpoint)
        {
            return endpoint.ReturnType == null
                ? "Task"
                : $"Task<{GetTypeName(endpoint.ReturnType)}>";
        }

        private static string GetOperationName(IHttpEndpointModel endpoint)
        {
            return $"{endpoint.Name.ToPascalCase().RemoveSuffix("Async")}Async";
        }
    }
}