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
    public class CreateEntityInteractionStrategy : IInteractionStrategy
    {
        //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();
        public bool IsMatch(IAssociationEnd interaction)
        {
            return interaction.IsCreateEntityActionTargetEndModel();
        }

        public void ImplementInteraction(CSharpClassMethod method, IAssociationEnd interaction)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            try
            {
                var createAction = interaction.AsCreateEntityActionTargetEndModel();
                var handlerClass = method.Class;
                var _csharpMapping = method.GetMappingManager();
                var entity = createAction.TypeReference.Element.AsClassModel() ?? createAction.TypeReference.Element.AsClassConstructorModel().ParentClass;

                var entityVariableName = createAction.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);
                var dataAccess = method.InjectDataAccessProvider(entity);

                method.TrackedEntities().Add(createAction.Id, new EntityDetails(entity.InternalElement, entityVariableName, dataAccess, true));

                var mapping = createAction.Mappings.SingleOrDefault();
                var statements = new List<CSharpStatement>();

                if (dataAccess.MustAccessEntityThroughAggregate())
                {
                    if (!method.TryGetFindAggregateStatements(mapping.SourceElement as IElement, entity, out statements))
                    {
                        Logging.Log.Warning($"Unable to implement creation logic for handler '{handlerClass.Name}'. See earlier warnings for more information.");
                        return;
                    }
                }

                if (mapping != null)
                {
                    var constructionStatement = _csharpMapping.GenerateCreationStatement(mapping);

                    handlerClass.WireupDomainServicesForConstructors(createAction, constructionStatement);

                    statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(entityVariableName), constructionStatement).WithSemicolon());
                }
                else
                {
                    statements.Add(new CSharpAssignmentStatement(new CSharpVariableDeclaration(entityVariableName), $"new {entity.Name}();"));
                }

                statements.Add(dataAccess.AddEntity(entityVariableName).SeparatedFromPrevious());

                _csharpMapping.SetFromReplacement(interaction, entityVariableName);
                _csharpMapping.SetFromReplacement(entity, entityVariableName);
                _csharpMapping.SetToReplacement(interaction, entityVariableName);
                _csharpMapping.SetToReplacement(entity, entityVariableName);

                method.AddStatements(statements);
            }
            catch (Exception ex)
            {
                throw new ElementException(interaction, "An error occurred while generating the domain interactions logic", ex);
            }
        }
    }
}