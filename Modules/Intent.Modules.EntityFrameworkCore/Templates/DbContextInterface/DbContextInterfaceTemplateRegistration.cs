using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.DbContextInterface
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DbContextInterfaceTemplateRegistration : FilePerModelTemplateRegistration<DbContextInstance>
    {
        private readonly IMetadataManager _metadataManager;

        public DbContextInterfaceTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DbContextInterfaceTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, DbContextInstance model)
        {
            return new DbContextInterfaceTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<DbContextInstance> GetModels(IApplication application)
        {
            return DbContextManager.GetDbContexts(application.Id, application.MetadataManager);
        }
    }
}