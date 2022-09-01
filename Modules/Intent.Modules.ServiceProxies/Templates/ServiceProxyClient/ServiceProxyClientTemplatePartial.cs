using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.RoslynWeaver.Attributes;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ServiceProxies.Templates.ServiceProxyClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ServiceProxyClientTemplate : CSharpTemplateBase<ServiceProxyModel>
    {
        public const string TemplateId = "Intent.ServiceProxies.ServiceProxyClient";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceProxyClientTemplate(IOutputTarget outputTarget, ServiceProxyModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MicrosoftExtensionsHttp);
            AddNugetDependency(NuGetPackages.MicrosoftAspNetCoreWebUtilities);
            AddTypeSource(ServiceContractTemplate.TemplateId);
            AddTypeSource(DtoContractTemplate.TemplateId).WithCollectionFormat("List<{0}>");

            WebApiQueries.Validate(model);
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.Create("List<{0}>"));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private bool UseExplicitNullSymbol => Project.GetProject().NullableEnabled;
        private string NotNull => UseExplicitNullSymbol ? "!" : string.Empty;

        private string GetReturnType(OperationModel operation)
        {
            if (operation.ReturnType == null)
            {
                return "Task";
            }

            return $"Task<{GetTypeName(operation.ReturnType)}>";
        }

        private string GetOperationName(OperationModel operation)
        {
            return operation.Name.ToPascalCase();
        }

        private string GetOperationParameters(OperationModel operation)
        {
            var parameters = new List<string>();

            parameters.AddRange(operation.Parameters.Select(s => $"{GetTypeName(s.TypeReference)} {s.Name.ToParameterName()}"));
            parameters.Add($"CancellationToken cancellationToken = default");

            return string.Join(", ", parameters);
        }

        private string GetRelativeUri(OperationModel operation)
        {
            var relativeUri = WebApiQueries.GetRelativeUri(operation);
            return relativeUri;
        }

        private bool HasQueryParameter(OperationModel operation)
        {
            return WebApiQueries.GetQueryParameters(operation).Any();
        }

        private IReadOnlyCollection<ParameterModel> GetQueryParameters(OperationModel operation)
        {
            return WebApiQueries.GetQueryParameters(operation);
        }

        private string GetHttpVerb(OperationModel operation)
        {
            return $"HttpMethod.{operation.GetHttpSettings()?.Verb().Value.ToLower().ToPascalCase()}";
        }

        private IReadOnlyCollection<WebApiQueries.HeaderParameter> GetHeaderParameters(OperationModel operation)
        {
            return WebApiQueries.GetHeaderParameters(operation);
        }

        private bool HasBodyParameter(OperationModel operation)
        {
            return WebApiQueries.GetBodyParameter(operation) != null;
        }

        private string GetBodyParameterName(OperationModel operation)
        {
            return WebApiQueries.GetBodyParameter(operation).Name.ToParameterName();
        }

        private bool HasFormUrlEncodedParameter(OperationModel operation)
        {
            return WebApiQueries.GetFormUrlEncodedParameters(operation).Any();
        }

        private IReadOnlyCollection<ParameterModel> GetFormUrlEncodedParameters(OperationModel operation)
        {
            return WebApiQueries.GetFormUrlEncodedParameters(operation);
        }

        private bool HasResponseType(OperationModel operation)
        {
            return operation.ReturnType != null;
        }
    }
}