using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.IaC.Terraform.Templates.Applications.AzureFunctionAppTf;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.Applications.AzureFunctionAppTf
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AzureFunctionAppTfTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public AzureFunctionAppTfTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public string TemplateId => AzureFunctionAppTfTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var apps = applicationManager.GetSolutionConfig()
                .GetApplicationReferences()
                .Select(s => applicationManager.GetSolutionConfig().GetApplicationConfig(s.Id))
                .Where(p => p.Modules.Any(x => x.ModuleId == "Intent.AzureFunctions"))
                .ToArray();
            foreach (var applicationConfig in apps)
            {
                registry.RegisterTemplate(TemplateId, project => new AzureFunctionAppTfTemplate(project, new ApplicationInfo(applicationConfig)));
            }
        }
    }
}