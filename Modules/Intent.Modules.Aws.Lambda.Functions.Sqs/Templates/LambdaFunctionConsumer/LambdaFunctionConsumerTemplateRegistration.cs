using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Aws.Lambda.Functions.Sqs.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Sqs.Templates.LambdaFunctionConsumer
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class LambdaFunctionConsumerTemplateRegistration : FilePerModelTemplateRegistration<IntegrationEventHandlerModel>
    {
        private readonly IMetadataManager _metadataManager;

        public LambdaFunctionConsumerTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => LambdaFunctionConsumerTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IntegrationEventHandlerModel model)
        {
            return new LambdaFunctionConsumerTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IntegrationEventHandlerModel> GetModels(IApplication application)
        {
            var allHandlers = _metadataManager.Services(application).GetIntegrationEventHandlerModels();
            // Filter handlers that have at least one Integration Event subscription applicable to this message broker
            var handlersWithEventSubscriptions = allHandlers
                .Where(handler => handler.IntegrationEventSubscriptions()
                    .Select(subscription => subscription.TypeReference.Element.AsMessageModel())
                    .FilterMessagesForThisMessageBroker(application, Constants.BrokerStereotypeIds)
                    .Any())
                .ToList();

            // Filter handlers that have at least one Integration Command subscription applicable to this message broker
            var handlersWithCommandSubscriptions = allHandlers
                .Where(handler => handler.IntegrationCommandSubscriptions()
                    .Select(subscription => subscription.TypeReference.Element.AsIntegrationCommandModel())
                    .FilterMessagesForThisMessageBroker(application, Constants.BrokerStereotypeIds)
                    .Any())
                .ToList();

            // Combine and return distinct handlers
            return handlersWithEventSubscriptions
                .Concat(handlersWithCommandSubscriptions)
                .Distinct()
                .ToList();
        }
    }
}