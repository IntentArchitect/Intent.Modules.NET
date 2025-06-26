using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;

public interface IServiceContractModel : IMetadataModel, IHasName, IElementWrapper
{
    IReadOnlyList<IServiceContractOperationModel?> Operations { get; }
}

public interface IServiceContractOperationModel : IMetadataModel, IHasName, IHasTypeReference, IElementWrapper
{
    IReadOnlyList<IServiceContractOperationParameterModel> Parameters { get; }
}

public interface IServiceContractOperationParameterModel : IMetadataModel, IHasName, IHasTypeReference;