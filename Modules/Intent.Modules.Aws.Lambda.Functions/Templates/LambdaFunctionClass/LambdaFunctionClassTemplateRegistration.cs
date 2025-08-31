using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Templates.LambdaFunctionClass
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class LambdaFunctionClassTemplateRegistration : FilePerModelTemplateRegistration<ILambdaFunctionContainerModel>
    {
        private readonly IMetadataManager _metadataManager;

        public LambdaFunctionClassTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => LambdaFunctionClassTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ILambdaFunctionContainerModel model)
        {
            return new LambdaFunctionClassTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ILambdaFunctionContainerModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetServiceModels()
                .Where(serv => serv.Operations.Any(q => q.HasHttpSettings()))
                .Select(serv => new TraditionalServiceLambdaFunctionContainerModel(serv, application))
                .ToList();
        }
    }
}