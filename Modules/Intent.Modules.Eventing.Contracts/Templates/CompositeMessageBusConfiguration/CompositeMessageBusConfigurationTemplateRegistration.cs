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

namespace Intent.Modules.Eventing.Contracts.Templates.CompositeMessageBusConfiguration
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CompositeMessageBusConfigurationTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public CompositeMessageBusConfigurationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public string TemplateId => CompositeMessageBusConfigurationTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            if (applicationManager.RequiresCompositeMessageBus())
            {
                registry.RegisterTemplate(TemplateId, project => new CompositeMessageBusConfigurationTemplate(project, null));
            }
        }
    }
}
