using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.Templates.CursorPagedResultMappingExtensions
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CursorPagedResultMappingExtensionsTemplateRegistration : SingleFileListModelTemplateRegistration<DTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public CursorPagedResultMappingExtensionsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CursorPagedResultMappingExtensionsTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<DTOModel> model)
        {
            return new CursorPagedResultMappingExtensionsTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<DTOModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetDTOModels()
                .Where(x => x.HasMapFromDomainMapping())
                .ToList();
        }
    }
}