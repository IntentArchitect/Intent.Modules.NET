using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class EntityTypeConfigurationTemplateRegistration : FilePerModelTemplateRegistration<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public EntityTypeConfigurationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => EntityTypeConfigurationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            return new EntityTypeConfigurationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels();
        }
    }
}