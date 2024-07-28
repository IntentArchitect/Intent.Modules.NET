#nullable enable
using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller;

public interface IControllerModel : IHasFolder, IHasName, IMetadataModel
{
    string? Route { get; }
    string? Comment { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IAuthorizationModel? AuthorizationModel { get; }
    public IList<IControllerOperationModel> Operations { get; }
    IList<IApiVersionModel> ApplicableVersions { get; }
    IElement? InternalElement => null;
}

public interface IControllerOperationModel : IHasName, IHasTypeReference, IMetadataModel
{
    string Comment { get; }
    ITypeReference ReturnType { get; }
    HttpVerb Verb { get; }
    string Route { get; }
    HttpMediaType? MediaType { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IAuthorizationModel AuthorizationModel { get; }
    IElement InternalElement { get; }
    IList<IControllerParameterModel> Parameters { get; }
    IList<IApiVersionModel> ApplicableVersions { get; }
    IControllerModel Controller { get; }
}

public interface IControllerParameterModel : IHasName, IHasTypeReference, IMetadataModel
{
    HttpInputSource? Source { get; }
    string HeaderName { get; }
    string QueryStringName { get; }
    ICanBeReferencedType MappedPayloadProperty { get; }
    string Value { get; }
}

public interface IAuthorizationModel
{
    ///<summary>
    /// Gets or sets the Authentication Schemes that determines access to this resource. Note the format will generate exactly in C#.
    ///</summary>
    [Obsolete("Set the authorization scheme through a factory extension. This will be removed in later versions.")]
    public string AuthenticationSchemesExpression { get; }

    ///<summary>
    /// Gets or sets the policy name that determines access to the resource. Note the format will generate exactly in C#.
    ///</summary>
    public string? Policy { get; }

    ///<summary>
    /// Gets or sets the Roles that determines access to this Resource. Note the format will generate exactly in C#.
    ///</summary>
    public string? RolesExpression { get; }
}

public interface IApiVersionModel
{
    public string DefinitionName { get; }
    public string Version { get; }
    public bool IsDeprecated { get; }
}