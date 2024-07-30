using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.CRUD.Decorators;
using Intent.Modules.Application.MediatR.Templates;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.MediatR.CRUD.CrudStrategies;

public class GetAllPaginationImplementationStrategy : ICrudImplementationStrategy
{
    private readonly CSharpTemplateBase<QueryModel> _template;
    private readonly Lazy<StrategyData> _matchingElementDetails;

    public GetAllPaginationImplementationStrategy(CSharpTemplateBase<QueryModel> template)
    {
        _template = template;
        _matchingElementDetails = new Lazy<StrategyData>(GetMatchingElementDetails);
    }

    public bool IsMatch()
    {
        return (_template.Model.Properties.Any(IsPageNumberParam) || _template.Model.Properties.Any(IsPageIndexParam))
               && _template.Model.Properties.Any(IsPageSizeParam)
               && _matchingElementDetails.Value.IsMatch;
    }

    public void BindToTemplate(ICSharpFileBuilderTemplate template)
    {
        template.CSharpFile.AfterBuild(_ => ApplyStrategy());
    }

    public void ApplyStrategy()
    {
        var @class = ((ICSharpFileBuilderTemplate)_template).CSharpFile.Classes.First(x => x.HasMetadata("handler"));
        var ctor = @class.Constructors.First();
        var repository = _matchingElementDetails.Value.Repository;
        ctor.AddParameter(repository.Type, repository.Name.ToParameterName(), param => param.IntroduceReadonlyField())
            .AddParameter(_template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());

        var handleMethod = @class.FindMethod("Handle");
        handleMethod.Statements.Clear();
        handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
        handleMethod.AddStatements(GetImplementation());
    }

    public IEnumerable<CSharpStatement> GetImplementation()
    {
        var pageNumberVar = _template.Model.Properties.SingleOrDefault(IsPageNumberParam);
        var pageIndexVar = pageNumberVar == null ? _template.Model.Properties.Single(IsPageIndexParam) : null;
        var pageSizeVar = _template.Model.Properties.Single(IsPageSizeParam);
        yield return
            $@"var results = await {_matchingElementDetails.Value.Repository.FieldName}.FindAllAsync(
                pageNo: request.{(pageNumberVar?.Name.ToPascalCase() ?? $"{pageIndexVar!.Name.ToPascalCase()}+1")},
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

        if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, mappedDomainEntity, out var repositoryInterface))
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
            default:
                break;
        }

        return false;
    }

    private bool IsPageIndexParam(DTOFieldModel param)
    {
        if (param.TypeReference.Element.Name != "int")
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
            default:
                break;
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