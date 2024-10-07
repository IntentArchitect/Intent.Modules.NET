using Intent.Metadata.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.FastEndpoints.Templates.Endpoint.Models;

public class ServiceEndpointParameterModel : IEndpointParameterModel
{
    public ServiceEndpointParameterModel(
        string id,
        string name,
        ITypeReference typeReference,
        HttpInputSource? source,
        string? headerName,
        string? queryStringName,
        ICanBeReferencedType? mappedPayloadProperty,
        string? value)
    {
        Id = id;
        Name = name;
        TypeReference = typeReference;
        Source = source;
        HeaderName = headerName;
        QueryStringName = queryStringName;
        MappedPayloadProperty = mappedPayloadProperty;
        Value = value;
    }

    public string Id { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public HttpInputSource? Source { get; }
    public string? HeaderName { get; }
    public string? QueryStringName { get; }
    public ICanBeReferencedType? MappedPayloadProperty { get; }
    public string? Value { get; }
}