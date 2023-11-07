using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Dispatch.MediatR.ImplicitControllers
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ImplicitControllerTemplateRegistration : FilePerModelTemplateRegistration<IControllerModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ImplicitControllerTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ControllerTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IControllerModel model)
        {
            return new ControllerTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IControllerModel> GetModels(IApplication application)
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
                .Select(grouping => new CqrsControllerModel(grouping.Key, grouping))
                .ToArray<IControllerModel>();
        }
    }
}