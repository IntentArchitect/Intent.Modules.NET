using Intent.Modelers.Domain.Api;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Entities.Templates.DomainEnum
{
    [Description(DomainEnumTemplate.TemplateId)]
    public class DomainEnumTemplateRegistrations : FilePerModelTemplateRegistration<EnumModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DomainEnumTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DomainEnumTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget project, EnumModel model)
        {
            return new DomainEnumTemplate(model, project, _metadataManager);
        }

        public override IEnumerable<EnumModel> GetModels(Engine.IApplication application)
        {
            return _metadataManager.Domain(application).GetEnumModels();
        }
    }
}
