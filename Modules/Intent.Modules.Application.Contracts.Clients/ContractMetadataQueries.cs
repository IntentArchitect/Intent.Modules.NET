using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Application.Contracts.Clients;

public static class ContractMetadataQueries
{
    private const string StereotypeAzureFunction = "Azure Function";
    
    public static bool IsAbleToReference(OperationModel operation)
    {
        return operation.HasStereotype(StereotypeAzureFunction) || operation.HasHttpSettings();
    }
}