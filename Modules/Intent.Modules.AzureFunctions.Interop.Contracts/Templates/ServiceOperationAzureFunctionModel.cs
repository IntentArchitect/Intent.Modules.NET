using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.AzureFunctions.Interop.Contracts.Templates;

public class ServiceOperationAzureFunctionModel : IAzureFunctionModel
{
    public ServiceOperationAzureFunctionModel(OperationModel operationModel)
    {
        Id = operationModel.Id;
        Name = operationModel.Name;
        TypeReference = operationModel.TypeReference;
        InternalElement = operationModel.InternalElement;
        TriggerType = Enum.TryParse<TriggerType>(operationModel.GetAzureFunction().Trigger().AsEnum().ToString(), out var triggerType)
            ? triggerType
            : throw new Exception($"Unable to determine Azure Function -> Trigger Type value for {operationModel.Name}"); ;
        AuthorizationLevel = operationModel.GetAzureFunction().AuthorizationLevel().Value;
        Parameters = operationModel.Parameters
            .Select(x => new AzureFunctionParameterModel(x.InternalElement, x.InternalElement.SpecializationType))
            .ToList<IAzureFunctionParameterModel>();
        QueueName = operationModel.GetAzureFunction().QueueName();
        IncludeMessageEnvelope = operationModel.GetAzureFunction().IncludeMessageEnvelope();
        Connection = operationModel.GetAzureFunction().Connection();
        ScheduleExpression = operationModel.GetAzureFunction().ScheduleExpression();
        EventHubName = operationModel.GetAzureFunction().EventHubName();
        ReturnType = operationModel.TypeReference.Element != null ? operationModel.TypeReference : null;
        IsMapped = operationModel.InternalElement.IsMapped;
        Mapping = operationModel.InternalElement.MappedElement;
        Folder = operationModel.ParentService.Folder;
    }

    public string Id { get; }

    public string Name { get; }

    public ITypeReference TypeReference { get; }

    public IElement InternalElement { get; }

    public TriggerType TriggerType { get; }

    public string AuthorizationLevel { get; }

    public IList<IAzureFunctionParameterModel> Parameters { get; }

    public string QueueName { get; }
    public bool IncludeMessageEnvelope { get; }
    public string Connection { get; }
    public string ScheduleExpression { get; }
    public string EventHubName { get; }

    public ITypeReference ReturnType { get; }

    public bool IsMapped { get; }

    public IElementMapping Mapping { get; }
    public IFolder Folder { get; }
}