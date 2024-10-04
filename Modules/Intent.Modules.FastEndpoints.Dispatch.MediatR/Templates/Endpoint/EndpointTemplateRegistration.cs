using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Registrations;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Dispatch.MediatR.Templates.Endpoint
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
            var elementsGroupedByParent = Enumerable.Empty<IElement>()
                .Concat(_metadataManager.Services(application).GetCommandModels()
                    .Where(x => x.HasHttpSettings())
                    .Select(x => x.InternalElement))
                .Concat(_metadataManager.Services(application).GetQueryModels()
                    .Where(x => x.HasHttpSettings())
                    .Select(x => x.InternalElement))
                .GroupBy(x => x.ParentElement);

            return elementsGroupedByParent
                .Select(grouping => new MediatREndpointContainerModel(grouping.Key, grouping))
                .SelectMany(container => container.Endpoints)
                .ToArray();
        }
    }
}