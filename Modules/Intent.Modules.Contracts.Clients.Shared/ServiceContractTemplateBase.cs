using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared;
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
            string dtoContractTemplateId,
            IEnumerable<string> additionalFolderParts = null,
            string typeNameSuffix = "Client")
            : base(templateId, outputTarget, model)
        {
            var additionalFolderPartsAsArray = additionalFolderParts?.ToArray() ?? Array.Empty<string>();
            
            AddTypeSource(enumContractTemplateId).WithCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(dtoContractTemplateId).WithCollectionFormatter(CSharpCollectionFormatter.CreateList());
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());

            CSharpFile = new CSharpFile(
                    @namespace: $"{this.GetNamespace(additionalFolderPartsAsArray)}",
                    relativeLocation: $"{this.GetFolderPath(additionalFolderPartsAsArray)}")
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .AddInterface($"I{Model.Name.RemoveSuffix("RestController", "Controller", "Service", "Client")}{typeNameSuffix}",
                    @interface =>
                    {
                        @interface.ImplementsInterfaces("IDisposable");

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
            var typeInfo = GetTypeInfo(endpoint.ReturnType);
            var typeName = UseType(typeInfo);
            if (!typeInfo.IsPrimitive)
            {
                typeName = $"{typeName}?";
            }

            return endpoint.ReturnType?.Element == null
                ? "Task"
                : $"Task<{typeName}>";
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

        public override RoslynMergeConfig ConfigureRoslynMerger() => ToFullyManagedUsingsMigration.GetConfig(Id, 2);
    }
}