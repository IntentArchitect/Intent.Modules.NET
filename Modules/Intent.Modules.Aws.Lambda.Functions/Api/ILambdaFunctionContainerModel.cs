using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Aws.Lambda.Functions.Api;

public interface ILambdaFunctionContainerModel: IMetadataModel, IHasName, IHasTypeReference, IHasFolder<IFolder>, IHasStereotypes
{
    IElement? InternalElement { get; }
    IReadOnlyCollection<ILambdaFunctionModel> Endpoints { get; }
}