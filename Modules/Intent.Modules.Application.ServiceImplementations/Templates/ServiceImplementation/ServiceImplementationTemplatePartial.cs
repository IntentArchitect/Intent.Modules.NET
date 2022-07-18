using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using ModelHasFolderTemplateExtensions = Intent.Modules.Common.CSharp.Templates.ModelHasFolderTemplateExtensions;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ServiceImplementationTemplate : CSharpTemplateBase<ServiceModel, ServiceImplementationDecorator>
    {
        public const string TemplateId = "Intent.Application.ServiceImplementations.ServiceImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceImplementationTemplate(IOutputTarget outputTarget, ServiceModel model)
            : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name.RemoveSuffix("RestController", "Controller", "Service")}Service",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: ModelHasFolderTemplateExtensions.GetFolderPath(this));
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Application")
                .ForInterface(GetTemplate<IClassProvider>(ServiceContractTemplate.TemplateId, Model)));
        }

        private string GetOperationDefinitionParameters(OperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(OperationModel o)
        {
            if (o.TypeReference.Element == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{GetTypeName(o.TypeReference)}>" : GetTypeName(o.TypeReference);
        }

        public string GetServiceInterfaceName()
        {
            var serviceContractTemplate = OutputTarget.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel<ServiceModel>(ServiceContractTemplate.TemplateId, x => x.Id == Model.Id));
            return GetTypeName(serviceContractTemplate);
        }

        private IReadOnlyCollection<ConstructorParameter> GetConstructorDependencies()
        {
            var parameters = GetDecorators()
                .SelectMany(s => s.GetConstructorDependencies())
                .Distinct()
                .ToArray();
            return parameters;
        }

        private string GetImplementation(OperationModel operation)
        {
            var output = GetDecorators().Aggregate(x => x.GetDecoratedImplementation(operation));
            if (string.IsNullOrWhiteSpace(output))
            {
                return @"
            throw new NotImplementedException(""Write your implementation for this service here..."");";
            }
            return output;
        }
    }
}
