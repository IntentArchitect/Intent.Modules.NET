using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Aws.Lambda.Functions.Api;

public interface ILambdaFunctionModel : IMetadataModel, IHasName, IHasTypeReference, IHasStereotypes
{
    string Comment { get; }
    ITypeReference? ReturnType { get; }
    HttpVerb Verb { get; }
    string? Route { get; }
    HttpMediaType? MediaType { get; }
    IElement InternalElement { get; }
    ILambdaFunctionContainerModel Container { get; }
    IList<IEndpointParameterModel> Parameters { get; }
}