using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.ServiceProxies.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperResponseMessage
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class MapperResponseMessageTemplateRegistration : FilePerModelTemplateRegistration<DTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public MapperResponseMessageTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => MapperResponseMessageTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, DTOModel model)
        {
            return new MapperResponseMessageTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application)
                .Elements
                .Concat(_metadataManager.ServiceProxies(application)
                    .Elements
                    .Where(p => p.MappedElement is not null)
                    .Select(s => s.MappedElement.Element)
                    .Cast<IElement>())
                .Where(element => HybridDtoModel.IsHybridDtoModel(element) &&
                                  element.HasStereotype(Constants.MessageTriggered) &&
                                  element.TypeReference?.Element is not null &&
                                  element.TypeReference.Element.SpecializationType == DTOModel.SpecializationType)
                .DistinctBy(k => k.TypeReference.Element.Id)
                .Select(element => new DTOModel((IElement)element.TypeReference.Element));
        }
    }
}