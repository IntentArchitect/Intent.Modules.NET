using System;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Application.DomainInteractions.Mapping.Resolvers;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Application.DomainInteractions.InteractionStrategies
{
    public class ProcessingActionInteractionStrategy : IInteractionStrategy
    {
        //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsProcessingActionModel();
        }

        public void ImplementInteraction(ICSharpClassMethodDeclaration method, IElement interactionElement)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var t = (ICSharpFileBuilderTemplate)method.File.Template;
            var csharpMapping = method.GetMappingManager();
            csharpMapping.AddMappingResolver(new EntityUpdateMappingTypeResolver(t));
            csharpMapping.AddMappingResolver(new StandardDomainMappingTypeResolver(t));
            csharpMapping.AddMappingResolver(new ValueObjectMappingTypeResolver(t));
            csharpMapping.AddMappingResolver(new DataContractMappingTypeResolver(t));
            var handlerClass = method.Class;
            var actions = interactionElement.AsProcessingActionModel();
            try
            {
                var processingStatements = csharpMapping.GenerateUpdateStatements(actions.InternalElement.Mappings.Single())
                    .Select(x =>
                    {
                        if (x is CSharpAssignmentStatement)
                        {
                            x.WithSemicolon();
                        }

                        return x;
                    }).ToList();

                handlerClass.WireupDomainServicesForProcessingAction(actions.InternalElement.Mappings.Single(), processingStatements);
                processingStatements.FirstOrDefault()?.SeparatedFromPrevious();
                method.AddStatements(ExecutionPhases.BusinessLogic, processingStatements);
            }
            catch (Exception ex)
            {
                throw new ElementException(actions.InternalElement, "An error occurred while generating processing action logic", ex);
            }
        }
    }
}