using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClient;
using Intent.Modules.Dapr.Shared;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientConfiguration;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClientConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Ignore)]
    public partial class HttpClientConfigurationTemplate : HttpClientConfigurationBase
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.ServiceInvocation.HttpClientConfigurationTemplate";
        private readonly IList<IServiceProxyModel> _typedModels;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientConfigurationTemplate(IOutputTarget outputTarget, IList<IServiceProxyModel> model)
            : base(TemplateId,
                  outputTarget,
                  model,
                  ServiceContractTemplate.TemplateId,
                  HttpClientTemplate.TemplateId,
                  (options, proxy, template) =>
                  {
                      options.AddStatement($"ApplyAppSettings(http, configuration, \"{((HttpClientConfigurationTemplate)template).GetGroupName(proxy)}\", \"{proxy.Name.ToPascalCase()}\");");
                  })
        {
            _typedModels = model;
        }

        private string GetGroupName(IServiceProxyModel serviceProxyModel)
        {
            return ExecutionContext.GetDaprApplicationName(serviceProxyModel.Endpoints[0].InternalElement.Package.ApplicationId);
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            var proxies = _typedModels.DistinctBy(x => x.Id);
            var groups = proxies.Select(GetGroupName).Distinct();
            foreach (var groupName in groups)
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(groupName, "Uri"), $"http://{groupName}/"));
            }
        }

        private static string GetConfigKey(string groupName, string key)
        {
            return $"HttpClients:{groupName}:{key.ToPascalCase()}";
        }
    }
}