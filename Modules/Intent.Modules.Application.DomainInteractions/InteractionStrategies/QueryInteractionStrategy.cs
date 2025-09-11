using System;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.DomainInteractions.InteractionStrategies
{
    public class QueryInteractionStrategy : IInteractionStrategy
    {
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsQueryEntityActionTargetEndModel() && interaction.Mappings?.GetQueryEntityMapping() != null;
        }

        public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
        {
            var interaction = (IAssociationEnd)interactionElement;
            ArgumentNullException.ThrowIfNull(method);

            try
            {
                var foundEntity = interaction.TypeReference.Element.AsClassModel();
                var queryMapping = interaction.Mappings.GetQueryEntityMapping();
                if (queryMapping == null)
                {
                    throw new ElementException(interaction, "Query Entity Mapping has not been specified.");
                }

                var entityVariableName = interaction.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);

                var csharpMapping = method.GetMappingManager();
                var queryContext = new QueryActionContext(method, ActionType.Query, interaction);
                csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
                csharpMapping.SetFromReplacement(interaction, entityVariableName);
                csharpMapping.SetToReplacement(foundEntity, entityVariableName);
                csharpMapping.SetToReplacement(interaction, entityVariableName);

                var queryStatements = method.GetQueryStatements(interaction, queryContext);
                method.AddStatements(queryStatements);
            }
            catch (ElementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ElementException(interaction, "An error occurred while generating the domain interactions logic", ex);
            }
        }
    }
}