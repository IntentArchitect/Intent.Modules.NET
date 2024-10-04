using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.ModelDefinition
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ModelDefinitionTemplateRegistration : FilePerModelTemplateRegistration<ModelDefinitionModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ModelDefinitionTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ModelDefinitionTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ModelDefinitionModel model)
        {
            return new ModelDefinitionTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ModelDefinitionModel> GetModels(IApplication application)
        {
            return _metadataManager.UserInterface(application).GetModelDefinitionModels()
                .Where(x => !x.InternalElement.ParentElement.IsComponentModel())
                .ToList();
        }
    }
}