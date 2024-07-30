using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorLayoutCodeBehind
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class RazorLayoutCodeBehindTemplateRegistration : FilePerModelTemplateRegistration<LayoutModel>
    {
        private readonly IMetadataManager _metadataManager;

        public RazorLayoutCodeBehindTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => RazorLayoutCodeBehindTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, LayoutModel model)
        {
            return new RazorLayoutCodeBehindTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<LayoutModel> GetModels(IApplication application)
        {
            if (!application.Settings.GetBlazor().UseCodeBehindFilesForComponents())
            {
                return [];
            }

            return _metadataManager.UserInterface(application).GetLayoutModels();
        }
    }
}