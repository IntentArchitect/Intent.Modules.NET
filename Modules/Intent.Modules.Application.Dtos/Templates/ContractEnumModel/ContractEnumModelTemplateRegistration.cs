using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Templates.ContractEnumModel
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ContractEnumModelTemplateRegistration : FilePerModelTemplateRegistration<EnumModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ContractEnumModelTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ContractEnumModelTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, EnumModel model)
        {
            return new ContractEnumModelTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<EnumModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetEnumModels()
                    .Where(e => e.InternalElement.Package.SpecializationTypeId != "df96d537-7bb5-4c49-811f-973fa6e95beb");//Eventing Pacakge
        }
    }
}