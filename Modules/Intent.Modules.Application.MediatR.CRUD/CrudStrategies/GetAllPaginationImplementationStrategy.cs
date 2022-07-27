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
    private readonly Lazy<StrategyData> _matchingElementDetails;

    public GetAllPaginationImplementationStrategy(QueryHandlerTemplate template, IApplication application,
        IMetadataManager metadataManager)
    {
        _template = template;
        _application = application;
        _metadataManager = metadataManager;
        _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
    }

    public bool IsMatch()
    {
        return _template.Model.Properties.Any(IsPageNumberParam)
               && _template.Model.Properties.Any(IsPageSizeParam)
               && _matchingElementDetails.Value.IsMatch;
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
                pageSize: request.{pageSizeVar.Name.ToPascalCase()},
                cancellationToken: cancellationToken);
            return results.MapToPagedResult(x => x.MapTo{_template.GetDtoName(_matchingElementDetails.Value.DtoModel)}(_mapper));";
    }

    private StrategyData GetMatchingElementDetails()
    {
        if (_template.Model.TypeReference.Element.Name != "PagedResult")
        {
            return NoMatch;
        }

        var nestedDtoModel = _template.Model.TypeReference.GenericTypeParameters.FirstOrDefault()?.Element.AsDTOModel();
        if (nestedDtoModel == null)
        {
            return NoMatch;
        }

        var mappedDomainEntity = nestedDtoModel.IsMapped ? nestedDtoModel.Mapping.Element.AsClassModel() : null;
        if (mappedDomainEntity == null)
        {
            return NoMatch;
        }

        var repositoryInterface = _template.GetEntityRepositoryInterfaceName(mappedDomainEntity);
        if (repositoryInterface == null)
        {
            return NoMatch;
        }

        var repository = new RequiredService(type: repositoryInterface,
            name: repositoryInterface.Substring(1).ToCamelCase());
        return new StrategyData(true, nestedDtoModel, repository);
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

    private static readonly StrategyData NoMatch = new StrategyData(false, null, null);

    private class StrategyData
    {
        public StrategyData(bool isMatch, DTOModel dtoModel, RequiredService repository)
        {
            IsMatch = isMatch;
            DtoModel = dtoModel;
            Repository = repository;
        }

        public bool IsMatch { get; }
        public DTOModel DtoModel { get; }
        public RequiredService Repository { get; }
    }
}