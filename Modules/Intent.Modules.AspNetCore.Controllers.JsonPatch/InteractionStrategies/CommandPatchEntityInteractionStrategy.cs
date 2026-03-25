using System;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.DataAccessProviders;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.UnitOfWork.Settings;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;


namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.InteractionStrategies;

/// <summary>
/// Overrides the default update entity interaction strategy for HTTP PATCH commands.
/// </summary>
internal class CommandPatchEntityInteractionStrategy : IInteractionStrategy
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
        if (action.OtherEnd().Element is not IElement { SpecializationType: "Command" } commandElement)
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
                          interaction.TypeReference.Element.AsOperationModel()?.ParentClass ??
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

            PatchBuilderHelper.EnsurePatchHelperMethods(method, updateAction, foundEntity, handleMethod =>
            {
                var param = handleMethod.Parameters.First(p => p.Name == "request");
                return new PayloadInfo("command", param.Type);
            });
            
            if (RequiresAggregateExplicitUpdate(entityDetails))
            {
                method.AddStatement(entityDetails.DataAccessProvider.Update(entityDetails.VariableName)
                    .SeparatedFromPrevious());
            }

            method.AddStatements(ExecutionPhases.BusinessLogic,
            [
                new CSharpStatement($"LoadOriginalState({entityDetails.VariableName}, request)")
                    .WithSemicolon(),
                new CSharpStatement("request.PatchExecutor.ApplyTo(request)")
                    .WithSemicolon(),
                new CSharpStatement($"ApplyChangesTo(request, {entityDetails.VariableName})")
                    .WithSemicolon()
            ]);
            
            if (RepositoryRequiresExplicitUpdate(method.File.Template, foundEntity))
            {
                method.AddStatement(entityDetails.DataAccessProvider.Update(entityDetails.VariableName.Singularize())
                    .SeparatedFromPrevious());
            }
            
            var automaticallyPersistUnitOfWork = method.Class.File.Template.ExecutionContext.GetSettings()
                .GetUnitOfWorkSettings()?.AutomaticallyPersistUnitOfWork() ?? true;
            var saveAlreadyCalled = method.FindStatement(x => x.ToString().Trim().Contains(entityDetails.DataAccessProvider.SaveChangesAsync().ToString().Trim())) != null;
            if (!saveAlreadyCalled && !automaticallyPersistUnitOfWork)
            {
                method.AddStatement(ExecutionPhases.Persistence, new CSharpStatement($"{entityDetails.DataAccessProvider.SaveChangesAsync()}"));
            }
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
    
    private static bool RequiresAggregateExplicitUpdate(EntityDetails entityDetails)
    {
        if (entityDetails.DataAccessProvider is CompositeDataAccessProvider cda)
        {
            return cda.RequiresExplicitUpdate();
        }
        return false;
    }
    
    private static bool RepositoryRequiresExplicitUpdate(ICSharpTemplate template, IMetadataModel forEntity)
    {
        return template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                   TemplateRoles.Repository.Interface.Entity,
                   forEntity,
                   out var repositoryInterfaceTemplate) &&
               repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
               requiresUpdate;
    }
}