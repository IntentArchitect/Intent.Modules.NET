using System;
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

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;

public class DeleteWithReturnDtoImplementationStrategy : IImplementationStrategy
{
    private readonly ServiceImplementationTemplate _template;
    private readonly IApplication _application;

    public DeleteWithReturnDtoImplementationStrategy(ServiceImplementationTemplate template, IApplication application)
    {
        _template = template;
        _application = application;
    }
    
    public bool IsMatch(OperationModel operationModel)
    {
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

        var lowerOperationName = operationModel.Name.ToLower();
        return new[] { "delete" }.Any(x => lowerOperationName.Contains(x));
    }

    public void ApplyStrategy(OperationModel operationModel)
    {
        _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
        _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
        _template.AddUsing("System.Linq");

        var (dtoModel, domainModel) = operationModel.GetDeleteModelPair(); 
        var dtoType = _template.TryGetTypeName(DtoModelTemplate.TemplateId, dtoModel, out var dtoName)
            ? dtoName
            : dtoModel.Name.ToPascalCase();
        var domainType = _template.TryGetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, domainModel, out var result)
            ? result
            : domainModel.Name;
        var domainTypePascalCased = domainType.ToPascalCase();
        var domainTypeCamelCased = domainType.ToCamelCase();
        var repositoryTypeName = _template.GetEntityRepositoryInterfaceName(domainModel);
        var repositoryFieldName = $"{domainTypeCamelCased}Repository";

        var codeLines = new CSharpStatementAggregator();
        codeLines.Add(
            $@"var existing{domainModel.Name} ={(operationModel.IsAsync() ? " await" : "")} {repositoryFieldName.ToPrivateMemberName()}.FindById{(operationModel.IsAsync() ? "Async" : "")}({operationModel.Parameters.Single().Name.ToCamelCase()}, cancellationToken);");
        codeLines.Add(new CSharpIfStatement($"existing{domainModel.Name} is null")
            .AddStatement($@"throw new {_template.GetNotFoundExceptionName()}($""Could not find {domainModel.Name.ToPascalCase()} {{{operationModel.Parameters.Single().Name.ToCamelCase()}}}"");"));
        codeLines.Add($"{repositoryFieldName.ToPrivateMemberName()}.Remove(existing{domainModel.Name});");
        codeLines.Add($@"return existing{domainModel.Name}.MapTo{dtoType}(_mapper);");

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