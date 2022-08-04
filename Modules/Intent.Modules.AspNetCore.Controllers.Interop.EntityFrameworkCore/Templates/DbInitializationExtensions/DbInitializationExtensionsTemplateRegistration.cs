using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Interop.EntityFrameworkCore.Templates.DbInitializationExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DbInitializationExtensionsTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public DbInitializationExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => DbInitializationExtensionsTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            if (!applicationManager.Settings.GetDatabaseSettings().DatabaseProvider().IsInMemory())
            {
                registry.RegisterTemplate(TemplateId, project => new DbInitializationExtensionsTemplate(project, null));
            }
        }
    }
}