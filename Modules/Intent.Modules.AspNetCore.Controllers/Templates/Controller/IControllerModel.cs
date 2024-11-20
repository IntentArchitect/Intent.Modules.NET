#nullable enable
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller;

public interface IControllerModel : IHasFolder, IHasName, IMetadataModel
{
    string? Route { get; }
    string? Comment { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    public IList<IControllerOperationModel> Operations { get; }
    IList<IApiVersionModel> ApplicableVersions { get; }
    IElement? InternalElement => null;
}

public interface IControllerOperationModel : IHasName, IHasTypeReference, IMetadataModel
{
    string Comment { get; }
    ITypeReference? ReturnType { get; }
    HttpVerb Verb { get; }
    string? Route { get; }
    HttpMediaType? MediaType { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    IElement InternalElement { get; }
    IList<IControllerParameterModel> Parameters { get; }
    IList<IApiVersionModel> ApplicableVersions { get; }
    IControllerModel Controller { get; }
}

public interface IControllerParameterModel : IHasName, IHasTypeReference, IMetadataModel
{
    HttpInputSource? Source { get; }
    string? HeaderName { get; }
    string? QueryStringName { get; }
    ICanBeReferencedType? MappedPayloadProperty { get; }
    string? Value { get; }
}

public interface IApiVersionModel
{
    public string? DefinitionName { get; }
    public string Version { get; }
    public bool IsDeprecated { get; }
}