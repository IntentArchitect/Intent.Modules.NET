using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modules.Common.Types.Api;
using Intent.SdkEvolutionHelpers;
using JetBrains.Annotations;

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller;

public interface IControllerModel : IHasFolder, IHasName, IMetadataModel
{
    string Route { get; }
    string Comment { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IAuthorizationModel AuthorizationModel { get; }
    public IList<IControllerOperationModel> Operations { get; }
}

public interface IControllerOperationModel : IHasName, IHasTypeReference, IMetadataModel
{
    string Comment { get; }
    ITypeReference ReturnType { get; }
    HttpVerb Verb { get; }
    string Route { get; }
    MediaTypeOptions MediaType { get; }
    bool RequiresAuthorization { get; }
    bool AllowAnonymous { get; }
    IAuthorizationModel AuthorizationModel { get; }
    IElement InternalElement { get; }
    IList<IControllerParameterModel> Parameters { get; }
}

public interface IControllerParameterModel : IHasName, IHasTypeReference, IMetadataModel
{
    SourceOptionsEnum Source { get; }
    [CanBeNull] string HeaderName { get; }
    ICanBeReferencedType MappedPayloadProperty { get; }
}

public interface IAuthorizationModel
{
    ///<summary>
    /// Gets or sets the Authentication Schemes that determines access to this resource. Note the format will generate exactly in C#.
    ///</summary>
    public string AuthenticationSchemesExpression { get; }

    ///<summary>
    /// Gets or sets the policy name that determines access to the resource. Note the format will generate exactly in C#.
    ///</summary>
    public string Policy { get; }

    ///<summary>
    /// Gets or sets the Roles that determines access to this Resource. Note the format will generate exactly in C#.
    ///</summary>
    public string RolesExpression { get; }
}

public enum HttpVerb
{
    Get,
    Post,
    Put,
    Patch,
    Delete,
}

public enum MediaTypeOptions
{
    Default,
    ApplicationJson,
}

public enum SourceOptionsEnum
{
    Default,
    FromQuery,
    FromBody,
    FromForm,
    FromRoute,
    FromHeader,
}