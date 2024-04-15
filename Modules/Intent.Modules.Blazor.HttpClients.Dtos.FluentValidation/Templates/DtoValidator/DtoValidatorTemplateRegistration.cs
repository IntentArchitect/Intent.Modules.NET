using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.WebClient.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Modules.Modelers.Types.ServiceProxies;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Dtos.FluentValidation.Templates.DtoValidator
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DtoValidatorTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;
        private const string CommandTypeId = "ccf14eb6-3a55-4d81-b5b9-d27311c70cb9";
        private const string QueryTypeId = "e71b0662-e29d-4db2-868b-8a12464b25d0";

        public DtoValidatorTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => DtoValidatorTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            var models = _metadataManager.WebClient(applicationManager).GetMappedServiceProxyInboundDTOModels()
                .Where(x =>
                {
                    if (x.InternalElement.IsCommandModel() || x.InternalElement.IsQueryModel())
                    {
                        return HttpEndpointModelFactory.GetEndpoint(x.InternalElement)?.Inputs.Any(i => i.Id == x.Id) == true;
                    }

                    return true;
                })
                .ToList();

            foreach (var model in models)
            {
                var advancedMappingSource = model.InternalElement.AssociatedElements.Count() switch
                {
                    0 => null,
                    1 => model.InternalElement,
                    _ => null,
                };

                registry.RegisterTemplate(TemplateId,
                    project => new DtoValidatorTemplate(
                        project,
                        model,
                        advancedMappingSource?.AssociatedElements));
            }
        }
    }
}