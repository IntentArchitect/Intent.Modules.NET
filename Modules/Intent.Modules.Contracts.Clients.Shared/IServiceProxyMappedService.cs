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
        ITypeReference? ReturnType,
        IReadOnlyCollection<MappedEndpointInput> Inputs) : IHasName;

    public record MappedEndpointInput : IHasName, IHasTypeReference
    {
        public MappedEndpointInput(string name, ITypeReference typeReference)
        {
            Name = name;
            TypeReference = typeReference;
        }

        public MappedEndpointInput(string name, ICanBeReferencedType referencedType)
        {
            Name = name;
            TypeReference = new AdaptTypeReference(referencedType);
        }

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
