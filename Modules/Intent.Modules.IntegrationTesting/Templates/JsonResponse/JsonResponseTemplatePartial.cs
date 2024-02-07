using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Integration.HttpClients.Shared.Templates.JsonResponse;
using Intent.Modules.IntegrationTesting.Templates.HttpClient;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IntegrationTesting.Templates.JsonResponse
{
    [IntentManaged(Mode.Ignore)]
    public partial class JsonResponseTemplate : JsonResponseTemplateBase
    {
        public const string TemplateId = "Intent.IntegrationTesting.JsonResponse";

        public JsonResponseTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        protected override IDesigner GetSourceDesigner(IMetadataManager metadataManager, string applicationId)
        {
            return null;
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.FindTemplateInstances(HttpClientTemplate.TemplateId).Any();
        }
    }
}