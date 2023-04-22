using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;

namespace Intent.Modules.AzureFunctions.Interop.Contracts.Templates;

public class ServiceOperationAzureFunctionModel : IAzureFunctionModel
{
    public ServiceOperationAzureFunctionModel(OperationModel operationModel)
    {
        Id = operationModel.Id;
        Name = operationModel.Name;
        TypeReference = operationModel.TypeReference;
        InternalElement = operationModel.InternalElement;
        TriggerType = GetTriggerType(operationModel.GetAzureFunction().Type());
        AuthorizationLevel = operationModel.GetAzureFunction().AuthorizationLevel().Value;
        Parameters = operationModel.Parameters
            .Select(x => new AzureFunctionParameterModel(x.InternalElement, x.InternalElement.SpecializationType))
            .ToList<IAzureFunctionParameterModel>();
        QueueName = operationModel.GetAzureFunction().QueueName();
        Connection = operationModel.GetAzureFunction().Connection();
        ReturnType = operationModel.TypeReference.Element != null ? operationModel.TypeReference : null;
        IsMapped = operationModel.InternalElement.IsMapped;
        Mapping = operationModel.InternalElement.MappedElement;
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

    private static TriggerType GetTriggerType(OperationModelStereotypeExtensions.AzureFunction.TypeOptions type)
    {
        return type.AsEnum() switch
        {
            OperationModelStereotypeExtensions.AzureFunction.TypeOptionsEnum.HttpTrigger => TriggerType.HttpTrigger,
            OperationModelStereotypeExtensions.AzureFunction.TypeOptionsEnum.ServiceBusTrigger => TriggerType.ServiceBusTrigger,
            OperationModelStereotypeExtensions.AzureFunction.TypeOptionsEnum.QueueTrigger => TriggerType.QueueTrigger,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}