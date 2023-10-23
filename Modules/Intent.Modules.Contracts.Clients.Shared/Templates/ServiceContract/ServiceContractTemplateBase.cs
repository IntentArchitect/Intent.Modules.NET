using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract
{
    public abstract class ServiceContractTemplateBase : CSharpTemplateBase<ServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        protected ServiceContractTemplateBase(
            string templateId,
            IOutputTarget outputTarget,
            ServiceProxyModel model,
            string dtoContractTemplateId,
            string enumContractTemplateId,
            string pagedResultTemplateId)
            : base(templateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            PagedResultTypeSource.ApplyTo(this, pagedResultTemplateId);
            AddTypeSource(dtoContractTemplateId);
            AddTypeSource(enumContractTemplateId);

            CSharpFile = new CSharpFile(
                    @namespace: $"{this.GetNamespace()}",
                    relativeLocation: $"{this.GetFolderPath()}")
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .AddInterface($"I{Model.Name}",
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

        public override RoslynMergeConfig ConfigureRoslynMerger() => ToFullyManagedUsingsMigration.GetConfig(Id, 2);
    }
}