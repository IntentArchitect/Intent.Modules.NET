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

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutFooter
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class RazorLayoutFooterTemplateRegistration : FilePerModelTemplateRegistration<LayoutFooterModel>
    {
        private readonly IMetadataManager _metadataManager;

        public RazorLayoutFooterTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RazorLayoutFooterTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, LayoutFooterModel model)
        {
            return new RazorLayoutFooterTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<LayoutFooterModel> GetModels(IApplication application)
        {
            return _metadataManager.UserInterface(application)
                .GetElementsOfType(LayoutFooterModel.SpecializationTypeId)
                .Select(e => e.AsLayoutFooterModel());
        }
    }
}