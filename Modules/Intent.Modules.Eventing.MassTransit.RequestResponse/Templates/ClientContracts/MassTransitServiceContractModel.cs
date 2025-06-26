using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts;

public class MassTransitServiceContractModel(ServiceProxyModel model) : IServiceContractModel
{
    private const string CommandTypeId = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";
    private const string QueryTypeId = "e71b0662-e29d-4db2-868b-8a12464b25d0";

    public string Id => model.Id;
    public string Name => model.Name;
    public IElement InternalElement => model.InternalElement;

    public IReadOnlyList<IServiceContractOperationModel> Operations { get; } = model.Operations
        .Select(x => x.Mapping?.Element as IElement)
        .Where(x => x?.SpecializationTypeId is CommandTypeId or QueryTypeId &&
                    x.HasStereotype(Constants.MessageTriggered))
        .Select(x => new OperationModel(x))
        .ToArray();

    private class OperationModel(IElement element) : IServiceContractOperationModel
    {
        public string Id => element.Id;
        public string Name { get; } = element.Name.RemoveSuffix("Command", "Query");
        public ITypeReference TypeReference => element.TypeReference;
        public IElement InternalElement => element;

        public IReadOnlyList<IServiceContractOperationParameterModel> Parameters { get; } = [new ParameterModel(element)];
    }

    private class ParameterModel(IElement element) : IServiceContractOperationParameterModel
    {
        public string Id => element.Id;
        public string Name { get; } = element.SpecializationTypeId switch
        {
            CommandTypeId => "command",
            QueryTypeId => "query",
            _ => throw new InvalidOperationException($"Unknown type: {element.SpecializationType} ({element.SpecializationTypeId})")
        };

        public ITypeReference TypeReference { get; } = new TypeReferenceAdapter(element);
    }

    private class TypeReferenceAdapter(ICanBeReferencedType referencedType) : ITypeReference
    {
        public IEnumerable<IStereotype> Stereotypes { get; } = referencedType.Stereotypes;
        public bool IsNullable { get; } = false;
        public bool IsCollection { get; } = false;
        public ICanBeReferencedType Element { get; } = referencedType;
        public IEnumerable<ITypeReference> GenericTypeParameters { get; } = [];
    }
}