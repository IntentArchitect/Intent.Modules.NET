using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

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
        public override string TemplateId => DbContextInterfaceTemplate.TemplateId;

        // This is a bit of a hack, and is an indicator that this template should not exist in this module:
        public override void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            if (application.InstalledModules.Any(x => x.ModuleId == "Intent.Entities.Repositories.Api"))
            {
                return;
            }
            base.DoRegistration(registry, application);
        }

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<ClassModel> model)
        {
            return new DbContextInterfaceTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<ClassModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels().ToList();
        }
    }
}