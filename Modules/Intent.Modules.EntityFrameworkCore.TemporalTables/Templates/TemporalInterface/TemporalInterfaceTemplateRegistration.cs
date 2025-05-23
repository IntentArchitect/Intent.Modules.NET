using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.TemporalTables.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.TemporalTables.Templates.TemporalInterface
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class TemporalInterfaceTemplateRegistration : SingleFileTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public override string TemplateId => TemporalInterfaceTemplate.TemplateId;

        public TemporalInterfaceTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new TemporalInterfaceTemplate(outputTarget);
        }

        public override void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var temporalModels = _metadataManager.Domain(application).GetClassModels().Where(c => c.HasTemporalTable());

            if (temporalModels.Any())
            {
                registry.RegisterTemplate(TemplateId, project => new TemporalInterfaceTemplate(project));
            }
        }
    }
}