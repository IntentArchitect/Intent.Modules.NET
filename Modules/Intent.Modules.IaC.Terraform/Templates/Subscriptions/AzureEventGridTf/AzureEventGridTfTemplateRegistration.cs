using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Integration.IaC.Shared;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.Subscriptions.AzureEventGridTf
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AzureEventGridTfTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public AzureEventGridTfTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public string TemplateId => AzureEventGridTfTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var apps = applicationManager.GetSolutionConfig()
                .GetApplicationReferences()
                .Select(s => applicationManager.GetSolutionConfig().GetApplicationConfig(s.Id))
                .Where(p => p.Modules.Any(x => x.ModuleId == "Intent.AzureFunctions"))
                .ToArray();
            foreach (var app in apps)
            {
                if (!IntegrationManager.Instance.GetAggregatedAzureEventGridSubscriptions(app.Id).Any())
                {
                    continue;
                }
                registry.RegisterTemplate(TemplateId, project => new AzureEventGridTfTemplate(project, new ApplicationInfo(app)));
            }
        }
    }
}