using System.Collections.Generic;
using System.ComponentModel;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntityState
{
    [Description(DomainEntityStateTemplate.TemplateId)]
    public class DomainEntityStateTemplateRegistrations : FilePerModelTemplateRegistration<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public DomainEntityStateTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => DomainEntityStateTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            return new DomainEntityStateTemplate(model, outputTarget);
        }

        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels();
        }
    }
}
