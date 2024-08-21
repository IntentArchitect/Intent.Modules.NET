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
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AspNetCore.SignalR.Templates.HubService
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class HubServiceTemplateRegistration : FilePerModelTemplateRegistration<SignalRHubModel>
    {
        private readonly IMetadataManager _metadataManager;

        public HubServiceTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => HubServiceTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, SignalRHubModel model)
        {
            return new HubServiceTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<SignalRHubModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetSignalRHubModels();
        }
    }
}