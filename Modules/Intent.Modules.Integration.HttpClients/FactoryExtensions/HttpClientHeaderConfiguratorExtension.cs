using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientConfiguration;
using System.Buffers;
using System.Reflection;
using System.Runtime.InteropServices;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Modules.Common.Templates;
using System;
using Intent.Modules.Integration.HttpClients.Shared;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HttpClientHeaderConfiguratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Integration.HttpClients.HttpClientHeaderConfiguratorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {

            HttpClientHeaderConfiguratorHelper.UpdateProxyAuthHeaderPopulation(application, HttpClientConfigurationTemplate.TemplateId);
            UpdateProxyTokenRefreshPopulation(application);
        }


        private void UpdateProxyTokenRefreshPopulation(IApplication application)
        {
            var httpClientConfigurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(HttpClientConfigurationTemplate.TemplateId);

            if (httpClientConfigurationTemplate == null) return;

            httpClientConfigurationTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                var method = @class.FindMethod("AddHttpClients");
                if (method == null) return;

                method.InsertStatement(0, @"services.AddAccessTokenManagement(options =>
            {
                configuration.GetSection(""IdentityClients"").Bind(options.Client.Clients);
            }).ConfigureBackchannelHttpClient();
");

                var proxyConfigurations = method.FindStatements(s => s is CSharpMethodChainStatement && s.TryGetMetadata<ServiceProxyModel>("model", out var _)).Cast<CSharpMethodChainStatement>().ToArray();

                foreach (var proxyConfiguration in proxyConfigurations)
                {
                    var proxyModel = proxyConfiguration.GetMetadata<ServiceProxyModel>("model");

                    if (RequiresMessageHandler(proxyModel))
                    {
                        proxyConfiguration.AddChainStatement(new CSharpInvocationStatement($"AddClientAccessTokenHandler").AddArgument($"configuration.GetValue<string>(\"{GetConfigKey(proxyModel, "IdentityClientKey")}\") ?? \"default\"").WithoutSemicolon());
                    }
                }
            });
        }


        private string GetConfigKey(ServiceProxyModel proxy, string key)
        {
            return $"HttpClients:{proxy.Name.ToPascalCase()}{(string.IsNullOrEmpty(key) ? string.Empty : ":")}{key?.ToPascalCase()}";
        }


        private bool RequiresMessageHandler(ServiceProxyModel proxy)
        {
            var parentSecured = default(bool?);
            return !ServiceProxyHelpers.GetMappedEndpoints(proxy)
                .All(x => !x.RequiresAuthorization && (parentSecured ??= x.InternalElement.ParentElement?.TryGetSecured(out _)) != true);
        }
    }
}