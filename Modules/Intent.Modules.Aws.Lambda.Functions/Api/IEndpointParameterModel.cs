using Intent.Metadata.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Aws.Lambda.Functions.Api;

public interface IEndpointParameterModel : IHasName, IHasTypeReference, IMetadataModel
{
    HttpInputSource? Source { get; }
    string? HeaderName { get; }
    string? QueryStringName { get; }
    ICanBeReferencedType? MappedPayloadProperty { get; }
    string? Value { get; }
}