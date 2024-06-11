using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientConfiguration;
using Intent.Modules.Integration.HttpClients.Templates.HttpClient;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.HttpClientConfiguration
{
    [IntentManaged(Mode.Fully, Signature = Mode.Ignore, Body = Mode.Ignore)]
    public partial class HttpClientConfigurationTemplate : HttpClientConfigurationBase
    {
        public const string TemplateId = "Intent.Integration.HttpClients.HttpClientConfiguration";
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
                      options.AddStatement($"http.Timeout = configuration.GetValue<TimeSpan?>(\"{GetConfigKey(proxy, "Timeout")}\") ?? TimeSpan.FromSeconds(100);");
                  }
                  )
        {
            _typedModels = model;
        }

        public override RoslynMergeConfig ConfigureRoslynMerger() => ToFullyManagedUsingsMigration.GetConfig(Id, 2);

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:Address", "https://localhost:{sts_port}/connect/token"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:ClientId", "clientId"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:ClientSecret", "secret"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:Scope", "api"));
            foreach (var proxy in _typedModels.Distinct(new ServiceModelComparer()))
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(proxy, "Uri"), "https://localhost:{app_port}/"));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(proxy, "IdentityClientKey"), "default"));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(proxy, "Timeout"), "00:01:00"));
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