using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;

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
            string pagedResultTemplateId,
            IServiceProxyMappedService serviceProxyMappedService)
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
                        @interface.RepresentsModel(Model);
                        @interface.ImplementsInterfaces("IDisposable");

                        foreach (var endpoint in serviceProxyMappedService.GetMappedEndpoints(model))
                        {
                            @interface.AddMethod(GetTypeName(endpoint.ReturnType), GetOperationName(endpoint), method =>
                            {
                                method.Async();
                                var representativeOperation = model.Operations.SingleOrDefault(x => x.Mapping.ElementId == endpoint.Id);
                                if (representativeOperation != null)
                                {
                                    method.RepresentsModel(representativeOperation);
                                }

                                foreach (var input in endpoint.Inputs)
                                {
                                    method.AddParameter(this.GetTypeName(input), input.Name.ToParameterName());
                                }

                                method.AddOptionalCancellationTokenParameter(this);
                            });
                        }
                    });
        }

        private static string GetOperationName(MappedEndpoint endpoint)
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