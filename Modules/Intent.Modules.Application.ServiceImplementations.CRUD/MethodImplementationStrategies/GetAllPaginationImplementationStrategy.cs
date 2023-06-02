using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
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
    private readonly IApplication _application;

    public GetAllPaginationImplementationStrategy(ServiceImplementationTemplate template, IApplication application)
    {
        _template = template;
        _application = application;
    }

    public bool IsMatch(OperationModel operationModel)
    {
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
        var domainType = _template.TryGetTypeName(TemplateFulfillingRoles.Domain.Entity.Primary, domainModel, out var result)
            ? result
            : domainModel.Name;
        var domainTypePascalCased = domainType.ToPascalCase();
        var domainTypeCamelCased = domainType.ToCamelCase();
        var repositoryTypeName = _template.GetEntityRepositoryInterfaceName(domainModel);
        var repositoryFieldName = $"{domainTypeCamelCased}Repository";
        
        var codeLines = new CSharpStatementAggregator();
        var pageNumberVar = operationModel.Parameters.SingleOrDefault(IsPageNumberParam);
        var pageIndexVar = pageNumberVar == null ? operationModel.Parameters.Single(IsPageIndexParam) : null;
        var pageSizeVar = operationModel.Parameters.Single(IsPageSizeParam);
        codeLines.Add(new CSharpInvocationStatement($@"var results = {(operationModel.IsAsync() ? " await" : string.Empty)} {repositoryFieldName.ToPrivateMemberName()}.FindAll{(operationModel.IsAsync() ? "Async" : "")}")
            .AddArgument($"pageNo: {pageNumberVar?.Name.ToParameterName() ?? $"{pageIndexVar.Name.ToParameterName()} + 1"}")
            .AddArgument($"pageSize: {pageSizeVar.Name.ToParameterName()}")
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
        if (ctor.Parameters.All(p => p.Name != repositoryFieldName))
        {
            ctor.AddParameter(repositoryTypeName, repositoryFieldName, parm => parm.IntroduceReadonlyField());
        }
        if (ctor.Parameters.All(p => p.Name != "mapper"))
        {
            ctor.AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", parm => parm.IntroduceReadonlyField());
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
        }

        return false;
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
        }

        return false;
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
        }

        return false;
    }
}