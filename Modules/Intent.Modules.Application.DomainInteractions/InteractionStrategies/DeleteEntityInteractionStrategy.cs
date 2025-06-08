using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.DomainInteractions.InteractionStrategies
{
    public class DeleteEntityInteractionStrategy : IInteractionStrategy
    {
        //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsDeleteEntityActionTargetEndModel();
        }

        public void ImplementInteraction(CSharpClassMethod method, IElement interactionElement)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var interaction = (IAssociationEnd)interactionElement;
            method.AddStatements(method.GetQueryStatements(interaction, new QueryActionContext(method, ActionType.Delete, interaction)));
            method.AddStatement(string.Empty);

            var deleteAction = interaction.AsDeleteEntityActionTargetEndModel();
            try
            {
                var entityDetails = method.TrackedEntities()[deleteAction.Id];
                var statements = new List<CSharpStatement>();
                if (entityDetails.IsCollection)
                {
                    statements.Add(new CSharpForEachStatement(entityDetails.VariableName.Singularize(), entityDetails.VariableName)
                        .AddStatement(entityDetails.DataAccessProvider.Remove(entityDetails.VariableName.Singularize()))
                        .SeparatedFromPrevious());
                }
                else
                {
                    statements.Add(entityDetails.DataAccessProvider.Remove(entityDetails.VariableName)
                        .SeparatedFromPrevious());
                }
                method.AddStatements(statements);
            }
            catch (Exception ex)
            {
                throw new ElementException(deleteAction.InternalAssociationEnd, "An error occurred while generating the domain interactions logic", ex);
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