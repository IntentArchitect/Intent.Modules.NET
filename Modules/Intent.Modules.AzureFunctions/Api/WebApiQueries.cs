using System;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.AzureFunctions.Api;

public static class WebApiQueries
{
    public static IAzureFunctionParameterModel GetRequestDtoParameter(this IAzureFunctionModel operationModel)
    {
        var endpointInfo = HttpEndpointModelFactory.GetEndpoint(operationModel.InternalElement);
        var dtoParams = operationModel.Parameters
            .Where(IsParameterBody)
            .ToArray();
        switch (dtoParams.Length)
        {
            case 0:
                return null;
            case > 1:
                throw new Exception($"Multiple DTOs not supported on {operationModel.Name} operation");
            default:
                {
                    var param = dtoParams.First();
                    return param;
                }
        }
    }

    private static bool IsParameterBody(IAzureFunctionParameterModel parameterModel)
    {
        return parameterModel.GetHttpInputSource() == HttpInputSource.FromBody;
    }
}