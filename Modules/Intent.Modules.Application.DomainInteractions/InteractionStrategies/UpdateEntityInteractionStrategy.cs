using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using Intent.Utils;
using Microsoft.VisualBasic;

namespace Intent.Modules.Eventing.Contracts.InteractionStrategies
{
    public class UpdateEntityInteractionStrategy : IInteractionStrategy
    {
        //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsUpdateEntityActionTargetEndModel();
        }

        public void ImplementInteraction(CSharpClassMethod method, IElement interactionElement)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var interaction = (IAssociationEnd)interactionElement;
            method.AddStatements(method.GetQueryStatements(interaction, new QueryActionContext(method, ActionType.Update, interaction)));
            method.AddStatement(string.Empty);

            var updateAction = interaction.AsUpdateEntityActionTargetEndModel();
            var _csharpMapping = method.GetMappingManager();
            var _template = method.File.Template;
            try
            {
                var entityDetails = method.TrackedEntities()[updateAction.Id];
                var entity = entityDetails.ElementModel.AsClassModel();
                var updateMapping = updateAction.Mappings.GetUpdateEntityMapping();

                var statements = new List<CSharpStatement>();

                if (entityDetails.IsCollection)
                {
                    _csharpMapping.SetToReplacement(entity, entityDetails.VariableName.Singularize());
                    if (updateMapping != null)
                    {
                        statements.Add(new CSharpForEachStatement(entityDetails.VariableName.Singularize(), entityDetails.VariableName)
                            .AddStatements(_csharpMapping.GenerateUpdateStatements(updateMapping)));
                    }

                    if (RepositoryRequiresExplicitUpdate(_template, entity))
                    {
                        statements.Add(entityDetails.DataAccessProvider.Update(entityDetails.VariableName.Singularize())
                            .SeparatedFromPrevious());
                    }
                }
                else
                {
                    if (updateMapping != null)
                    {
                        var updateStatements = _csharpMapping.GenerateUpdateStatements(updateMapping);
                        method.Class.WireupDomainServicesForOperations(updateAction, updateStatements);
                        AdjustOperationInvocationForAsyncAndReturn(method, updateMapping, updateStatements);

                        statements.AddRange(updateStatements);
                    }

                    if (RepositoryRequiresExplicitUpdate(_template, entity))
                    {
                        statements.Add(entityDetails.DataAccessProvider.Update(entityDetails.VariableName)
                            .SeparatedFromPrevious());
                    }
                }

                if (RequiresAggegateExplicitUpdate(entityDetails))
                {
                    statements.Add(entityDetails.DataAccessProvider.Update(entityDetails.VariableName)
                        .SeparatedFromPrevious());
                }

                method.AddStatements(statements);
            }
            catch (Exception ex)
            {
                throw new ElementException(updateAction.InternalAssociationEnd, "An error occurred while generating the domain interactions logic", ex);
            }
        }

        private bool RepositoryRequiresExplicitUpdate(ICSharpTemplate _template, IMetadataModel forEntity)
        {
            return _template.TryGetTemplate<ICSharpFileBuilderTemplate>(
                       TemplateRoles.Repository.Interface.Entity,
                       forEntity,
                       out var repositoryInterfaceTemplate) &&
                   repositoryInterfaceTemplate.CSharpFile.Interfaces[0].TryGetMetadata<bool>("requires-explicit-update", out var requiresUpdate) &&
                   requiresUpdate;
        }

        private bool RequiresAggegateExplicitUpdate(EntityDetails entityDetails)
        {
            if (entityDetails.DataAccessProvider is CompositeDataAccessProvider cda)
            {
                return cda.RequiresExplicitUpdate();
            }
            return false;
        }

        private void AdjustOperationInvocationForAsyncAndReturn(CSharpClassMethod method, IElementToElementMapping updateMapping, IList<CSharpStatement> updateStatements)
        {

            if (updateMapping.MappedEnds.Any(me => OperationModelExtensions.IsOperationModel(me.TargetElement)))
            {
                foreach (var invocation in updateMapping.MappedEnds.Where(me => OperationModelExtensions.IsOperationModel(me.TargetElement)))
                {

                    var operationName = ((IElement)invocation.TargetElement).Name;
                    var variableName = $"{operationName.ToCamelCase()}Result";
                    bool hasReturn = invocation.TargetElement.TypeReference?.Element != null;

                    for (int i = 0; i < updateStatements.Count; i++)
                    {
                        if (updateStatements[i] is CSharpInvocationStatement s && s.Expression.Reference is ICSharpMethodDeclaration md && md.Name == operationName)
                        {
                            if (s.IsAsyncInvocation())
                            {
                                s.AddArgument("cancellationToken");
                                updateStatements[i] = new CSharpAwaitExpression(updateStatements[i]);
                            }
                            if (hasReturn)
                            {
                                updateStatements[i] = new CSharpAssignmentStatement(new CSharpVariableDeclaration(variableName), updateStatements[i]);
                            }
                        }
                    }

                    method.TrackedEntities().Add(invocation.TargetElement.Id, new EntityDetails((IElement)invocation.TargetElement.TypeReference.Element, variableName, null, false, null, invocation.TargetElement.TypeReference.IsCollection));
                }
            }
        }
    }
}