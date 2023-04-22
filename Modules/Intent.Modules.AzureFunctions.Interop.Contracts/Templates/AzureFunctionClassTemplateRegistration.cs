using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Interop.Contracts.Templates
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AzureFunctionClassTemplateRegistration : FilePerModelTemplateRegistration<IAzureFunctionModel>
    {
        private readonly IMetadataManager _metadataManager;

        public AzureFunctionClassTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => AzureFunctionClassTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IAzureFunctionModel model)
        {
            return new AzureFunctionClassTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IAzureFunctionModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetServiceModels()
                .SelectMany(x => x.Operations)
                .Where(x => x.HasAzureFunction())
                .Select(x => new ServiceOperationAzureFunctionModel(x))
                .ToList();
        }
    }

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
}