using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.EnumContract;
using Intent.Modules.Application.Contracts.Clients.Templates.PagedResult;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Fakes.Templates.HttpClientFake
{
    [IntentManaged(Mode.Ignore)]
    public class HttpClientFakeTemplate : CSharpTemplateBase<IServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Integration.HttpClients.Fakes.HttpClientFake";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientFakeTemplate(IOutputTarget outputTarget, IServiceProxyModel model)
            : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            PagedResultTypeSource.ApplyTo(this, PagedResultTemplate.TemplateId);
            AddTypeSource(ServiceContractTemplate.TemplateId);
            AddTypeSource(DtoContractTemplate.TemplateId);
            AddTypeSource(EnumContractTemplate.TemplateId);

            CSharpFile = new CSharpFile(
                    @namespace: this.GetNamespace(),
                    relativeLocation: this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .IntentManagedFully()
                .AddClass($"{Model.Name.RemoveSuffix("Http", "Client")}HttpClientFake", @class =>
                {
                    @class.RepresentsModel(Model);
                    @class.ImplementsInterface(GetTypeName(ServiceContractTemplate.TemplateId, Model));

                    foreach (var endpoint in Model.Endpoints)
                    {
                        @class.AddMethod(GetReturnType(endpoint), $"{endpoint.Name.ToPascalCase().RemoveSuffix("Async")}Async", method =>
                        {
                            method
                                .Async()
                                .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored());

                            if (Model.UnderlyingModel is ServiceProxyModel serviceProxyModel && serviceProxyModel.Operations.Any())
                            {
                                var operationModel = serviceProxyModel.Operations.Single(x => x.Mapping?.ElementId == endpoint.Id);
                                method.RepresentsModel(operationModel);
                            }

                            if (Model.CreateParameterPerInput)
                            {
                                foreach (var input in endpoint.Inputs)
                                {
                                    method.AddParameter(GetTypeName(input.TypeReference), input.Name.ToParameterName());
                                }
                            }
                            else
                            {
                                var fields = endpoint.InternalElement.ChildElements.Where(x => x.IsDTOFieldModel()).ToArray();

                                switch (fields.Length)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        method.AddParameter(GetTypeName(fields[0].TypeReference), fields[0].Name.ToParameterName());
                                        break;
                                    default:
                                        var parameterName = endpoint.InternalElement.SpecializationTypeId switch
                                        {
                                            CommandModel.SpecializationTypeId => "command",
                                            QueryModel.SpecializationTypeId => "query",
                                            _ => endpoint.InternalElement.Name.ToParameterName()
                                        };
                                        method.AddParameter(GetTypeName(endpoint.InternalElement), parameterName);
                                        break;
                                }
                            }

                            method.AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"));
                            method.AddStatement($"throw new {UseType("System.NotImplementedException")}();");
                        });
                    }

                    @class.AddMethod("void", "Dispose", method =>
                    {
                    });

                });
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

        private string GetReturnType(IHttpEndpointModel endpoint)
        {
            return endpoint.ReturnType?.Element == null
                ? "Task"
                : $"Task<{GetTypeName(endpoint.ReturnType)}>";
        }
    }
}
