using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.Decorators;
using Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;
using ParameterModel = Intent.Modelers.Services.Api.ParameterModel;

namespace Intent.Modules.Application.ServiceImplementations.Conventions.CRUD.MethodImplementationStrategies;

public class GetAllPaginationImplementationStrategy : IImplementationStrategy
{
    private readonly CrudConventionDecorator _decorator;

    public GetAllPaginationImplementationStrategy(CrudConventionDecorator decorator)
    {
        _decorator = decorator;
    }

    public bool Match(ClassModel domainModel, OperationModel operationModel)
    {
        if (!operationModel.Parameters.Any(IsPageNumberParam)
             || !operationModel.Parameters.Any(IsPageSizeParam)
             || operationModel.ReturnType.Element.Name != "PagedResult"
             || !operationModel.ReturnType.GenericTypeParameters.Any())
        {
            return false;
        }
        
        return _decorator.HasEntityRepositoryInterfaceName(domainModel) 
               && _decorator.HasDtoName(operationModel.TypeReference.GenericTypeParameters.First().Element);
    }

    private bool IsPageNumberParam(ParameterModel param)
    {
        if (param.Type.Element.Name != "int")
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

    private bool IsPageSizeParam(ParameterModel param)
    {
        if (param.Type.Element.Name != "int")
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

    public string GetImplementation(ClassModel domainModel, OperationModel operationModel)
    {
        var pageNumberVar = operationModel.Parameters.Single(IsPageNumberParam);
        var pageSizeVar = operationModel.Parameters.Single(IsPageSizeParam);
        var genericDtoType = operationModel.TypeReference.GenericTypeParameters.First().Element;
        return
            $@"var results = {(operationModel.IsAsync() ? " await" : string.Empty)} {domainModel.Name.ToPrivateMemberName()}Repository.FindAll{(operationModel.IsAsync() ? "Async" : "")}(
                pageNo: {pageNumberVar.Name.ToParameterName()},
                pageSize: {pageSizeVar.Name.ToParameterName()});
            return results.MapToPagedResult(x => x.MapTo{_decorator.GetDtoName(genericDtoType)}(_mapper));";
    }

    public IEnumerable<ConstructorParameter> GetRequiredServices(ClassModel domainModel)
    {
        var repo = _decorator.GetEntityRepositoryInterfaceName(domainModel);
        return new[]
        {
            new ConstructorParameter(repo, repo.Substring(1).ToCamelCase()),
            new ConstructorParameter(_decorator.Template.UseType("AutoMapper.IMapper"), "mapper"),
        };
    }
}