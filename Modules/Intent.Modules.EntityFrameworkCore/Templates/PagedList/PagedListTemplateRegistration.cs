using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Constants;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.PagedList
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class PagedListTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public PagedListTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => PagedListTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            // This bypasses the cached lookup
            var templates = applicationManager.FindTemplateInstances(TemplateRoles.Application.Common.PagedList, _ => true).ToArray();
            if (templates.All(p => p.Id == TemplateId)) // GCB - what is this for? Ensures that there is only one?
            {
                registry.RegisterTemplate(TemplateId, project => new PagedListTemplate(project, null));
            }
        }
    }
}
