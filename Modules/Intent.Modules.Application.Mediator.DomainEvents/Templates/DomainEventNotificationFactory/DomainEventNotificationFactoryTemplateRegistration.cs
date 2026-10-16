using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.DomainEvents.Templates.DomainEventNotificationFactory
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DomainEventNotificationFactoryTemplateRegistration : SingleFileListModelTemplateRegistration<DomainEventModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DomainEventNotificationFactoryTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DomainEventNotificationFactoryTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<DomainEventModel> model)
        {
            return new DomainEventNotificationFactoryTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<DomainEventModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetDomainEventModels();
        }
    }
}
