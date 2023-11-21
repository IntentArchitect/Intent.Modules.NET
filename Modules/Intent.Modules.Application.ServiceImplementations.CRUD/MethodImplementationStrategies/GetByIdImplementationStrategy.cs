using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies
{
    public class GetByIdImplementationStrategy : IImplementationStrategy
    {
        private readonly ServiceImplementationTemplate _template;

        public GetByIdImplementationStrategy(ServiceImplementationTemplate template, IApplication application)
        {
            _template = template;
        }

        public bool IsMatch(OperationModel operationModel)
        {
            if (operationModel.CreateEntityActions().Any()
                || operationModel.UpdateEntityActions().Any()
                || operationModel.DeleteEntityActions().Any()
                || operationModel.QueryEntityActions().Any())
            {
                return false;
            }

            if (operationModel.Parameters.Count != 1)
            {
                return false;
            }

            if (!operationModel.Parameters.Any(p => p.Name.Contains("id", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            var dtoModel = operationModel.TypeReference.Element?.AsDTOModel();
            if (dtoModel == null)
            {
                return false;
            }

            var domainModel = dtoModel.Mapping?.Element?.AsClassModel();
            if (domainModel == null)
            {
                return false;
            }

            if (!_template.TryGetTemplate<ITemplate>(TemplateFulfillingRoles.Repository.Interface.Entity, domainModel, out _))
            {
                return false;
            }

            var lowerOperationName = operationModel.Name.ToLower();
            return new[] { "get", "find", domainModel.Name }.Any(x => lowerOperationName.Contains(x));
        }

        public void ApplyStrategy(OperationModel operationModel)
        {
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");

            var dtoModel = operationModel.TypeReference.Element.AsDTOModel();
            var dtoType = _template.TryGetTypeName(DtoModelTemplate.TemplateId, dtoModel, out var dtoName)
                ? dtoName
                : dtoModel.Name.ToPascalCase();
            var domainModel = dtoModel.Mapping.Element.AsClassModel();
            var repositoryTypeName = _template.GetTypeName(TemplateFulfillingRoles.Repository.Interface.Entity, domainModel);
            var repositoryParameterName = repositoryTypeName.Split('.').Last()[1..].ToLocalVariableName();
            var repositoryFieldName = repositoryParameterName.ToPrivateMemberName();

            var codeLines = new CSharpStatementAggregator();
            codeLines.Add($@"var element ={(operationModel.IsAsync() ? " await" : "")} {repositoryFieldName}.FindById{(operationModel.IsAsync() ? "Async" : "")}({operationModel.Parameters.First().Name.ToCamelCase()}, cancellationToken);");
            codeLines.Add(new CSharpIfStatement($"element is null")
                .AddStatement($@"throw new {_template.GetNotFoundExceptionName()}($""Could not find {domainModel.Name.ToPascalCase()} {{{operationModel.Parameters.First().Name.ToCamelCase()}}}"");"));
            codeLines.Add($@"return element.MapTo{dtoType}(_mapper);");

            var @class = _template.CSharpFile.Classes.First();
            var method = @class.FindMethod(m => m.Name.Equals(operationModel.Name, StringComparison.OrdinalIgnoreCase));
            var attr = method.Attributes.OfType<CSharpIntentManagedAttribute>().FirstOrDefault();
            if (attr == null)
            {
                attr = CSharpIntentManagedAttribute.Fully();
                method.AddAttribute(attr);
            }

            attr.WithBodyFully();
            method.Statements.Clear();
            method.AddStatements(codeLines.ToList());

            var ctor = @class.Constructors.First();
            if (ctor.Parameters.All(p => p.Name != repositoryParameterName))
            {
                ctor.AddParameter(repositoryTypeName, repositoryParameterName, parameter => parameter.IntroduceReadonlyField());
            }
            if (ctor.Parameters.All(p => p.Name != "mapper"))
            {
                ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", parameter => parameter.IntroduceReadonlyField());
            }
        }
    }
}