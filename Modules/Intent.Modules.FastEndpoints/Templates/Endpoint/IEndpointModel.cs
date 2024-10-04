using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.FastEndpoints.Templates.Endpoint;

public interface IEndpointContainerModel : IHasFolder, IHasName, IMetadataModel
{
    string Name { get; }
    IElement InternalElement { get; }
    IList<IEndpointModel> Endpoints { get; }
}

public interface IEndpointModel : IHasName, IHasTypeReference, IMetadataModel, IHasFolder
{
    string Comment { get; }
    ITypeReference? ReturnType { get; }
    HttpVerb Verb { get; }
    string Route { get; }
    HttpMediaType? MediaType { get; }
    IElement InternalElement { get; }
    IEndpointContainerModel Container { get; }
    IList<IEndpointParameterModel> Parameters { get; }
}

public interface IEndpointParameterModel : IHasName, IHasTypeReference, IMetadataModel
{
    HttpInputSource? Source { get; }
    string? HeaderName { get; }
    string? QueryStringName { get; }
    ICanBeReferencedType MappedPayloadProperty { get; }
    string Value { get; }
}