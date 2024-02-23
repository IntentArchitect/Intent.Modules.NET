using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.ServiceProxies.Api;
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
                return new Producer(dtoFullName, dtoFullName.ToKebabCase());
            })
            .ToArray();
        return results;
    }
}