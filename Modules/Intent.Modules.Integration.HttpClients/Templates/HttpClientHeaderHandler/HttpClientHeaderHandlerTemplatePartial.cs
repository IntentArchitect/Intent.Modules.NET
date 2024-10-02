using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Settings;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientHeaderDelegatingHandler;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.HttpClientHeaderHandler
{
    [IntentManaged(Mode.Ignore)]
    public partial class HttpClientHeaderHandlerTemplate : HttpClientHeaderHandlerTemplateBase
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Integration.HttpClients.HttpClientHeaderHandlerTemplate";

        public HttpClientHeaderHandlerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && ExecutionContext.Settings.GetIntegrationHttpClientSettings().AuthorizationSetup().IsTransmittableAccessToken();
        }

    }
}