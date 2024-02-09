using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.DtoContract;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.EnumContract;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.PagedResult;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.ProxyServiceContract
{
    [IntentManaged(Mode.Ignore)]
    public partial class ProxyServiceContractTemplate : CSharpTemplateBase<IServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.ProxyServiceContract";

        public ProxyServiceContractTemplate(IOutputTarget outputTarget, IServiceProxyModel model) : base(TemplateId, outputTarget, model)
        {
            var dtoContractTemplateId = DtoContractTemplate.TemplateId;
            var enumContractTemplateId = EnumContractTemplate.TemplateId;
            var pagedResultTemplateId = PagedResultTemplate.TemplateId;

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
                .AddInterface($"I{Model.Name.RemoveSuffix("Http", "Client", "Service")}Service",
                    @interface =>
                    {
                        @interface.RepresentsModel(Model);
                        @interface.ImplementsInterfaces("IDisposable");

                        foreach (var endpoint in model.GetMappedEndpoints().ToArray())
                        {
                            @interface.AddMethod(GetTypeName(endpoint.ReturnType), GetOperationName(endpoint), method =>
                            {
                                method.Async();

                                foreach (var input in endpoint.Inputs)
                                {
                                    method.AddParameter(GetTypeName(input.TypeReference), input.Name.ToParameterName());
                                }

                                method.AddOptionalCancellationTokenParameter(this);
                            });
                        }
                    });
        }

        private static string GetOperationName(IHttpEndpointModel endpoint)
        {
            return endpoint.Name.ToPascalCase().EnsureSuffixedWith("Async");
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}