using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Modules.Common.UnitOfWork.Settings;
using static Intent.Modules.Constants.TemplateRoles.Domain;

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

            var queryContext = new QueryActionContext(method, ActionType.Delete, interaction);
            var foundEntity = interaction.TypeReference.Element.AsClassModel();
            var dataAccess = method.InjectDataAccessProvider(foundEntity, queryContext);
            var projectedType = queryContext.ImplementWithProjections() && dataAccess.IsUsingProjections
                ? queryContext.GetDtoProjectionReturnType()
                : null;

            method.AddStatements(ExecutionPhases.BusinessLogic, method.GetQueryStatements(
                dataAccessProvider: dataAccess,
                interaction: interaction,
                foundEntity: foundEntity,
                projectedType: projectedType));

            method.AddStatement(ExecutionPhases.BusinessLogic, string.Empty);

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
                throw new ElementException(deleteAction.InternalAssociationEnd, "An error occurred while generating the domain interactions logic", ex);
            }
        }
    }
}