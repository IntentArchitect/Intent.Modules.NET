using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared;

namespace Intent.Modules.Eventing.MassTransit.Templates.ClientContracts;

public class MassTransitServiceProxyMappedService : IServiceProxyMappedService
{
    public bool HasMappedEndpoints(ServiceProxyModel model)
    {
        var result = model.Operations
            .Select(x => x.Mapping?.Element)
            .Any(IsMassTransitElement);
        return result;
    }

    public IReadOnlyCollection<MappedEndpoint> GetMappedEndpoints(ServiceProxyModel model)
    {
        var result = model.Operations
            .Select(x => x.Mapping?.Element)
            .Where(IsMassTransitElement)
            .Cast<IElement>()
            .Select(GetMappedEndpoint)
            .ToArray();
        return result;
    }

    private const string Command = nameof(Command);
    private const string Query = nameof(Query);

    private static bool IsMassTransitElement(ICanBeReferencedType? p)
    {
        return p is not null &&
               p.SpecializationType is Command or Query &&
               p.HasStereotype(MassTransit.RequestResponse.Constants.MessageTriggered);
    }

    private static MappedEndpoint GetMappedEndpoint(IElement element)
    {
        return new MappedEndpoint(
            Id: element.Id,
            Name: element.Name.RemoveSuffix(Command, Query),
            TypeReference: element.TypeReference,
            ReturnType: element.TypeReference,
            Inputs: GetMappedEndpointInputs(element),
            InternalElement: element);

        static IReadOnlyCollection<MappedEndpointInput> GetMappedEndpointInputs(ICanBeReferencedType element)
        {
            var input = element.SpecializationType switch
            {
                Command => new MappedEndpointInput("command", element),
                Query => new MappedEndpointInput("query", element),
                _ => throw new Exception("")
            };
            return new[] { input };
        }
    }
}