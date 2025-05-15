using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.IaC.Terraform.Templates.AzureFunctions.AzureFunctionAppTf
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
            
            var arr = applicationManager.MetadataManager.GetSolutionMetadata<IElement>("Services").ToArray();
            
            //registry.RegisterTemplate(TemplateId, project => new AzureFunctionAppTfTemplate(project, null));
        }
    }
}