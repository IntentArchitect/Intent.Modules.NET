using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;

public class GetAllPaginationImplementationStrategy : IImplementationStrategy
{
    private readonly ServiceImplementationTemplate _template;

    public GetAllPaginationImplementationStrategy(ServiceImplementationTemplate template, IApplication application)
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

        if ((!operationModel.Parameters.Any(IsPageNumberParam)
             && !operationModel.Parameters.Any(IsPageIndexParam))
             || !operationModel.Parameters.Any(IsPageSizeParam)
             || operationModel.ReturnType.Element.Name != "PagedResult"
             || !operationModel.ReturnType.GenericTypeParameters.Any())
        {
            return false;
        }

        var dtoModel = operationModel.ReturnType.GenericTypeParameters.First().Element.AsDTOModel();
        return dtoModel.Mapping?.Element?.AsClassModel() != null;
    }

    public void ApplyStrategy(OperationModel operationModel)
    {
        _template.AddTypeSource(TemplateFulfillingRoles.Domain.Entity.Primary);
        _template.AddTypeSource(TemplateFulfillingRoles.Domain.ValueObject);
        _template.AddUsing("System.Linq");

        var genericDtoElement = operationModel.TypeReference.GenericTypeParameters.First().Element;
        var dtoModel = genericDtoElement.AsDTOModel();
        var dtoType = _template.TryGetTypeName(DtoModelTemplate.TemplateId, dtoModel, out var dtoName)
            ? dtoName
            : dtoModel.Name.ToPascalCase();
        var domainModel = dtoModel.Mapping.Element.AsClassModel();
        var repositoryTypeName = _template.GetEntityRepositoryInterfaceName(domainModel);
        var repositoryParameterName = repositoryTypeName.Split('.').Last()[1..].ToLocalVariableName();
        var repositoryFieldName = repositoryParameterName.ToPrivateMemberName();

        var codeLines = new CSharpStatementAggregator();
        var pageNumberVar = operationModel.Parameters.SingleOrDefault(IsPageNumberParam);
        var pageIndexVar = pageNumberVar == null ? operationModel.Parameters.Single(IsPageIndexParam) : null;
        var pageSizeVar = operationModel.Parameters.Single(IsPageSizeParam);
        codeLines.Add(new CSharpInvocationStatement($@"var results = {(operationModel.IsAsync() ? " await" : string.Empty)} {repositoryFieldName}.FindAll{(operationModel.IsAsync() ? "Async" : "")}")
            .AddArgument($"pageNo: {pageNumberVar?.Name.ToParameterName() ?? $"{pageIndexVar!.Name.ToParameterName()} + 1"}")
            .AddArgument($"pageSize: {pageSizeVar.Name.ToParameterName()}")
            .AddStatement("cancellationToken: cancellationToken")
            .WithArgumentsOnNewLines());
        codeLines.Add($"return results.MapToPagedResult(x => x.MapTo{dtoType}(_mapper));");

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

    private bool IsPageNumberParam(ParameterModel param)
    {
        if (!param.Type.HasIntType())
        {
            return false;
        }

        switch (param.Name.ToLower())
        {
            case "page":
            case "pageno":
            case "pagenum":
            case "pagenumber":
                return true;
            default:
                return false;
        }
    }

    private bool IsPageIndexParam(ParameterModel param)
    {
        if (!param.Type.HasIntType())
        {
            return false;
        }

        switch (param.Name.ToLower())
        {
            case "pageindex":
                return true;
            default:
                return false;
        }
    }

    private bool IsPageSizeParam(ParameterModel param)
    {
        if (!param.Type.HasIntType())
        {
            return false;
        }

        switch (param.Name.ToLower())
        {
            case "size":
            case "pagesize":
                return true;
            default:
                return false;
        }
    }
}