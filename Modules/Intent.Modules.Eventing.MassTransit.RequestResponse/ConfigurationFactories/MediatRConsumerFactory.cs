using System.Collections.Generic;
using System.Linq;
using Intent.Eventing.MassTransit.RequestResponse.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Contracts.Clients.Shared.FileNamespaceProviders;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperRequestMessage;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Consumers;

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.ConfigurationFactories;

internal class MediatRConsumerFactory : IConsumerFactory
{
    private readonly MassTransitConfigurationTemplate _template;

    public MediatRConsumerFactory(MassTransitConfigurationTemplate template)
    {
        _template = template;
    }

    public IReadOnlyCollection<Consumer> CreateConsumers()
    {
        var namespaceProvider = new SourcePackageFileNamespaceProvider();
        var services = _template.ExecutionContext.MetadataManager.Services(_template.ExecutionContext.GetApplicationConfig().Id);
        var relevantCommands = services.GetElementsOfType("Command")
            .Where(p => p.HasStereotype(Constants.MessageTriggered) && _template.ExecutionContext.TemplateExists(TemplateRoles.Application.Handler.Command, p.Id));
        var relevantQueries = services.GetElementsOfType("Query")
            .Where(p => p.HasStereotype(Constants.MessageTriggered) && _template.ExecutionContext.TemplateExists(TemplateRoles.Application.Handler.Query, p.Id));
        var consumers = relevantCommands.Concat(relevantQueries)
            .Select(commandQuery =>
            {
                var fullTypeName = _template.GetFullyQualifiedTypeName(MapperRequestMessageTemplate.TemplateId, commandQuery);
                var t = _template.GetTemplate<MapperRequestMessageTemplate>(MapperRequestMessageTemplate.TemplateId, commandQuery.Id);

                var destinationAddress = commandQuery switch
                {
                    _ when commandQuery.SpecializationType == CommandModel.SpecializationType => new CommandModel(commandQuery).GetMessageTriggered()?.QueueName(),
                    _ when commandQuery.SpecializationType == QueryModel.SpecializationType => new QueryModel(commandQuery).GetMessageTriggered()?.QueueName(),
                    _ => null
                };
                if (string.IsNullOrWhiteSpace(destinationAddress))
                {
                    destinationAddress = $"{namespaceProvider.GetFileNamespace(t)}.{t.ClassName}".ToKebabCase();
                }

                return new Consumer
                {
                    Message = new MessageDetail
                    {
                        MessageName = commandQuery.Name,
                        MessageTypeFullName = fullTypeName,
                        TopicNameOverride = null
                    },
                    ConsumerTypeName = $@"{_template.GetMediatRConsumerName()}<{fullTypeName}>",
                    ConsumerDefinitionTypeName = $"{_template.GetMediatRConsumerName()}Definition<{fullTypeName}>",
                    IsSpecificMessageConsumer = true,
                    DestinationAddress = destinationAddress
                };
            })
            .ToList();
        return consumers;
    }
}