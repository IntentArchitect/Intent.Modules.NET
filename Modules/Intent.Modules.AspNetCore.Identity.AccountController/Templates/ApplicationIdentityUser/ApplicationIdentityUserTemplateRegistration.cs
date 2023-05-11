using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.Identity.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.Templates.ApplicationIdentityUser
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ApplicationIdentityUserTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public ApplicationIdentityUserTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public string TemplateId => ApplicationIdentityUserTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            if (_metadataManager.Domain(applicationManager).GetClassModels().Any(p => p.HasIdentityUser()))
            {
                return;
            }
            registry.RegisterTemplate(TemplateId, project => new ApplicationIdentityUserTemplate(project, null));
        }
    }
}