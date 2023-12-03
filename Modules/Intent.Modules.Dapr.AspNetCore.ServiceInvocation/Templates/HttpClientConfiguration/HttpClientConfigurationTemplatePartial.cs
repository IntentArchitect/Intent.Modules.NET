using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClient;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientConfiguration;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientHeaderDelegatingHandler;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using JetBrains.Annotations;
using static System.Net.WebRequestMethods;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClientConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HttpClientConfigurationTemplate : HttpClientConfigurationBase
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.ServiceInvocation.HttpClientConfigurationTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientConfigurationTemplate(IOutputTarget outputTarget, IList<ServiceProxyModel> model)
            : base(TemplateId,
                  outputTarget,
                  model,
                  ServiceContractTemplate.TemplateId,
                  HttpClientTemplate.TemplateId,
                  (options, proxy, template) =>
                  {
                      options.AddStatement($"http.BaseAddress = new Uri($\"http://{template.ExecutionContext.GetDaprApplicationName(model.First().InternalElement.MappedElement.ApplicationId)}\");");
                  }
                  )
        { 
        }
    }
}