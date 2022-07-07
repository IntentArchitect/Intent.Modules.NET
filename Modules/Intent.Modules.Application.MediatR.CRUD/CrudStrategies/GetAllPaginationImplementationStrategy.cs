using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies;

public class GetAllPaginationImplementationStrategy : ICrudImplementationStrategy
{
    private readonly QueryHandlerTemplate _template;
    private readonly IApplication _application;
    private readonly IMetadataManager _metadataManager;
    private readonly Lazy<(bool IsComplete, RequiredService Repository)> _matchingElementDetails;

    public GetAllPaginationImplementationStrategy(QueryHandlerTemplate template, IApplication application,
        IMetadataManager metadataManager)
    {
        _template = template;
        _application = application;
        _metadataManager = metadataManager;
        _matchingElementDetails = new Lazy<(bool IsComplete, RequiredService Repository)>(GetMatchingElementDetails);
    }

    public bool IsMatch()
    {
        return _template.Model.Properties.Any(IsPageNumberParam)
               && _template.Model.Properties.Any(IsPageSizeParam)
               && _matchingElementDetails.Value.IsComplete;
    }

    public IEnumerable<RequiredService> GetRequiredServices()
    {
        return new[]
        {
            _matchingElementDetails.Value.Repository,
            new RequiredService(_template.UseType("AutoMapper.IMapper"), "mapper"),
        };
    }

    public string GetImplementation()
    {
        var pageNumberVar = _template.Model.Properties.Single(IsPageNumberParam);
        var pageSizeVar = _template.Model.Properties.Single(IsPageSizeParam);
        return
            $@"var results = await {_matchingElementDetails.Value.Repository.FieldName}.FindAllAsync(
                pageNo: request.{pageNumberVar.Name.ToPascalCase()},
                pageSize: request.{pageSizeVar.Name.ToPascalCase()});
            return results.MapToPagedResult(_mapper);";
    }

    private (bool IsComplete, RequiredService Repository) GetMatchingElementDetails()
    {
        if (_template.Model.TypeReference.Element.Name != "PagedResult")
        {
            return (IsComplete: false, Repository: null);
        }

        var nestedDtoModel = _template.Model.TypeReference.GenericTypeParameters.FirstOrDefault()?.Element.AsDTOModel();
        if (nestedDtoModel == null)
        {
            return (IsComplete: false, Repository: null);
        }

        var mappedDomainEntity = nestedDtoModel.IsMapped ? nestedDtoModel.Mapping.Element.AsClassModel() : null;
        if (mappedDomainEntity == null)
        {
            return (IsComplete: false, Repository: null);
        }

        var repositoryInterface = _template.GetTypeName(EntityRepositoryInterfaceTemplate.TemplateId, mappedDomainEntity,
            new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
        if (repositoryInterface == null)
        {
            return (IsComplete: false, Repository: null);
        }

        var repository = new RequiredService(type: repositoryInterface,
            name: repositoryInterface.Substring(1).ToCamelCase());
        return (IsComplete: true, Repository: repository);
    }

    private bool IsPageNumberParam(DTOFieldModel param)
    {
        if (param.TypeReference.Element.Name != "int")
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

    private bool IsPageSizeParam(DTOFieldModel param)
    {
        if (param.TypeReference.Element.Name != "int")
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