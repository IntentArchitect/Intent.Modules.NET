using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies;

public class GetAllPaginationImplementationStrategy : ICrudImplementationStrategy
{
    private readonly QueryHandlerTemplate _template;
    private readonly IApplication _application;
    private readonly IMetadataManager _metadataManager;

    public GetAllPaginationImplementationStrategy(QueryHandlerTemplate template, IApplication application, IMetadataManager metadataManager)
    {
        _template = template;
        _application = application;
        _metadataManager = metadataManager;
    }
    
    public bool IsMatch()
    {
        return _template.Model.Properties.Any(IsPageNumberParam)
            && _template.Model.Properties.Any(IsPageSizeParam)
            && _template.Model.TypeReference.Element.Name == "PagedResult";
    }

    public IEnumerable<RequiredService> GetRequiredServices()
    {
        return new[]
        {
            _repository,
            new RequiredService(_template.UseType("AutoMapper.IMapper"), "mapper"), 
        };
    }

    public string GetImplementation()
    {
        var pageNumberVar = _template.Model.Properties.Single(IsPageNumberParam);
        var pageSizeVar = _template.Model.Properties.Single(IsPageSizeParam);
        return
            $@"var results = await {_repository.FieldName}Repository.FindAllAsync(
                pageNo: request.{pageNumberVar.Name.ToPascalCase()},
                pageSize: request.{pageSizeVar.Name.ToPascalCase()});
            return results.MapToPagedResult(_mapper);";
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