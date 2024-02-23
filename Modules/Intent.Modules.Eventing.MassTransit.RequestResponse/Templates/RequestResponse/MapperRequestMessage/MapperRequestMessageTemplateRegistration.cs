using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Eventing.MassTransit.Templates.RequestResponse;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperRequestMessage
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class MapperRequestMessageTemplateRegistration : FilePerModelTemplateRegistration<HybridDtoModel>
    {
        private readonly IMetadataManager _metadataManager;

        public MapperRequestMessageTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => MapperRequestMessageTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, HybridDtoModel model)
        {
            return new MapperRequestMessageTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<HybridDtoModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application)
                .Elements
                .Where(element => HybridDtoModel.IsHybridDtoModel(element) && element.HasStereotype(Constants.MessageRequestEndpointStereotype))
                .Select(element => new HybridDtoModel(element));
        }
    }
}