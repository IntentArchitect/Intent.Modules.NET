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
/// Convention-based implementation of an OData "get all" query handler: no modelled Domain
/// Interaction, the <c>ODataQuery</c> stereotype applied directly to the query, returning a
/// collection of a DTO mapped from a domain entity. Mirrors the tech-agnostic, modelled-path
/// <c>ODataQueryInteractionStrategy</c> (Intent.Modules.Application.DomainInteractions) but fires
/// as a convention when there is no <c>Query Entity Action</c> association to interact against.
///
/// Transport-agnostic: operates only against <see cref="ICSharpFileBuilderTemplate"/> and the
/// role-discovered <see cref="QueryModel"/> - it has no dependency on any concrete handler template type.
/// </summary>
internal static class ConventionODataGetAllStrategy
{
    private const string ODataQueryStereotype = "ODataQuery";

    public static void TryApply(ICSharpFileBuilderTemplate template, QueryModel model)
    {
        // The convention only applies when nothing else drives the handler body.
        if (model.HasDomainInteractions())
        {
            return;
        }

        // Must return a collection and carry the ODataQuery stereotype directly on the query.
        if (model.TypeReference?.Element == null || !model.TypeReference.IsCollection ||
            !model.HasStereotype(ODataQueryStereotype))
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

        // Nested compositional entities must be reached through their aggregate root.
        var nestedCompOwner = foundEntity.GetNestedCompositionalOwner();
        var repositoryTarget = nestedCompOwner ?? foundEntity;

        if (!template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, repositoryTarget, out var repositoryInterface))
        {
            return;
        }

        var repositoryName = repositoryInterface.Substring(1).ToCamelCase();

        // Read the property here - eagerly, before AfterBuild - since it depends only on the model, not on
        // generated CSharpFile structure.
        var allowSelect = model.GetStereotype(ODataQueryStereotype).GetProperty<bool>("Enable Select");

        template.CSharpFile.AfterBuild(_ =>
        {
            template.AddTypeSource(TemplateRoles.Domain.Entity.Primary);

            var @class = template.CSharpFile.Classes.First(x => x.FindMethod("Handle") is not null);

            var ctor = @class.Constructors.First();
            ctor.AddParameter(repositoryInterface, repositoryName.ToParameterName(), param => param.IntroduceReadonlyField());

            var handleMethod = @class.FindMethod("Handle")!;
            handleMethod.Statements.Clear();
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

            handleMethod.AddStatement(allowSelect
                ? $"return await _{repositoryName}.FindAllProjectToWithTransformationAsync(filterExpression: null, transform: request.Transform, cancellationToken: cancellationToken);"
                : $"return await _{repositoryName}.FindAllProjectToAsync(filterExpression: null, filterProjection: request.Transform, cancellationToken: cancellationToken);");
        });
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
