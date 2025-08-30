using System;
using System.Collections;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.JsonResponse;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.JsonResponse
{
    [IntentManaged(Mode.Ignore)]
    public class JsonResponseTemplate : JsonResponseTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Integration.HttpClients.JsonResponse";

        public JsonResponseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        protected override IEnumerable<IServiceProxyModel> GetServiceContractModels(IMetadataManager metadataManager, string applicationId)
        {
            const string serviceProxiesDesignerId = "2799aa83-e256-46fe-9589-b96f7d6b09f7";
            return metadataManager.GetServiceProxyModels(
                applicationId,
                applicationId => metadataManager.GetDesigner(applicationId, serviceProxiesDesignerId), // for backward compatibility
                metadataManager.Services);
        }
    }
}