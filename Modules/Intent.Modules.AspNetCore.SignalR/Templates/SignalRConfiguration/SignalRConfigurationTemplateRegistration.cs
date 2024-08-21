using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.SignalR.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileListModel", Version = "1.0")]

namespace Intent.Modules.AspNetCore.SignalR.Templates.SignalRConfiguration
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class SignalRConfigurationTemplateRegistration : SingleFileListModelTemplateRegistration<SignalRHubModel>
    {
        private readonly IMetadataManager _metadataManager;

        public SignalRConfigurationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }
        public override string TemplateId => SignalRConfigurationTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IList<SignalRHubModel> model)
        {
            return new SignalRConfigurationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IList<SignalRHubModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetSignalRHubModels().ToList();
        }
    }
}