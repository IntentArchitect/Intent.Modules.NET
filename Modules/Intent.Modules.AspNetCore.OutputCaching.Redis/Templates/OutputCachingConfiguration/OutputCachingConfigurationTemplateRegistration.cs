using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.OutputCaching.Redis.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OutputCaching.Redis.Templates.OutputCachingConfiguration
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class OutputCachingConfigurationTemplateRegistration : SingleFileTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public override string TemplateId => OutputCachingConfigurationTemplate.TemplateId;

        public OutputCachingConfigurationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        [IntentManaged(Mode.Ignore)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            var cachingPolicies = _metadataManager.Services(outputTarget.ExecutionContext.GetApplicationConfig().Id).GetCachingPolicyModels();
            return new OutputCachingConfigurationTemplate(outputTarget, cachingPolicies);
        }
    }
}