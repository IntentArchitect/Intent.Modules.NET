using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Hangfire.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.Hangfire.Templates.HangfireConfiguration
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class HangfireConfigurationTemplateRegistration : SingleFileListModelTemplateRegistration<HangfireConfigurationModel>
    {
        public override string TemplateId => HangfireConfigurationTemplate.TemplateId;

        private readonly IMetadataManager _metadataManager;

        public HangfireConfigurationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<HangfireConfigurationModel> model)
        {
            return new HangfireConfigurationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<HangfireConfigurationModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetHangfireConfigurationModels()?.ToList();
        }
    }
}