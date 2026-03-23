using System;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.DataAccessProviders;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.InteractionStrategies;

/// <summary>
/// Overrides the default update entity interaction strategy for HTTP PATCH service operations.
/// </summary>
internal class TradServicePatchEntityInteractionStrategy : IInteractionStrategy
{
    public bool IsMatch(IElement interaction)
    {
        if (!interaction.IsUpdateEntityActionTargetEndModel())
        {
            return false;
        }

        var action = interaction.AsUpdateEntityActionTargetEndModel();
        if (action?.TypeReference?.Element == null || !action.Mappings.Any())
        {
            return false;
        }

        // Determine if the source end of the interaction is a Command and whether it is an HTTP PATCH endpoint.
        if (action.OtherEnd().Element is not IElement { SpecializationType: "Operation" } commandElement)
        {
            return false;
        }

        return HttpEndpointModelFactory.TryGetEndpoint(commandElement, null, false, out var endpoint)
               && endpoint.Verb == HttpVerb.Patch;
    }

    public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
    {
        ArgumentNullException.ThrowIfNull(method);
        var interaction = (IAssociationEnd)interactionElement;

        var updateAction = interactionElement.AsUpdateEntityActionTargetEndModel();
        if (updateAction == null)
        {
            throw new ElementException(interactionElement, "Expected Update Entity Action interaction.");
        }

        var foundEntity = interaction.TypeReference.Element.AsClassModel() ??
                          Intent.Modelers.Domain.Api.OperationModelExtensions.AsOperationModel(interaction.TypeReference.Element)?.ParentClass ??
                          throw new ElementException(interactionElement,
                              $"Unable to determine entity for update interaction targeting '{interaction.TypeReference.Element.Name}'.");

        try
        {
            var queryContext = new QueryActionContext(method, ActionType.Update, interaction);
            var dataAccess = method.InjectDataAccessProvider(foundEntity, queryContext);
            var projectedType = queryContext.ImplementWithProjections() && dataAccess.IsUsingProjections
                ? queryContext.GetDtoProjectionReturnType()
                : null;

            method.AddStatements(ExecutionPhases.BusinessLogic, method.GetQueryStatements(
                dataAccessProviderInjector: DataAccessProviderInjector.Instance,
                dataAccessProvider: dataAccess,
                interaction: interaction,
                foundEntity: foundEntity,
                projectedType: projectedType,
                mustAccessEntityThroughAggregate: dataAccess.MustAccessEntityThroughAggregate(),
                compositeEntityAccessor: (dataAccess as CompositeDataAccessProvider)?.Accessor,
                aggregateDetails: out _));

            method.AddStatement(ExecutionPhases.BusinessLogic, string.Empty);

            // Use tracked entity details to determine the variable name.
            var entityDetails = method.TrackedEntities()[updateAction.Id];

            var param = method.Parameters.FirstOrDefault(param =>
                param.TryGetMetadata<Intent.Modelers.Services.Api.ParameterModel>("model", out var paramModel)
                && paramModel.TypeReference.Element.IsDTOModel())!;
            
            PatchBuilderHelper.EnsurePatchHelperMethods(method, updateAction, foundEntity, handleMethod => param);

            method.AddStatements(ExecutionPhases.BusinessLogic,
            [
                new CSharpStatement($"LoadOriginalState({entityDetails.VariableName}, {param.Name})")
                    .WithSemicolon(),
                new CSharpStatement($"{param.Name}.PatchExecutor.ApplyTo({param.Name})")
                    .WithSemicolon(),
                new CSharpStatement($"ApplyChangesTo({param.Name}, {entityDetails.VariableName})")
                    .WithSemicolon()
            ]);
        }
        catch (ElementException)
        {
            throw;
        }
        catch (FriendlyException ex)
        {
            throw new ElementException(updateAction.InternalAssociationEnd, ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ElementException(updateAction.InternalAssociationEnd, "An error occurred while generating the JsonPatch update logic", ex);
        }
    }
}