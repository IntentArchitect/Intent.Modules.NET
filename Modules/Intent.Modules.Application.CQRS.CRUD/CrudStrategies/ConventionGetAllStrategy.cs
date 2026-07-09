using System.Linq;
using Intent.Exceptions;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.CQRS.CRUD.CrudStrategies;

/// <summary>
/// Convention-based implementation of an unfiltered "get all" query handler.
/// Mirrors the legacy MediatR CRUD module's <c>GetAllImplementationStrategy</c>: when a query has no
/// modelled domain interactions but returns a collection of a DTO that is mapped from a domain
/// entity, the handler body is generated as a repository <c>FindAllAsync</c> followed by an
/// AutoMapper projection. This complements the domain-interactions path (which requires a
/// Query Entity Action association) so that conventionally modelled queries are not left as stubs.
///
/// Transport-agnostic: operates only against <see cref="ICSharpFileBuilderTemplate"/> and the
/// role-discovered <see cref="QueryModel"/> - it has no dependency on any concrete handler template type.
/// </summary>
internal static class ConventionGetAllStrategy
{
    public static void TryApply(ICSharpFileBuilderTemplate template, QueryModel model)
    {
        // The convention only applies when nothing else drives the handler body.
        if (model.HasDomainInteractions())
        {
            return;
        }

        if (model.TypeReference?.Element == null)
        {
            return;
        }

        // Paged "get all": returns a PagedResult<TDto> (not a plain collection).
        if (model.TypeReference.Element.Name == "PagedResult")
        {
            TryApplyPaged(template, model);
            return;
        }

        // Must return a collection of a DTO that maps from a domain entity.
        if (!model.TypeReference.IsCollection)
        {
            return;
        }

        var returnDto = model.TypeReference.Element.AsDTOModel();
        if (returnDto?.Mapping == null)
        {
            return;
        }

        var foundEntity = returnDto.Mapping.Element.AsClassModel();
        if (foundEntity == null)
        {
            return;
        }

        // Nested compositional entities must be reached through their aggregate root, which this
        // convention does not model - leave those to the domain-interactions path.
        if (foundEntity.GetNestedCompositionalOwner() != null)
        {
            return;
        }

        if (!template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, foundEntity, out var repositoryInterface))
        {
            return;
        }

        var repositoryName = repositoryInterface.Substring(1).ToCamelCase();
        var repositoryFieldName = $"_{repositoryName}";

        template.CSharpFile.AfterBuild(_ =>
        {
            template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);

            var @class = template.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);

            var ctor = @class.Constructors.First();
            ctor.AddParameter(repositoryInterface, repositoryName.ToParameterName(), param => param.IntroduceReadonlyField())
                .AddParameter(template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle")!;
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

            var entitiesVariable = foundEntity.Name.ToCamelCase().Pluralize();
            handleMethod.AddStatement($"var {entitiesVariable} = await {repositoryFieldName}.FindAllAsync(cancellationToken);");
            handleMethod.AddStatement($"return {entitiesVariable}.MapTo{template.GetDtoName(returnDto)}List(_mapper);");
        });
    }

    /// <summary>
    /// Convention-based implementation of a paged "get all" query handler. Mirrors the legacy MediatR CRUD
    /// module's <c>GetAllPaginationImplementationStrategy</c>: when a query has no modelled domain interactions
    /// but returns a <c>PagedResult&lt;TDto&gt;</c> whose nested DTO maps from a domain entity, and it exposes
    /// page-number + page-size fields, the handler is generated as a paged repository <c>FindAllAsync</c>
    /// followed by <c>MapToPagedResult</c>. Without this, paged queries were left as NotImplemented stubs
    /// (only plain collections were handled), breaking MediatR parity for the transport-agnostic CRUD module.
    /// </summary>
    private static void TryApplyPaged(ICSharpFileBuilderTemplate template, QueryModel model)
    {
        var nestedDto = model.TypeReference.GenericTypeParameters.FirstOrDefault()?.Element.AsDTOModel();
        if (nestedDto?.Mapping == null)
        {
            return;
        }

        var foundEntity = nestedDto.Mapping.Element.AsClassModel();
        if (foundEntity == null)
        {
            return;
        }

        // Nested compositional entities must be reached through their aggregate root - leave to the interactions path.
        if (foundEntity.GetNestedCompositionalOwner() != null)
        {
            return;
        }

        var pageNoProp = model.Properties.FirstOrDefault(IsPageNumberParam);
        var pageSizeProp = model.Properties.FirstOrDefault(IsPageSizeParam);
        if (pageNoProp == null || pageSizeProp == null)
        {
            return;
        }

        if (!template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, foundEntity, out var repositoryInterface))
        {
            return;
        }

        var repositoryName = repositoryInterface.Substring(1).ToCamelCase();
        var repositoryFieldName = $"_{repositoryName}";

        template.CSharpFile.AfterBuild(_ =>
        {
            template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);

            var @class = template.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);

            var ctor = @class.Constructors.First();
            ctor.AddParameter(repositoryInterface, repositoryName.ToParameterName(), param => param.IntroduceReadonlyField())
                .AddParameter(template.UseType("AutoMapper.IMapper"), "mapper", param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle")!;
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

            handleMethod.AddStatement(
                $"var results = await {repositoryFieldName}.FindAllAsync(request.{pageNoProp.Name.ToPascalCase()}, request.{pageSizeProp.Name.ToPascalCase()}, cancellationToken);");
            handleMethod.AddStatement(
                $"return results.MapToPagedResult(x => x.MapTo{template.GetDtoName(nestedDto)}(_mapper));");
        });
    }

    private static bool IsPageNumberParam(DTOFieldModel param)
    {
        if (param.TypeReference.Element?.Name != "int")
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

    private static bool IsPageSizeParam(DTOFieldModel param)
    {
        if (param.TypeReference.Element?.Name != "int")
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

    private static string GetDtoName(this ICSharpTemplate template, DTOModel dtoModel)
    {
        var dtoTemplate = template.GetTemplate<IClassProvider>("Application.Contract.Dto", dtoModel, TemplateDiscoveryOptions.DoNotThrow);
        if (dtoTemplate == null)
        {
            return dtoModel.Name;
        }

        template.AddUsing(dtoTemplate.Namespace);
        return dtoTemplate.ClassName;
    }

    private static ClassModel? GetNestedCompositionalOwner(this ClassModel entity)
    {
        var aggregateRootClasses = entity.AssociatedClasses
            .Where(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
                        p.IsSourceEnd() && !p.IsCollection && !p.IsNullable)
            .Select(s => s.Class)
            .Distinct()
            .ToList();

        if (aggregateRootClasses.Count > 1)
        {
            throw new ElementException(entity.InternalElement,
                $"{entity.Name} has multiple owners ({string.Join(",", aggregateRootClasses.Select(a => a.Name))}). " +
                "Owned entities can only have 1 owner.");
        }

        return aggregateRootClasses.SingleOrDefault();
    }
}
