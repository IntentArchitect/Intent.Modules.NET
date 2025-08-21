using Intent.Metadata.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Aws.Lambda.Functions.Api;

public class TraditionalServiceEndpointParameterModel : IEndpointParameterModel
{
    public TraditionalServiceEndpointParameterModel(
        string id,
        string name,
        ITypeReference typeReference,
        HttpInputSource? source = null,
        string? headerName = null,
        string? queryStringName = null,
        ICanBeReferencedType? mappedPayloadProperty = null,
        string? value = null)
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
    
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public string Id { get; }
    public HttpInputSource? Source { get; }
    public string? HeaderName { get; }
    public string? QueryStringName { get; }
    public ICanBeReferencedType? MappedPayloadProperty { get; }
    public string? Value { get; }
}