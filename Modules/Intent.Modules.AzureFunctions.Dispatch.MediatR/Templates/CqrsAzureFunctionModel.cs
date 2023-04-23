using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
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
        TriggerType = Enum.TryParse<TriggerType>(command.GetAzureFunction().Trigger().AsEnum().ToString(), out var triggerType)
            ? triggerType
            : throw new Exception($"Unable to determine Azure Function -> Trigger Type value for {command.Name}");
        AuthorizationLevel = command.GetAzureFunction().AuthorizationLevel().Value;
        Parameters = command.GetAzureFunction().Trigger().IsHttpTrigger()
            ? HttpEndpointModelFactory.GetEndpoint(command.InternalElement)!
                .Inputs.Select(CqrsAzureFunctionParameterModel.ForHttpTrigger)
                .ToList<IAzureFunctionParameterModel>()
            : new List<IAzureFunctionParameterModel>() { CqrsAzureFunctionParameterModel.ForEventTrigger(command) };
        QueueName = command.GetAzureFunction().QueueName();
        Connection = command.GetAzureFunction().Connection();
        ScheduleExpression = command.GetAzureFunction().ScheduleExpression();
        EventHubName = command.GetAzureFunction().EventHubName();
        ReturnType = command.TypeReference.Element != null ? command.TypeReference : null;
        IsMapped = command.InternalElement.IsMapped;
        Mapping = command.InternalElement.MappedElement;
        Folder = command.Folder;
    }

    public CqrsAzureFunctionModel(QueryModel query)
    {
        Id = query.Id;
        Name = query.Name.RemoveSuffix("Query");
        TypeReference = query.TypeReference;
        InternalElement = query.InternalElement;
        TriggerType = Enum.TryParse<TriggerType>(query.GetAzureFunction().Trigger().AsEnum().ToString(), out var triggerType)
            ? triggerType
            : throw new Exception($"Unable to determine Azure Function -> Trigger Type value for {query.Name}");
        AuthorizationLevel = query.GetAzureFunction().AuthorizationLevel().Value;
        Parameters = query.GetAzureFunction().Trigger().IsHttpTrigger()
            ? HttpEndpointModelFactory.GetEndpoint(query.InternalElement)!
                .Inputs.Select(CqrsAzureFunctionParameterModel.ForHttpTrigger)
                .ToList<IAzureFunctionParameterModel>()
            : new List<IAzureFunctionParameterModel>() { CqrsAzureFunctionParameterModel.ForEventTrigger(query) };
        QueueName = query.GetAzureFunction().QueueName();
        Connection = query.GetAzureFunction().Connection();
        ScheduleExpression = query.GetAzureFunction().ScheduleExpression();
        EventHubName = query.GetAzureFunction().EventHubName();
        ReturnType = query.TypeReference.Element != null ? query.TypeReference : null;
        IsMapped = query.InternalElement.IsMapped;
        Mapping = query.InternalElement.MappedElement;
        Folder = query.Folder;
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
    public string ScheduleExpression { get; }
    public string EventHubName { get; }

    public ITypeReference ReturnType { get; }

    public bool IsMapped { get; }

    public IElementMapping Mapping { get; }
    public IFolder Folder { get; }
}