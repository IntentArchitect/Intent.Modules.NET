using Intent.Metadata.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Aws.Lambda.Functions.Api;

public class EndpointParameterModel : IEndpointParameterModel
{
    public EndpointParameterModel(IHttpEndpointInputModel model)
    {
        Name = model.Name;
        TypeReference = model.TypeReference;
        Id = model.Id;
        Source = model.Source;
        HeaderName = model.HeaderName;
        QueryStringName = model.QueryStringName;
        MappedPayloadProperty = model.MappedPayloadProperty;
        Value = model.Value;
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