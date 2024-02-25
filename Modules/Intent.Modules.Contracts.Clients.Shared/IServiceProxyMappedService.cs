using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;

namespace Intent.Modules.Contracts.Clients.Shared
{
    public interface IServiceProxyMappedService
    {
        bool HasMappedEndpoints(ServiceProxyModel model);
        IReadOnlyCollection<MappedEndpoint> GetMappedEndpoints(ServiceProxyModel model);
    }

    public record MappedEndpoint(
        string Id,
        string Name,
        ITypeReference TypeReference,
        ITypeReference ReturnType,
        IReadOnlyCollection<MappedEndpointInput> Inputs,
        IElement InternalElement) : IHasName;

    public record MappedEndpointInput : IHasName, IHasTypeReference, IMetadataModel
    {
        public MappedEndpointInput(string id, string name, ITypeReference typeReference)
        {
            Id = id;
            Name = name;
            TypeReference = typeReference;
        }

        public MappedEndpointInput(string name, ICanBeReferencedType referencedType)
        {
            Id = referencedType.Id;
            Name = name;
            TypeReference = new AdaptTypeReference(referencedType);
        }

        public string Id { get; }
        public string Name { get; }
        public ITypeReference TypeReference { get; }
    };

    internal class AdaptTypeReference : ITypeReference
    {
        public AdaptTypeReference(ICanBeReferencedType referencedType)
        {
            Stereotypes = referencedType.Stereotypes;
            IsNullable = false;
            IsCollection = false;
            GenericTypeParameters = Enumerable.Empty<ITypeReference>();
            Element = referencedType;
        }

        public IEnumerable<IStereotype> Stereotypes { get; }
        public bool IsNullable { get; }
        public bool IsCollection { get; }
        public ICanBeReferencedType Element { get; }
        public IEnumerable<ITypeReference> GenericTypeParameters { get; }
    }
}
