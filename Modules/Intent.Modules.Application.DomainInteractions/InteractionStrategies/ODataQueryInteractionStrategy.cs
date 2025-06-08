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
    //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();
    public bool IsMatch(IElement interaction)
    {
        return interaction.IsQueryEntityActionTargetEndModel() && interaction.AsQueryEntityActionTargetEndModel().OtherEnd().Element.HasStereotype(ODataQueryStereoType);
    }

    public void ImplementInteraction(CSharpClassMethod method, IElement interactionElement)
    {
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }

        var interaction = (IAssociationEnd)interactionElement;
        try
        {
            var @class = method.Class;
            var foundEntity = interaction.TypeReference.Element.AsClassModel();
            var nestedCompOwner = GetNestedCompositionalOwner(foundEntity);
            var _template = method.File.Template;
            if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, nestedCompOwner != null ? nestedCompOwner : foundEntity, out var repositoryInterface))
            {
                return;
            }

            var repositoryName =  @class.InjectService(repositoryInterface);

            var handleMethod = @class.FindMethod("Handle");
            handleMethod.Attributes.OfType<CSharpIntentManagedAttribute>().SingleOrDefault()?.WithBodyFully();
            var allowSelect = interaction.AsQueryEntityActionTargetEndModel().OtherEnd().Element.GetStereotype(ODataQueryStereoType)
                .GetProperty<bool>("Allow Select");
            if (allowSelect)
            {
                handleMethod.AddStatement(ExecutionPhases.Response, $"return await {repositoryName}.FindAllProjectToWithTransformationAsync(filterExpression: null, transform: request.Transform, cancellationToken: cancellationToken);");
            }
            else
            {
                handleMethod.AddStatement(ExecutionPhases.Response, $"return await {repositoryName}.FindAllProjectToAsync(filterExpression: null, filterProjection: request.Transform, cancellationToken: cancellationToken);");
            }
        }
        catch (ElementException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ElementException(interaction, "An error occurred while generating the domain interactions logic", ex);
        }
    }

    public static ClassModel GetNestedCompositionalOwner(ClassModel entity)
    {
        var aggregateRootClass = entity.AssociatedClasses
            .Where(p => p.TypeReference?.Element?.AsClassModel()?.IsAggregateRoot() == true &&
                        p.IsSourceEnd() && !p.IsCollection && !p.IsNullable)
            .Select(s => s.Class)
            .Distinct()
            .ToList();
        if (aggregateRootClass.Count > 1)
        {
            throw new ElementException(entity.InternalElement, $"{entity.Name} has multiple owners ({string.Join(",", aggregateRootClass.Select(a => a.Name))}). Owned entities can only have 1 owner.");
        }
        return aggregateRootClass.SingleOrDefault();
    }
}