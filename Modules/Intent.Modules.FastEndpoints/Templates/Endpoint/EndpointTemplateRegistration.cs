using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Templates.Endpoint
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class EndpointTemplateRegistration : FilePerModelTemplateRegistration<IEndpointModel>
    {
        private readonly IMetadataManager _metadataManager;

        public EndpointTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => EndpointTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IEndpointModel model)
        {
            return new EndpointTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IEndpointModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetServiceModels()
                .Where(p => p.Operations.Any(q => q.HasHttpSettings()))
                .SelectMany(service => service.Operations.Select(operation => new { Service = service, Operation = operation }))
                .Select(x => new ServiceEndpointModel(x.Service, x.Operation))
                .ToArray();
        }
    }
}