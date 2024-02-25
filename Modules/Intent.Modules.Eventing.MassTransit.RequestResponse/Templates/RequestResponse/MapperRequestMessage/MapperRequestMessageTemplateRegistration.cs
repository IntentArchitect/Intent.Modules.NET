using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.ServiceProxies.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
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
                .Concat(_metadataManager.ServiceProxies(application)
                    .Elements
                    .Where(p => p.MappedElement is not null)
                    .Select(s => s.MappedElement.Element)
                    .Cast<IElement>())
                .Where(element => HybridDtoModel.IsHybridDtoModel(element) && element.HasStereotype(Constants.MessageTriggered))
                .DistinctBy(k => k.Id)
                .Select(element => new HybridDtoModel(element));
        }
    }
}