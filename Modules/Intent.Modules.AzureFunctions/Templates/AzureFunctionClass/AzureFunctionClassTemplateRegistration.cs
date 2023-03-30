using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AzureFunctionClassTemplateRegistration : FilePerModelTemplateRegistration<AzureFunctionModel>
    {
        private readonly IMetadataManager _metadataManager;

        public AzureFunctionClassTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => AzureFunctionClassTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, AzureFunctionModel model)
        {
            return new AzureFunctionClassTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<AzureFunctionModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetAzureFunctionModels();
        }
    }
}