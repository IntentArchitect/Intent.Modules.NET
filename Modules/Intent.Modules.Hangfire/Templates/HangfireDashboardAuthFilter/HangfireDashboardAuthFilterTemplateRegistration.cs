using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Hangfire.Api;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.VisualStudio.Projects.Api.CSharpProjectNETModelStereotypeExtensions.NETSettings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.Hangfire.Templates.HangfireDashboardAuthFilter
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class HangfireDashboardAuthFilterTemplateRegistration : SingleFileListModelTemplateRegistration<HangfireConfigurationModel>
    {
        private readonly IMetadataManager _metadataManager;

        public HangfireDashboardAuthFilterTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public override string TemplateId => HangfireDashboardAuthFilterTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<HangfireConfigurationModel> model)
        {
            return new HangfireDashboardAuthFilterTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<HangfireConfigurationModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetHangfireConfigurationModels().ToList();
        }
    }
}