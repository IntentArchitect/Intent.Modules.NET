using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    partial class ServiceImplementationTemplate : CSharpTemplateBase<ServiceModel>, ITemplate, IHasTemplateDependencies, ITemplateBeforeExecutionHook, IHasDecorators<ServiceImplementationDecoratorBase>, ITemplatePostCreationHook
    {
        private readonly IList<ServiceImplementationDecoratorBase> _decorators = new List<ServiceImplementationDecoratorBase>();

        public const string Identifier = "Intent.Application.ServiceImplementations";
        public ServiceImplementationTemplate(IOutputTarget outputTarget, ServiceModel model)
            : base(Identifier, outputTarget, model)
        {
            AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
        }

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return base.GetTemplateDependencies()
                .Concat(new[]
                {
                    TemplateDependency.OnModel<ServiceModel>(ServiceContractTemplate.TemplateId, x => x.Id == Model.Id)
                })
                .Concat(GetDecorators()
                    .SelectMany(x => x.GetConstructorDependencies(Model)
                        .Select(d => d.TemplateDependency)));
        }

        public void AddDecorator(ServiceImplementationDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<ServiceImplementationDecoratorBase> GetDecorators()
        {
            return _decorators;
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Application")
                .ForInterface(GetTemplate<IClassProvider>(ServiceContractTemplate.TemplateId, Model)));
        }

        public override string DependencyUsings
        {
            get
            {
                var builder = new StringBuilder(base.DependencyUsings).AppendLine();
                var additionalUsings = GetDecorators()
                    .SelectMany(s => s.GetUsings(Model))
                    .Distinct()
                    .Where(p => !string.IsNullOrEmpty(p))
                    .ToArray();
                foreach (var @using in additionalUsings)
                {
                    builder.AppendLine($"using {@using};");
                }

                return builder.ToString();
            }
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
            var serviceContractTemplate = Project.Application.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel<ServiceModel>(ServiceContractTemplate.TemplateId, x => x.Id == Model.Id));
            return $"{serviceContractTemplate.Namespace}.{serviceContractTemplate.ClassName}";
        }

        private IEnumerable<ConstructorParameter> GetConstructorDependencies()
        {
            var parameters = GetDecorators()
                .SelectMany(s => s.GetConstructorDependencies(this.Model))
                .Distinct()
                .ToArray();
            return parameters;
        }

        private string GetImplementation(OperationModel operation)
        {
            var output = GetDecorators().Aggregate(x => x.GetDecoratedImplementation(Model, operation));
            if (string.IsNullOrWhiteSpace(output))
            {
                return @"
            throw new NotImplementedException(""Write your implementation for this service here..."");";
            }
            return output;
        }
    }
}
