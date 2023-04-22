using Intent.AzureFunctions.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AzureFunctions.Dispatch.MediatR.Templates;

public class CqrsAzureFunctionParameterModel : IAzureFunctionParameterModel
{
    public static CqrsAzureFunctionParameterModel ForHttpTrigger(IHttpEndpointInputModel inputModel)
    {
        return new CqrsAzureFunctionParameterModel
        {
            Id = inputModel.Id,
            Name = inputModel.Name,
            TypeReference = inputModel.TypeReference,
            Type = inputModel.TypeReference,
            InputSource = inputModel.Source,
            IsMapped = inputModel.Source == HttpInputSource.FromRoute,
            MappedPath = inputModel.Source == HttpInputSource.FromRoute ? inputModel.Name : null,
        };
    }

    public static CqrsAzureFunctionParameterModel ForEventTrigger(CommandModel command)
    {
        return new CqrsAzureFunctionParameterModel
        {
            Id = command.Id,
            Name = command.Name.ToCamelCase(),
            TypeReference = command.InternalElement.AsTypeReference(),
            InputSource = null,
            IsMapped = false,
            MappedPath = null,
        };
    }

    public static CqrsAzureFunctionParameterModel ForEventTrigger(QueryModel query)
    {
        return new CqrsAzureFunctionParameterModel
        {
            Id = query.Id,
            Name = query.Name.ToCamelCase(),
            TypeReference = query.InternalElement.AsTypeReference(),
            InputSource = null,
            IsMapped = false,
            MappedPath = null,
        };
    }


    public string Id { get; init; }
    public string Name { get; init; }
    public ITypeReference TypeReference { get; init; }
    public ITypeReference Type { get; init; }
    public HttpInputSource? InputSource { get; init; }
    public bool IsMapped { get; init; }
    public string MappedPath { get; init; }
}