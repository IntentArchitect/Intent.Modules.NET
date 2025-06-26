using System.Linq;
using System.Net;
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
    public abstract class ServiceContractTemplateBase : CSharpTemplateBase<IServiceContractModel>, ICSharpFileBuilderTemplate
    {
        protected ServiceContractTemplateBase(
            string templateId,
            IOutputTarget outputTarget,
            IServiceContractModel model,
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
                        @interface.RepresentsModel(Model);
                        @interface.ImplementsInterfaces("IDisposable");

                        foreach (var operation in model.Operations)
                        {
                            var operationName = operation.Name.ToPascalCase().EnsureSuffixedWith("Async");

                            @interface.AddMethod(GetTypeName(operation), operationName, method =>
                            {
                                method.Async();
                                method.RepresentsModel(operation);

                                foreach (var input in operation.Parameters)
                                {
                                    method.AddParameter(GetTypeName(input), input.Name.ToParameterName());
                                }

                                method.AddOptionalCancellationTokenParameter(this);
                            });
                        }
                    });
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