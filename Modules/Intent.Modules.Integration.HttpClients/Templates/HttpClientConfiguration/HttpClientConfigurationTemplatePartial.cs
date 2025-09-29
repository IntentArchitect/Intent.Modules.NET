using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Application.Contracts.Clients.Templates;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Integration.HttpClients.Settings;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;
using Intent.Modules.Integration.HttpClients.Shared.Templates.HttpClientConfiguration;
using Intent.Modules.Integration.HttpClients.Templates.HttpClient;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Persistence;
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
                      options.AddStatement($"ApplyAppSettings(http, configuration, \"{GetGroupName(proxy.InternalElement)}\", \"{proxy.Name.ToPascalCase()}\");");
                  })
        {
            _typedModels = model;
        }

        public override RoslynMergeConfig ConfigureRoslynMerger() => ToFullyManagedUsingsMigration.GetConfig(Id, 2);

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            if (ExecutionContext.Settings.GetIntegrationHttpClientSettings().AuthorizationSetup().IsClientAccessTokenManagement())
            {
                var dotNetVersion = OutputTarget.GetMaxNetAppVersion();
                if (dotNetVersion.Major >= 8)
                {
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:TokenEndpoint", "https://localhost:{sts_port}/connect/token"));
                }
                else
                {
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:Address", "https://localhost:{sts_port}/connect/token"));
                }

                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:ClientId", "clientId"));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:ClientSecret", "secret"));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:Scope", "api"));
            }

            var proxies = _typedModels.DistinctBy(x => x.Id);
            var proxySettings = proxies.Select(p => (GroupName: GetGroupName(p.InternalElement), Url: ProxyUrlHelper.GetProxyApplicationtUrl(p.InternalElement, ExecutionContext))).DistinctBy(d => d.GroupName);
            foreach (var (GroupName, Url) in proxySettings)
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(GroupName, "Uri"), Url));
                if (ExecutionContext.Settings.GetIntegrationHttpClientSettings().AuthorizationSetup().IsClientAccessTokenManagement())
                {
                    ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(GroupName, "IdentityClientKey"), "default"));
                }
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(GroupName, "Timeout"), "00:01:00"));

            }
        }

        internal static string GetConfigKey(IServiceProxyModel proxy, KeyType keyType, string key)
        {
            switch (keyType)
            {
                case KeyType.Group:
                    return GetConfigKey(GetGroupName(proxy.InternalElement), key);
                case KeyType.Service:
                default:
                    return GetConfigKey(proxy.Name.ToPascalCase(), key);
            }
        }

        internal static string GetGroupName(IElement element)
        {
            var result = element.MappedElement?.Element?.Package?.Name;
            result ??= element.Package.Name;
            return result;
        }

        private static string GetConfigKey(string groupName, string key)
        {
            return $"HttpClients:{groupName}:{key.ToPascalCase()}";
        }
    }

    [IntentIgnore]
    public enum KeyType
    {
        Group,
        Service
    }
}