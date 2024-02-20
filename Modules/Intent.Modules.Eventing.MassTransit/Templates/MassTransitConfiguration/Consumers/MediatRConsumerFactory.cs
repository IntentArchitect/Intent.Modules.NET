using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Consumers;

internal class MediatRConsumerFactory : IConsumerFactory
{
    private readonly MassTransitConfigurationTemplate _template;

    public MediatRConsumerFactory(MassTransitConfigurationTemplate template)
    {
        _template = template;
    }

    public IReadOnlyCollection<Consumer> CreateConsumers()
    {
        var services = _template.ExecutionContext.MetadataManager.Services(_template.ExecutionContext.GetApplicationConfig().Id);
        var relevantCommands = services.GetElementsOfType("Command")
            .Where(p => p.HasStereotype(Constants.MassTransitConsumerStereotype) && _template.ExecutionContext.TemplateExists(TemplateRoles.Application.Handler.Command, p.Id));
        var relevantQueries = services.GetElementsOfType("Query")
            .Where(p => p.HasStereotype(Constants.MassTransitConsumerStereotype) && _template.ExecutionContext.TemplateExists(TemplateRoles.Application.Handler.Query, p.Id));
        var consumers = relevantCommands.Concat(relevantQueries)
            .Select(commandQuery =>
            {
                var messageType = _template.GetTypeName(TemplateRoles.Application.Command, commandQuery, new TemplateDiscoveryOptions() { ThrowIfNotFound = false })
                                  ?? _template.GetTypeName(TemplateRoles.Application.Query, commandQuery, new TemplateDiscoveryOptions() { ThrowIfNotFound = false });

                return new Consumer(
                    MessageTypeFullName: _template.GetFullyQualifiedTypeName(commandQuery),
                    ConsumerTypeName: $@"{_template.GetMediatRConsumerName()}<{messageType}>",
                    ConsumerDefinitionTypeName: $"{_template.GetMediatRConsumerName()}Definition<{messageType}>",
                    ConfigureConsumeTopology: false,
                    DestinationAddress: null,
                    AzureConsumerSettings: null,
                    RabbitMqConsumerSettings: null);
            })
            .ToList();
        return consumers;
    }
}