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

namespace Intent.Modules.CosmosDb.Templates.Integration.UnitOfWorkBehaviour
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class UnitOfWorkBehaviourTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public UnitOfWorkBehaviourTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => UnitOfWorkBehaviourTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            if (!IntegrationCoordinator.ShouldInstallMediatRIntegration(applicationManager))
            {
                return;
            }

            registry.RegisterTemplate(TemplateId, project => new UnitOfWorkBehaviourTemplate(project, null));
        }
    }
}