using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.MultiTenancy.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates.Swagger.TenantHeaderOperationFilter
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class TenantHeaderOperationFilterTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public TenantHeaderOperationFilterTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public string TemplateId => TenantHeaderOperationFilterTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            if (applicationManager.InstalledModules.All(p => p.ModuleId != "Intent.AspNetCore.Swashbuckle")
                || applicationManager.Settings.GetMultitenancySettings()?.Strategy()?.IsHeader() != true)
            {
                return;
            }
            registry.RegisterTemplate(TemplateId, project => new TenantHeaderOperationFilterTemplate(project, null));
        }
    }
}