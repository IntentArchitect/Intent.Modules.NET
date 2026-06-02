using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Fakes.Templates.ResponseDtoFactory
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ResponseDtoFactoryTemplateRegistration : FilePerModelTemplateRegistration<ResponseDtoFactoryModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ResponseDtoFactoryTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ResponseDtoFactoryTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ResponseDtoFactoryModel model)
        {
            return new ResponseDtoFactoryTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IEnumerable<ResponseDtoFactoryModel> GetModels(IApplication application)
        {
            return ResponseDtoFactoryModelProvider.GetModels(_metadataManager, application);
        }
    }
}
