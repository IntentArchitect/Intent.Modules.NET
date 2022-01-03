using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContextInterface
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DbContextInterfaceTemplateRegistration : SingleFileListModelTemplateRegistration<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DbContextInterfaceTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DbContextInterfaceTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<ClassModel> models)
        {
            return new DbContextInterfaceTemplate(outputTarget, models);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<ClassModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels().ToList();
        }
    }
}
