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

namespace Intent.Modules.Eventing.Contracts.Templates.EventBusInterface
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class EventBusInterfaceTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public EventBusInterfaceTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public string TemplateId => EventBusInterfaceTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var useLegacy = Settings.ModuleSettingsExtensions.GetEventingSettings(applicationManager.Settings).UseLegacyInterfaceName();
            if (useLegacy)
            {
                registry.RegisterTemplate(TemplateId, project => new EventBusInterfaceTemplate(project, null));
            }
        }
    }
}