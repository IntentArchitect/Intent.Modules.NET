using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ServiceProxies.Templates.ServiceProxiesConfiguration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class ServiceProxiesConfigurationTemplate : CSharpTemplateBase<IList<ServiceProxyModel>>
    {
        public const string TemplateId = "Intent.ServiceProxies.ServiceProxiesConfiguration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceProxiesConfigurationTemplate(IOutputTarget outputTarget, IList<ServiceProxyModel> model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.IdentityModelAspNetCore);
        }

        public override void BeforeTemplateExecution()
        {
            foreach (var proxy in Model.Distinct(new ServiceModelComparer()))
            {
                ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(GetConfigKey(proxy, null),
                    new
                    {
                        Uri = "https://localhost/",
                        IdentityClientKey = "default",
                        Timeout = "00:01:00"
                    }));
            }

            ExecutionContext.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddServiceProxies", ServiceConfigurationRequest.ParameterType.Configuration)
                .ForConcern("Infrastructure")
                .HasDependency(this));
            ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest("IdentityClients", new
            {
                @default = new
                {
                    Address = "https://localhost:{sts_port}/connect/token",
                    ClientId = "clientId",
                    ClientSecret = "secret",
                    Scope = "api"
                }
            }));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ServiceProxiesConfiguration",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        private string GetConfigKey(ServiceProxyModel proxy, string key)
        {
            return $"Proxies:{proxy.MappedService.Name.ToPascalCase()}{(string.IsNullOrEmpty(key) ? string.Empty : ":")}{key?.ToPascalCase()}";
        }

        private string GetMessageHandlers(ServiceProxyModel proxy)
        {
            var handlers = new List<string>();

            if (proxy.MappedService.HasSecured() || proxy.MappedService.Operations.Any(x => x.HasSecured()))
            {
                handlers.Add($@".AddClientAccessTokenHandler(configuration.GetValue<string>(""{GetConfigKey(proxy, "IdentityClientKey")}"") ?? ""default"")");
            }
            
            const string newLine = @"
                ";
            return string.Join(newLine, handlers);
        }
    }

    class ServiceModelComparer : IEqualityComparer<ServiceProxyModel>
    {
        public bool Equals(ServiceProxyModel x, ServiceProxyModel y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return Equals(x.MappedService, y.MappedService);
        }

        public int GetHashCode(ServiceProxyModel obj)
        {
            return obj.MappedService.GetHashCode();
        }
    }
}