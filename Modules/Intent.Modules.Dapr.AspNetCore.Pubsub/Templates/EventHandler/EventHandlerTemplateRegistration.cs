using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Modelers.Eventing;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.EventHandler
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class EventHandlerTemplateRegistration : FilePerModelTemplateRegistration<MessageModel>
    {
        private readonly IMetadataManager _metadataManager;

        public EventHandlerTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => EventHandlerTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, MessageModel model)
        {
            return new EventHandlerTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<MessageModel> GetModels(IApplication application)
        {
            return _metadataManager.GetSubscribedToMessageModels(application);
        }
    }
}