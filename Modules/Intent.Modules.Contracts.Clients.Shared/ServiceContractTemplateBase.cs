using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Contracts.Clients.Shared
{
    public abstract class ServiceContractTemplateBase : CSharpTemplateBase<ServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        protected ServiceContractTemplateBase(
            string templateId,
            IOutputTarget outputTarget,
            ServiceProxyModel model,
            string enumContractTemplateId,
            string dtoContractTemplateId)
            : base(templateId, outputTarget, model)
        {
            AddTypeSource(enumContractTemplateId).WithCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(dtoContractTemplateId).WithCollectionFormatter(CSharpCollectionFormatter.CreateList());
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());

            CSharpFile = new CSharpFile(
                    @namespace: $"{this.GetNamespace(Model.Name.ToPascalCase())}",
                    relativeLocation: $"{this.GetFolderPath(Model.Name.ToPascalCase())}")
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"I{Model.Name.RemoveSuffix("RestController", "Controller", "Service", "Client")}Client",
                    @interface =>
                    {
                        foreach (var endpoint in model.GetMappedEndpoints().ToArray())
                        {
                            @interface.AddMethod(GetOperationReturnType(endpoint), GetOperationName(endpoint), method =>
                            {
                                foreach (var input in endpoint.Inputs)
                                {
                                    method.AddParameter(GetTypeName(input.TypeReference), input.Name.ToParameterName());
                                }

                                method.AddOptionalCancellationTokenParameter(this);
                            });
                        }
                    });
        }

        private string GetOperationReturnType(IHttpEndpointModel endpoint)
        {
            return endpoint.ReturnType?.Element == null
                ? "Task"
                : $"Task<{GetTypeName(endpoint.ReturnType)}>";
        }

        private static string GetOperationName(IHttpEndpointModel endpoint)
        {
            return endpoint.Name.ToPascalCase().EnsureSuffixedWith("Async");
        }

        public CSharpFile CSharpFile { get; }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}