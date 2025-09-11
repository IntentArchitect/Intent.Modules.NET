using System;
using System.Collections.Generic;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.DomainInteractions.InteractionStrategies
{
    public class DeleteEntityInteractionStrategy : IInteractionStrategy
    {
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsDeleteEntityActionTargetEndModel();
        }

        public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
        {
            ArgumentNullException.ThrowIfNull(method);
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
                        .AddStatement<CSharpForEachStatement, CSharpStatement>(entityDetails.DataAccessProvider.Remove(entityDetails.VariableName.Singularize()))
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
    }
}