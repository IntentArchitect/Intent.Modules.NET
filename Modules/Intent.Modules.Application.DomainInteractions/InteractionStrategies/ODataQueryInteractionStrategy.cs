using System;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.DomainInteractions.InteractionStrategies;

public class ODataQueryInteractionStrategy : IInteractionStrategy
{
    private const string ODataQueryStereoType = "ODataQuery"; // Note that IDs should work too.
    public bool IsMatch(IElement interaction)
    {
        return interaction.IsQueryEntityActionTargetEndModel() && interaction.AsQueryEntityActionTargetEndModel().OtherEnd().Element.HasStereotype(ODataQueryStereoType);
    }

    public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
    {
        ArgumentNullException.ThrowIfNull(method);
        var interaction = (IAssociationEnd)interactionElement;

        try
        {
            var @class = method.Class;
            var foundEntity = interaction.TypeReference.Element.AsClassModel();
            var nestedCompOwner = GetNestedCompositionalOwner(foundEntity);
            var template = method.File.Template;
            if (!template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, nestedCompOwner != null ? nestedCompOwner : foundEntity, out var repositoryInterface))
            {
                return;
            }

            var repositoryName = @class.InjectService(repositoryInterface);

            var handleMethod = @class.FindMethod("Handle")!;
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();

            var allowSelect = interaction.AsQueryEntityActionTargetEndModel().OtherEnd().Element.GetStereotype(ODataQueryStereoType)
                .GetProperty<bool>("Allow Select");

            handleMethod.AddStatement(
                phase: ExecutionPhases.Response,
                statement: allowSelect
                    ? $"return await {repositoryName}.FindAllProjectToWithTransformationAsync(filterExpression: null, transform: request.Transform, cancellationToken: cancellationToken);"
                    : $"return await {repositoryName}.FindAllProjectToAsync(filterExpression: null, filterProjection: request.Transform, cancellationToken: cancellationToken);");
        }
        catch (ElementException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ElementException(interaction, "An error occurred while generating the domain interactions logic", ex);
        }
    }

    public static ClassModel? GetNestedCompositionalOwner(ClassModel entity)
    {
        var aggregateRootClass = entity.AssociatedClasses
            .Where(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
                        p.IsSourceEnd() &&
                        p is { IsCollection: false, IsNullable: false })
            .Select(s => s.Class)
            .Distinct()
            .ToList();

        return aggregateRootClass.Count <= 1
            ? aggregateRootClass.SingleOrDefault()
            : throw new ElementException(entity.InternalElement, $"{entity.Name} has multiple owners ({string.Join(",", aggregateRootClass.Select(a => a.Name))}). Owned entities can only have 1 owner.");
    }
}