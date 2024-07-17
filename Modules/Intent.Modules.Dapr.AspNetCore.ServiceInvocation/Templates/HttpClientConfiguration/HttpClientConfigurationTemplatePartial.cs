using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClient;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;
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
    [IntentManaged(Mode.Fully, Body = Mode.Ignore, Signature = Mode.Ignore)]
    public partial class HttpClientConfigurationTemplate : HttpClientConfigurationBase
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.ServiceInvocation.HttpClientConfigurationTemplate";
        private readonly IList<ServiceProxyModel> _typedModels;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientConfigurationTemplate(IOutputTarget outputTarget, IList<ServiceProxyModel> model)
            : base(TemplateId,
                  outputTarget,
                  model,
                  ServiceContractTemplate.TemplateId,
                  HttpClientTemplate.TemplateId,
                  (options, proxy, template) =>
                  {
                      options.AddStatement($"ApplyAppSettings(http, configuration, \"{((HttpClientConfigurationTemplate)template).GetGroupName(proxy)}\", \"{proxy.Name.ToPascalCase()}\");");
                  }
                  )
        {
            _typedModels = model;
        }


        private string GetGroupName(IServiceProxyModel proxyInterface)
        {
            var proxy = proxyInterface as ServiceProxyModelAdapter;
            if (proxy == null)
            {
                return "default";
            }
            return GetGroupName(proxy.Model);
        }

        private string GetGroupName(ServiceProxyModel proxy)
        {
            return ExecutionContext.GetDaprApplicationName(proxy.InternalElement.MappedElement.ApplicationId);
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            var proxies = _typedModels.Distinct(new ServiceModelComparer());
            var groups = proxies.Select(p => GetGroupName(p)).Distinct();
            foreach (var groupName in groups)
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(groupName, "Uri"), $"http://{groupName}/"));
            }
        }

        private static string GetConfigKey(string groupName, string key)
        {
            return $"HttpClients:{groupName}:{key.ToPascalCase()}";
        }

        class ServiceModelComparer : IEqualityComparer<ServiceProxyModel>
        {
            public bool Equals(ServiceProxyModel x, ServiceProxyModel y)
            {
                if (x == null || y == null)
                {
                    return false;
                }

                return Equals(x.Mapping.ElementId, y.Mapping.ElementId);
            }

            public int GetHashCode(ServiceProxyModel obj)
            {
                return obj.Mapping.ElementId.GetHashCode();
            }
        }
    }
}