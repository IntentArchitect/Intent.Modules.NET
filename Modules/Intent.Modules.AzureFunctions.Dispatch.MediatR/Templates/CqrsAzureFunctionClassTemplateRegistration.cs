using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Dispatch.MediatR.Templates
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CqrsAzureFunctionClassTemplateRegistration : FilePerModelTemplateRegistration<IAzureFunctionModel>
    {
        private readonly IMetadataManager _metadataManager;

        public CqrsAzureFunctionClassTemplateRegistration(IMetadataManager metadataManager)
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
            return _metadataManager.Services(application).GetCommandModels()
                .Where(x => x.HasAzureFunction())
                .Select(x => new CqrsRequestAzureFunctionModel(x))
                .Concat(_metadataManager.Services(application).GetQueryModels()
                    .Where(x => x.HasAzureFunction())
                    .Select(x => new CqrsRequestAzureFunctionModel(x)))
                .ToList();
        }
    }
}