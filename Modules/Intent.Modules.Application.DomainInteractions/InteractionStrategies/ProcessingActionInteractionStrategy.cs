using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.DomainInteractions.Api;
using Intent.Modules.Application.DomainInteractions;
using Intent.Modules.Application.DomainInteractions.Extensions;
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
    public class ProcessingActionInteractionStrategy : IInteractionStrategy
    {
        //public Dictionary<string, EntityDetails> TrackedEntities { get; set; } = new();
        public bool IsMatch(IElement interaction)
        {
            return interaction.IsProcessingActionModel();
        }

        public void ImplementInteraction(CSharpClassMethod method, IElement interactionElement)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var _csharpMapping = method.GetMappingManager();
            var handlerClass = method.Class;
            var actions = interactionElement.AsProcessingActionModel();
            try
            {
                var processingStatements = _csharpMapping.GenerateUpdateStatements(actions.InternalElement.Mappings.Single())
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