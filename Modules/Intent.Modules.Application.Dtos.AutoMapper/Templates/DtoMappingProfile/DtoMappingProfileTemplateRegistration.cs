using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.Templates.DtoMappingProfile
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class DtoMappingProfileTemplateRegistration : FilePerModelTemplateRegistration<DTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DtoMappingProfileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DtoMappingProfileTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, DTOModel model)
        {
            return new DtoMappingProfileTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            if (application.GetSettings().GetAutoMapperSettings().ProfileLocation().IsProfileSeparate())
            {
                return _metadataManager.Services(application).GetDTOModels()
                    .Where(x => x.HasMapFromDomainMapping())
                    .ToList();

            }
            return new List<DTOModel>();
        }
    }
}