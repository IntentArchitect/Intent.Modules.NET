using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.SoftwareFactory;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.Entities.Templates.DomainEnum
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DomainEnumTemplateRegistration : FilePerModelTemplateRegistration<EnumModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DomainEnumTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DomainEnumTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, EnumModel model)
        {
            return new DomainEnumTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<EnumModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetEnumModels();
        }
    }
}
