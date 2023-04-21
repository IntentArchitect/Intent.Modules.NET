using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates.HttpClientConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class HttpClientConfigurationTemplate : CSharpTemplateBase<IList<ServiceProxyModel>>
    {
        public const string TemplateId = "Intent.Integration.HttpClients.HttpClientConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientConfigurationTemplate(IOutputTarget outputTarget, IList<ServiceProxyModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.IdentityModelAspNetCore);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"HttpClientConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override void BeforeTemplateExecution()
        {
            foreach (var proxy in Model.Distinct(new ServiceModelComparer()))
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(proxy, "Uri"), "https://localhost:{app_port}/"));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(proxy, "IdentityClientKey"), "default"));
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(proxy, "Timeout"), "00:01:00"));
            }

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddHttpClients", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));

            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:Address", "https://localhost:{sts_port}/connect/token"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:ClientId", "clientId"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:ClientSecret", "secret"));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients:default:Scope", "api"));
        }

        private string GetConfigKey(ServiceProxyModel proxy, string key)
        {
            return $"HttpClients:{proxy.Name.ToPascalCase()}{(string.IsNullOrEmpty(key) ? string.Empty : ":")}{key?.ToPascalCase()}";
        }

        private string GetMessageHandlers(ServiceProxyModel proxy)
        {
            var parentSecured = default(bool?);
            if (proxy.GetMappedEndpoints()
                .All(x => !x.RequiresAuthorization && (parentSecured ??= x.InternalElement.ParentElement?.TryGetSecured(out _)) != true))
            {
                return string.Empty;
            }

            return $".AddClientAccessTokenHandler(configuration.GetValue<string>(\"{GetConfigKey(proxy, "IdentityClientKey")}\") ?? \"default\")";
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