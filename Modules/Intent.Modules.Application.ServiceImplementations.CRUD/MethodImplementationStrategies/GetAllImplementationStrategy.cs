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
    public class GetAllImplementationStrategy : IImplementationStrategy
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public GetAllImplementationStrategy(ICSharpFileBuilderTemplate template, IApplication application)
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
            
            if (_template.CSharpFile.Classes.First()
                    .FindMethod(m => m.TryGetMetadata<OperationModel>("model", out var model) && model.Id == operationModel.Id) is null)
            {
                return false;
            }

            if (operationModel.Parameters.Any())
            {
                return false;
            }

            if (operationModel.TypeReference?.IsCollection != true)
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

            if (!_template.TryGetTemplate<ITemplate>(TemplateRoles.Repository.Interface.Entity, domainModel, out _))
            {
                return false;
            }

            var lowerOperationName = operationModel.Name.ToLower();
            return new[] { "get", "find" }.Any(x => lowerOperationName.Contains(x));
        }

        public void BindToTemplate(ICSharpFileBuilderTemplate template, OperationModel operationModel)
        {
            template.CSharpFile.AfterBuild(_ => ApplyStrategy(operationModel));
        }

        public void ApplyStrategy(OperationModel operationModel)
        {
            _template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            _template.AddTypeSource(TemplateRoles.Domain.ValueObject);
            _template.AddUsing("System.Linq");

            var dtoModel = operationModel.TypeReference.Element.AsDTOModel();
            var unqualifiedDtoTypeName = _template.TryGetTemplate<IClassProvider>(DtoModelTemplate.TemplateId, dtoModel, out var dtoTemplate)
                ? dtoTemplate.ClassName
                : dtoModel.Name.ToPascalCase();
            var domainModel = dtoModel.Mapping.Element.AsClassModel();
            var repositoryTypeName = _template.GetTypeName(TemplateRoles.Repository.Interface.Entity, domainModel);
            var repositoryParameterName = repositoryTypeName.Split('.').Last()[1..].ToLocalVariableName();
            var repositoryFieldName = repositoryParameterName.ToPrivateMemberName();

            var codeLines = new CSharpStatementAggregator();
            codeLines.Add(
                $@"var elements ={(operationModel.IsAsync() ? " await" : string.Empty)} {repositoryFieldName}.FindAll{(operationModel.IsAsync() ? "Async" : "")}(cancellationToken);");
            codeLines.Add($@"return elements.MapTo{unqualifiedDtoTypeName}List(_mapper);");

            var @class = _template.CSharpFile.Classes.First();
            var method = @class.FindMethod(m => m.TryGetMetadata<OperationModel>("model", out var model) && model.Id == operationModel.Id);
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