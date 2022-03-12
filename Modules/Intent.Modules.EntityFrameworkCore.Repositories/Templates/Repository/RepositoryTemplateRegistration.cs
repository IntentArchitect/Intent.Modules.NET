using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Intent.SoftwareFactory;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class RepositoryTemplateRegistration : FilePerModelTemplateRegistration<ClassModel>, ISupportsConfiguration
    {
        private readonly IMetadataManager _metadataManager;
        private IEnumerable<string> _stereotypeNames;

        public RepositoryTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RepositoryTemplate.TemplateId;

        public void Configure(IDictionary<string, string> settings)
        {
            var createOnStereotypeValues = settings["Create On Stereotype"];
            _stereotypeNames = createOnStereotypeValues.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            return new RepositoryTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            var allModels = _metadataManager.Domain(application).GetClassModels();
            var filteredModels = allModels.Where(p => _stereotypeNames.Any(p.HasStereotype));

            if (!filteredModels.Any())
            {
                return allModels;
            }

            return filteredModels;
        }
    }
}
