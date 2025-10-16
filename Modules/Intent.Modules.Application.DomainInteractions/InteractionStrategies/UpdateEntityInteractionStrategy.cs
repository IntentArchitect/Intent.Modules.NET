using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.DataAccessProviders;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.UnitOfWork.Settings;
using Intent.Modules.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Application.DomainInteractions.InteractionStrategies
{
    public class UpdateEntityInteractionStrategy : IInteractionStrategy
    {
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsUpdateEntityActionTargetEndModel();
        }

        public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
        {
            ArgumentNullException.ThrowIfNull(method);
            var interaction = (IAssociationEnd)interactionElement;

            var queryContext = new QueryActionContext(method, ActionType.Update, interaction);
            var foundEntity = interaction.TypeReference.Element.AsClassModel() ??
                              interaction.TypeReference.Element.AsOperationModel().ParentClass;
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

            var updateAction = interaction.AsUpdateEntityActionTargetEndModel();
            var csharpMapping = method.GetMappingManager();
            var template = method.File.Template;
            try
            {
                var entityDetails = method.TrackedEntities()[updateAction.Id];
                var entity = entityDetails.ElementModel.AsClassModel();
                var updateMapping = updateAction.Mappings.GetUpdateEntityMapping();

                var statements = new List<CSharpStatement>();

                if (entityDetails.IsCollection)
                {
                    csharpMapping.SetToReplacement(entity, entityDetails.VariableName.Singularize());
                    if (updateMapping != null)
                    {
                        statements.Add(new CSharpForEachStatement(entityDetails.VariableName.Singularize(), entityDetails.VariableName)
                            .AddStatements(csharpMapping.GenerateUpdateStatements(updateMapping)));
                    }

                    if (RepositoryRequiresExplicitUpdate(template, entity))
                    {
                        statements.Add(entityDetails.DataAccessProvider.Update(entityDetails.VariableName.Singularize())
                            .SeparatedFromPrevious());
                    }
                }
                else
                {
                    if (updateMapping != null)
                    {
                        var updateStatements = csharpMapping.GenerateUpdateStatements(updateMapping);
                        method.Class.WireUpDomainServicesForOperations(updateAction, updateStatements);
                        AdjustOperationInvocationForAsyncAndReturn(method, updateMapping, updateStatements);

                        statements.AddRange(updateStatements);
                    }

                    if (RepositoryRequiresExplicitUpdate(template, entity))
                    {
                        statements.Add(entityDetails.DataAccessProvider.Update(entityDetails.VariableName)
                            .SeparatedFromPrevious());
                    }
                }

                if (RequiresAggregateExplicitUpdate(entityDetails))
                {
                    statements.Add(entityDetails.DataAccessProvider.Update(entityDetails.VariableName)
                        .SeparatedFromPrevious());
                }

                method.AddStatements(ExecutionPhases.BusinessLogic, statements);

                var automaticallyPersistUnitOfWork = method.Class.File.Template.ExecutionContext.GetSettings()
                    .GetUnitOfWorkSettings()?.AutomaticallyPersistUnitOfWork() ?? true;
                var saveAlreadyCalled = method.FindStatement(x => x.ToString().Trim().Contains(entityDetails.DataAccessProvider.SaveChangesAsync().ToString().Trim())) != null;
                if (!saveAlreadyCalled && !automaticallyPersistUnitOfWork)
                {
                    method.AddStatement(ExecutionPhases.Persistence, new CSharpStatement($"{entityDetails.DataAccessProvider.SaveChangesAsync()}"));
                }
            }
            catch (Exception ex)
            {
                throw new ElementException(updateAction.InternalAssociationEnd, "An error occurred while generating the domain interactions logic", ex);
            }
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

        private static bool RequiresAggregateExplicitUpdate(EntityDetails entityDetails)
        {
            if (entityDetails.DataAccessProvider is CompositeDataAccessProvider cda)
            {
                return cda.RequiresExplicitUpdate();
            }
            return false;
        }

        private static void AdjustOperationInvocationForAsyncAndReturn(ICSharpClassMethodDeclaration method, IElementToElementMapping updateMapping, IList<CSharpStatement> updateStatements)
        {
            if (!updateMapping.MappedEnds.Any(me => me.TargetElement.IsOperationModel()))
            {
                return;
            }

            foreach (var invocation in updateMapping.MappedEnds.Where(me => me.TargetElement.IsOperationModel()))
            {
                var operationName = ((IElement)invocation.TargetElement).Name;
                var variableName = $"{operationName.ToCamelCase()}Result";
                var hasReturn = invocation.TargetElement.TypeReference?.Element != null;

                for (var i = 0; i < updateStatements.Count; i++)
                {
                    if (updateStatements[i] is not CSharpInvocationStatement s ||
                        s.Expression.Reference is not ICSharpMethodDeclaration md ||
                        md.Name != operationName)
                    {
                        continue;
                    }

                    if (s.IsAsyncInvocation() == true)
                    {
                        if (method.Parameters.Any(x => x.Type == "CancellationToken"))
                        {
                            s.AddArgument(method.Parameters.Single(x => x.Type == "CancellationToken").Name);
                        }

                        updateStatements[i] = new CSharpAwaitExpression(updateStatements[i]);
                    }

                    if (hasReturn)
                    {
                        updateStatements[i] = new CSharpAssignmentStatement(new CSharpVariableDeclaration(variableName), updateStatements[i]);
                    }
                }

                method.TrackedEntities().Add(
                    key: invocation.TargetElement.Id,
                    value: new EntityDetails(
                        ElementModel: (IElement)invocation.TargetElement.TypeReference!.Element,
                        VariableName: variableName,
                        DataAccessProvider: null,
                        IsNew: false,
                        ProjectedType: null,
                        IsCollection: invocation.TargetElement.TypeReference.IsCollection));
            }
        }
    }
}