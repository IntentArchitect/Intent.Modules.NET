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

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutSider
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class RazorLayoutSiderTemplateRegistration : FilePerModelTemplateRegistration<LayoutSiderModel>
    {
        private readonly IMetadataManager _metadataManager;

        public RazorLayoutSiderTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RazorLayoutSiderTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, LayoutSiderModel model)
        {
            return new RazorLayoutSiderTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<LayoutSiderModel> GetModels(IApplication application)
        {
            return _metadataManager.UserInterface(application)
                .GetElementsOfType(LayoutSiderModel.SpecializationTypeId)
                .Select(e => e.AsLayoutSiderModel());
        }
    }
}