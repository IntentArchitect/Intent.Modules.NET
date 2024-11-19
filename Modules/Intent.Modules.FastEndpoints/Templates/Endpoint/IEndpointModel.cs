using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.FastEndpoints.Templates.Endpoint;


// We need IHasFolder so we add subfolders to the template output paths as needed
public interface IEndpointContainerModel : IHasName, IMetadataModel, IHasFolder
{
    IElement? InternalElement { get; }
    IReadOnlyCollection<IEndpointModel> Endpoints { get; }
    IReadOnlyCollection<IApiVersionModel> ApplicableVersions { get; }
}

// We need IHasFolder so we add subfolders to the template output paths as needed
public interface IEndpointModel : IHasName, IHasTypeReference, IMetadataModel, IHasFolder
{
    string Comment { get; }
    ITypeReference? ReturnType { get; }
    HttpVerb Verb { get; }
    string? Route { get; }
    HttpMediaType? MediaType { get; }
    IElement InternalElement { get; }
    IEndpointContainerModel Container { get; }
    IList<IEndpointParameterModel> Parameters { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    IReadOnlyCollection<IApiVersionModel> ApplicableVersions { get; }
}

public interface IEndpointParameterModel : IHasName, IHasTypeReference, IMetadataModel
{
    HttpInputSource? Source { get; }
    string? HeaderName { get; }
    string? QueryStringName { get; }
    ICanBeReferencedType? MappedPayloadProperty { get; }
    string? Value { get; }
}

public interface IApiVersionModel
{
    public string DefinitionName { get; }
    public string Version { get; }
    public bool IsDeprecated { get; }
}