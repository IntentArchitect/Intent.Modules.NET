using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Templates.DefaultDomainEventHandler
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DefaultDomainEventHandlerTemplateRegistration : FilePerModelTemplateRegistration<DomainEventModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DefaultDomainEventHandlerTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Fully)]
        public override string TemplateId => DefaultDomainEventHandlerTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, DomainEventModel model)
        {
            return new DefaultDomainEventHandlerTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<DomainEventModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetDomainEventModels()
                .Where(x => !x.AssociatedClasses().Any()) // only create default if no AggregateManagers are specified.
                .ToList();
        }
    }
}