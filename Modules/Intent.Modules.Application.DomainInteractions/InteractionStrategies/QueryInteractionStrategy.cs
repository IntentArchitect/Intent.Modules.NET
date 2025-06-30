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
        //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsQueryEntityActionTargetEndModel() && interaction.Mappings?.GetQueryEntityMapping() != null;
        }

        public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var interaction = (IAssociationEnd)interactionElement;
            try
            {
                var associationEnd = interaction;
                var foundEntity = interaction.TypeReference.Element.AsClassModel();
                var queryMapping = interaction.Mappings.GetQueryEntityMapping();
                if (queryMapping == null)
                {
                    throw new ElementException(interaction, "Query Entity Mapping has not been specified.");
                }

                var entityVariableName = associationEnd.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);

                var _template = method.File.Template;
                var _csharpMapping = method.GetMappingManager();
                var queryContext = new QueryActionContext(method, ActionType.Query, interaction);
                _csharpMapping.SetFromReplacement(foundEntity, entityVariableName);
                _csharpMapping.SetFromReplacement(associationEnd, entityVariableName);
                _csharpMapping.SetToReplacement(foundEntity, entityVariableName);
                _csharpMapping.SetToReplacement(associationEnd, entityVariableName);

                var queryStatements = method.GetQueryStatements(interaction, queryContext);
                method.AddStatements(queryStatements);
            }
            catch (ElementException ex)
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