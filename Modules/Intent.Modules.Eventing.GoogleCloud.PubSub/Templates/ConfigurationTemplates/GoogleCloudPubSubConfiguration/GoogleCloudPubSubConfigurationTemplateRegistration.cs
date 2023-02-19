using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ConfigurationTemplates.GoogleCloudPubSubConfiguration
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class GoogleCloudPubSubConfigurationTemplateRegistration : SingleFileListModelTemplateRegistration<EventingPackageModel>
    {
        private readonly IMetadataManager _metadataManager;

        public GoogleCloudPubSubConfigurationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public override string TemplateId => GoogleCloudPubSubConfigurationTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<EventingPackageModel> model)
        {
            return new GoogleCloudPubSubConfigurationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<EventingPackageModel> GetModels(IApplication application)
        {
            return _metadataManager.Eventing(application).GetEventingPackageModels().Where(Helper.HasMessages).ToList();
        }
    }
}