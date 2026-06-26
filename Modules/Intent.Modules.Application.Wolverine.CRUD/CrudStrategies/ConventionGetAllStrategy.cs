using System.Linq;
using Intent.Exceptions;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Application.Wolverine.Templates.QueryHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.Wolverine.CRUD.CrudStrategies;

/// <summary>
/// Convention-based implementation of an unfiltered "get all" query handler.
/// Mirrors the MediatR CRUD module's <c>GetAllImplementationStrategy</c>: when a query has no
/// modelled domain interactions but returns a collection of a DTO that is mapped from a domain
/// entity, the handler body is generated as a repository <c>FindAllAsync</c> followed by an
/// AutoMapper projection. This complements the domain-interactions path (which requires a
/// Query Entity Action association) so that conventionally modelled queries are not left as stubs.
/// </summary>
internal static class ConventionGetAllStrategy
{
    public static void TryApply(QueryHandlerTemplate template)
    {
        var model = template.Model;

        // The convention only applies when nothing else drives the handler body.
        if (model.HasDomainInteractions())
        {
            return;
        }

        // Must return a collection of a DTO that maps from a domain entity.
        if (model.TypeReference?.Element == null || !model.TypeReference.IsCollection)
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
