using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.AzureFunctions.Dispatch.Services.Templates;
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
    public class ServiceOperationAzureFunctionClassTemplateRegistration : FilePerModelTemplateRegistration<IAzureFunctionModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceOperationAzureFunctionClassTemplateRegistration(IMetadataManager metadataManager)
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
}