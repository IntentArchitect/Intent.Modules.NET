using System.Collections.Generic;
using System.Linq;
using Intent.Eventing.MassTransit.RequestResponse.Api;
using Intent.Modelers.ServiceProxies.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperRequestMessage;
using Intent.Modules.Eventing.MassTransit.Templates.ClientContracts;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Producers;

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.ConfigurationFactories;

internal class CommandQueryProducerFactory : IProducerFactory
{
    private readonly MassTransitConfigurationTemplate _template;

    public CommandQueryProducerFactory(MassTransitConfigurationTemplate template)
    {
        _template = template;
    }

    public IReadOnlyCollection<Producer> CreateProducers()
    {
        var proxyMappedService = new MassTransitServiceProxyMappedService();

        var serviceProxies = _template.ExecutionContext.MetadataManager
            .ServiceProxies(_template.ExecutionContext.GetApplicationConfig().Id)
            .GetServiceProxyModels();
        var results = serviceProxies.SelectMany(proxyModel => proxyMappedService.GetMappedEndpoints(proxyModel))
            .Select(endpoint =>
            {
                var dtoFullName = _template.GetFullyQualifiedTypeName(MapperRequestMessageTemplate.TemplateId, endpoint.Id);

                var queueName = endpoint switch
                {
                    var element when endpoint.InternalElement.SpecializationType == CommandModel.SpecializationType => new CommandModel(endpoint.InternalElement)
                        .GetMessageTriggered()?.QueueName(),
                    var element when endpoint.InternalElement.SpecializationType == QueryModel.SpecializationType => new QueryModel(endpoint.InternalElement).GetMessageTriggered()
                        ?.QueueName(),
                    _ => null
                };

                if (string.IsNullOrWhiteSpace(queueName))
                {
                    queueName = $"{dtoFullName.ToKebabCase()}";
                }

                if (!queueName.StartsWith("queue:"))
                {
                    queueName = "queue:" + queueName;
                }

                return new Producer
                {
                    MessageTypeName = dtoFullName,
                    Urn = queueName
                };
            })
            .ToArray();
        return results;
    }
}