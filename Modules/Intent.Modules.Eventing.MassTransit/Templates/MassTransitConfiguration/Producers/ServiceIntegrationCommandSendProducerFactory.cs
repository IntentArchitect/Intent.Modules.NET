using System.Collections.Generic;
using System.Linq;
using Intent.Eventing.MassTransit.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.EventInteractions;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Producers;

internal class ServiceIntegrationCommandSendProducerFactory : IProducerFactory
{
    private readonly MassTransitConfigurationTemplate _template;

    public ServiceIntegrationCommandSendProducerFactory(MassTransitConfigurationTemplate template)
    {
        _template = template;
    }

    public IReadOnlyCollection<Producer> CreateProducers()
    {
        var results = _template.ExecutionContext.MetadataManager
            .GetExplicitlySentIntegrationCommandDispatches(_template.ExecutionContext.GetApplicationConfig().Id)
            .Select(subscription =>
            {
                var model = subscription.TypeReference.Element.AsIntegrationCommandModel();
                var queueName = subscription.GetCommandDistribution().DestinationQueueName();
                if (string.IsNullOrWhiteSpace(queueName))
                {
                    queueName = $"{_template.GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, model).ToKebabCase()}";
                }

                if (!queueName.StartsWith("queue:"))
                {
                    queueName = "queue:" + queueName;
                }

                return new Producer
                {
                    MessageTypeName = _template.GetFullyQualifiedTypeName(IntegrationCommandTemplate.TemplateId, model),
                    Urn = queueName
                };
            })
            .ToArray();
        return results;
    }
}