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
                      options.AddStatement($"http.BaseAddress = configuration.GetValue<Uri>(\"{GetConfigKey(proxy, "Uri")}\");");
                  }
                  )
        {
            _typedModels = model;
        }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            foreach (var proxy in _typedModels.Distinct(new ServiceModelComparer()))
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(proxy, "Uri"), $"http://{ExecutionContext.GetDaprApplicationName(proxy.InternalElement.MappedElement.ApplicationId)}/"));
            }
        }

        private static string GetConfigKey(IServiceProxyModel proxy, string key)
        {
            return $"HttpClients:{proxy.Name.ToPascalCase()}{(string.IsNullOrEmpty(key) ? string.Empty : ":")}{key?.ToPascalCase()}";
        }

        private static string GetConfigKey(ServiceProxyModel proxy, string key)
        {
            return $"HttpClients:{proxy.Name.ToPascalCase()}{(string.IsNullOrEmpty(key) ? string.Empty : ":")}{key?.ToPascalCase()}";
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