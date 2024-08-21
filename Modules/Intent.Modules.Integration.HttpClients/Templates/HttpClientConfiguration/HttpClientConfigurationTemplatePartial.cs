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
using Intent.Modules.Integration.HttpClients.Settings;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;
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
                      options.AddStatement($"ApplyAppSettings(http, configuration, \"{GetGroupName(proxy)}\", \"{proxy.Name.ToPascalCase()}\");");
                  }
                  )
        {
            _typedModels = model;
        }

        public override RoslynMergeConfig ConfigureRoslynMerger() => ToFullyManagedUsingsMigration.GetConfig(Id, 2);

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            if (ExecutionContext.Settings.GetIntegrationHttpClientSettings().AuthorizationSetup().IsClientAccessTokenManagement())
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:Address", "https://localhost:{sts_port}/connect/token"));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:ClientId", "clientId"));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:ClientSecret", "secret"));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:Scope", "api"));
            }


            var proxies = _typedModels.Distinct(new ServiceModelComparer());
            var groups = proxies.Select(p => GetGroupName(p)).Distinct();
            foreach (var groupName in groups)
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(groupName, "Uri"), "https://localhost:{app_port}/"));
                if (ExecutionContext.Settings.GetIntegrationHttpClientSettings().AuthorizationSetup().IsClientAccessTokenManagement())
                {
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(groupName, "IdentityClientKey"), "default"));
                }
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(groupName, "Timeout"), "00:01:00"));

            }
        }

        internal static string GetConfigKey(ServiceProxyModel proxy, KeyType keyType, string key)
        {
            switch (keyType)
            {
                case KeyType.Group:
                    return GetConfigKey(GetGroupName(proxy), key);
                case KeyType.Service:
                default:
                    return GetConfigKey(proxy.Name.ToPascalCase(), key);
            }
        }

        internal static string GetGroupName(IServiceProxyModel proxyInterface)
        {
            var proxy = proxyInterface as ServiceProxyModelAdapter;
            if (proxy == null)
            {
                return "default";
            }
            return GetGroupName(proxy.Model);
        }

        internal static string GetGroupName(ServiceProxyModel proxy)
        {
            var result = proxy.InternalElement.MappedElement?.Element?.Package?.Name;
            result ??= proxy.InternalElement.Package.Name;
            return result;
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

    [IntentIgnore]
    public enum KeyType
    {
        Group,
        Service
    }
}