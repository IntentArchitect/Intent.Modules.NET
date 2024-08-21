using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Eventing.Contracts.DomainMapping.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.DomainMapping.Templates.DtoExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DtoExtensionsTemplateRegistration : FilePerModelTemplateRegistration<EventingDTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DtoExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DtoExtensionsTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, EventingDTOModel model)
        {
            return new DtoExtensionsTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<EventingDTOModel> GetModels(IApplication application)
        {
            var applicationModel = _metadataManager.Eventing(application).GetApplicationModels().SingleOrDefault();
            if (applicationModel == null)
            {
                return Enumerable.Empty<EventingDTOModel>();
            }

            var eventingDtoModels = applicationModel
                .PublishedMessages()
                .SelectMany(x => GetEventingDtoModels((IElement)x.Element))
                .Where(x => x.InternalElement.IsMapped)
                .Distinct()
                .ToList();

            return eventingDtoModels;

            static IEnumerable<EventingDTOModel> GetEventingDtoModels(IElement element)
            {
                if (element.TypeReference?.Element?.IsEventingDTOModel() == true)
                {
                    var model = element.TypeReference.Element.AsEventingDTOModel();

                    yield return model;

                    foreach (var childModel in model.InternalElement.ChildElements.SelectMany(GetEventingDtoModels))
                    {
                        yield return childModel;
                    }
                }

                foreach (var childModel in element.ChildElements.SelectMany(GetEventingDtoModels))
                {
                    yield return childModel;
                }
            }
        }
    }
}