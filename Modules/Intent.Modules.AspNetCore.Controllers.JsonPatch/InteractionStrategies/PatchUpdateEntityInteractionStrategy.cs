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
using Intent.Modules.Constants;
using Intent.Modules.Metadata.WebApi.Models;


namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.InteractionStrategies
{
    /// <summary>
    /// Overrides the default update entity interaction strategy for HTTP PATCH commands.
    /// </summary>
    public class PatchUpdateEntityInteractionStrategy : IInteractionStrategy
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

                EnsurePatchHelperMethods(method, updateAction, foundEntity);

                method.AddStatements(ExecutionPhases.BusinessLogic,
                [
                    new CSharpStatement($"LoadOriginalState({entityDetails.VariableName}, request)")
                        .WithSemicolon(),
                    new CSharpStatement("request.PatchExecutor.ApplyTo(request)")
                        .WithSemicolon(),
                    new CSharpStatement($"ApplyChangesTo(request, {entityDetails.VariableName})")
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

        private static void EnsurePatchHelperMethods(
            ICSharpClassMethodDeclaration handleMethod,
            UpdateEntityActionTargetEndModel updateAction,
            ClassModel foundEntity)
        {
            var requestParameter = handleMethod.Parameters.FirstOrDefault(p => p.Name == "request") ??
                                   handleMethod.Parameters.First();

            var entityTypeName = handleMethod.File.Template.GetTypeName(TemplateRoles.Domain.Entity.Primary, foundEntity)!;
            var commandTypeName = requestParameter.Type;
            var updateMapping = updateAction.Mappings.GetUpdateEntityMapping();
            if (updateMapping == null)
            {
                throw new ElementException(updateAction.InternalAssociationEnd, "No Update Entity mapping was found for PATCH update interaction.");
            }

            var @class = handleMethod.Class;

            @class.AddMethod(commandTypeName, "LoadOriginalState", method =>
            {
                method.Static();
                method.Private();
                method.AddParameter(entityTypeName, "entity");
                method.AddParameter(commandTypeName, "command");
                    
                method.AddStatement("ArgumentNullException.ThrowIfNull(entity);");
                method.AddStatement("ArgumentNullException.ThrowIfNull(command);");

                var generator = new JsonPatchLoadOriginalStateGenerator(handleMethod, updateMapping);
                foreach (var statement in generator.Generate())
                {
                    if (statement is CSharpAssignmentStatement)
                    {
                        statement.WithSemicolon();
                    }
                    method.AddStatement(statement);
                }

                method.AddStatement("return command;");
            });

            @class.AddMethod(entityTypeName, "ApplyChangesTo", method =>
            {
                method.Static();
                method.Private();
                method.AddParameter(commandTypeName, "command");
                method.AddParameter(entityTypeName, "entity");

                method.AddStatement("ArgumentNullException.ThrowIfNull(command);");
                method.AddStatement("ArgumentNullException.ThrowIfNull(entity);");

                var mappingManager = handleMethod.GetMappingManager();

                // Ensure mapping statements target the helper method parameters.
                mappingManager.SetFromReplacement((IElement)updateAction.OtherEnd().Element, "command");
                mappingManager.SetToReplacement(foundEntity, "entity");
                mappingManager.SetToReplacement(updateAction, "entity");

                var updateStatements = mappingManager.GenerateUpdateStatements(updateMapping);

                foreach (var statement in updateStatements)
                {
                    if (statement is CSharpAssignmentStatement)
                    {
                        statement.WithSemicolon();
                    }
                    method.AddStatement(statement);
                }

                method.AddStatement("return entity;");
            });
        }
    }
}