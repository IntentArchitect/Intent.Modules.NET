using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AzureFunctions.Dispatch.MediatR.Templates;

public class CqrsAzureFunctionModel : IAzureFunctionModel
{
    public CqrsAzureFunctionModel(CommandModel command)
    {
        Id = command.Id;
        Name = command.Name.RemoveSuffix("Command");
        TypeReference = command.TypeReference;
        InternalElement = command.InternalElement;
        TriggerType = Enum.TryParse<TriggerType>(command.GetAzureFunction().Type().AsEnum().ToString(), out var triggerType)
            ? triggerType
            : throw new Exception($"Unable to determine Azure Function -> Trigger Type value for {command.Name}");
        AuthorizationLevel = command.GetAzureFunction().AuthorizationLevel().Value;
        Parameters = command.GetAzureFunction().Type().IsHttpTrigger()
            ? HttpEndpointModelFactory.GetEndpoint(command.InternalElement)!
                .Inputs.Select(CqrsAzureFunctionParameterModel.ForHttpTrigger)
                .ToList<IAzureFunctionParameterModel>()
            : new List<IAzureFunctionParameterModel>() { CqrsAzureFunctionParameterModel.ForEventTrigger(command) };
        QueueName = command.GetAzureFunction().QueueName();
        Connection = command.GetAzureFunction().Connection();
        ReturnType = command.TypeReference.Element != null ? command.TypeReference : null;
        IsMapped = command.InternalElement.IsMapped;
        Mapping = command.InternalElement.MappedElement;
    }

    public CqrsAzureFunctionModel(QueryModel query)
    {
        Id = query.Id;
        Name = query.Name.RemoveSuffix("Query");
        TypeReference = query.TypeReference;
        InternalElement = query.InternalElement;
        TriggerType = Enum.TryParse<TriggerType>(query.GetAzureFunction().Type().AsEnum().ToString(), out var triggerType)
            ? triggerType
            : throw new Exception($"Unable to determine Azure Function -> Trigger Type value for {query.Name}");
        AuthorizationLevel = query.GetAzureFunction().AuthorizationLevel().Value;
        Parameters = query.GetAzureFunction().Type().IsHttpTrigger()
            ? HttpEndpointModelFactory.GetEndpoint(query.InternalElement)!
                .Inputs.Select(CqrsAzureFunctionParameterModel.ForHttpTrigger)
                .ToList<IAzureFunctionParameterModel>()
            : new List<IAzureFunctionParameterModel>() { CqrsAzureFunctionParameterModel.ForEventTrigger(query) };
        QueueName = query.GetAzureFunction().QueueName();
        Connection = query.GetAzureFunction().Connection();
        ReturnType = query.TypeReference.Element != null ? query.TypeReference : null;
        IsMapped = query.InternalElement.IsMapped;
        Mapping = query.InternalElement.MappedElement;
    }

    public string Id { get; }

    public string Name { get; }

    public ITypeReference TypeReference { get; }

    public IElement InternalElement { get; }

    public TriggerType TriggerType { get; }

    public string AuthorizationLevel { get; }

    public IList<IAzureFunctionParameterModel> Parameters { get; }

    public string QueueName { get; }

    public string Connection { get; }

    public ITypeReference ReturnType { get; }

    public bool IsMapped { get; }

    public IElementMapping Mapping { get; }
}