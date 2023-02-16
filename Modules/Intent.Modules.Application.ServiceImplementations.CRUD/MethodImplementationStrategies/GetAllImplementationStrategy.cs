using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies
{
    public class GetAllImplementationStrategy : IImplementationStrategy
    {
        private readonly ServiceImplementationTemplate _template;
        private readonly IApplication _application;

        public GetAllImplementationStrategy(ServiceImplementationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public bool IsMatch(OperationModel operationModel)
        {
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
            
            var lowerOperationName = operationModel.Name.ToLower();
            return new[] { "get", "find" }.Any(x => lowerOperationName.Contains(x));
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
            var domainType = _template.TryGetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, domainModel, out var result)
                ? result
                : domainModel.Name;
            var domainTypePascalCased = domainType.ToPascalCase();
            var repositoryTypeName = _template.GetTypeName(_template.GetRepositoryInterfaceName(), domainModel);
            var repositoryFieldName = $"{repositoryTypeName.ToCamelCase()}Repository";
            
            var codeLines = new CSharpStatementAggregator();
            codeLines.Add(
                $@"var elements ={(operationModel.IsAsync() ? " await" : string.Empty)} {repositoryFieldName.ToPrivateMemberName()}.FindAll{(operationModel.IsAsync() ? "Async" : "")}();");
            codeLines.Add($@"return elements.MapTo{dtoType}List(_mapper);");
            
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
            if (ctor.Parameters.All(p => p.Name != repositoryFieldName))
            {
                ctor.AddParameter(repositoryTypeName, repositoryFieldName, parm => parm.IntroduceReadonlyField());
            }
            if (ctor.Parameters.All(p => p.Name != "mapper"))
            {
                ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", parm => parm.IntroduceReadonlyField());
            }
        }
    }
}