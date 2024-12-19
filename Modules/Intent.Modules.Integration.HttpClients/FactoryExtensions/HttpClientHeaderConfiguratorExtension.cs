using System;
using System.Buffers;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Integration.HttpClients.Settings;
using Intent.Modules.Integration.HttpClients.Shared;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientAuthorizationHeaderHandler;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientConfiguration;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HttpClientHeaderConfiguratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Integration.HttpClients.HttpClientHeaderConfiguratorExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;


        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (application.Settings.GetIntegrationHttpClientSettings().AuthorizationSetup().IsTransmittableAccessToken())
            {
                HttpClientHeaderConfiguratorHelper.UpdateProxyAuthHeaderPopulation(application, HttpClientConfigurationTemplate.TemplateId);
            }

            if (application.Settings.GetIntegrationHttpClientSettings().AuthorizationSetup().IsClientAccessTokenManagement())
            {
                UpdateProxyTokenRefreshPopulation(application);
            }

            if (application.Settings.GetIntegrationHttpClientSettings().AuthorizationSetup().IsAuthorizationHeaderProvider())
            {
                HttpClientHeaderConfiguratorHelper.ImplementAuthorizationHeaderProvider(application, HttpClientConfigurationTemplate.TemplateId, HttpClientAuthorizationHeaderHandlerTemplate.TemplateId);
            }
        }


        private void UpdateProxyTokenRefreshPopulation(IApplication application)
        {
            var httpClientConfigurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(HttpClientConfigurationTemplate.TemplateId);

            if (httpClientConfigurationTemplate is null) return;

            httpClientConfigurationTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                var method = @class.FindMethod("AddHttpClients");
                if (method == null) return;

                method.InsertStatement(0,
                    """
                    services.AddAccessTokenManagement(options =>
                                {
                                    configuration.GetSection("IdentityClients").Bind(options.Client.Clients);
                                }).ConfigureBackchannelHttpClient();

                    """);

                var proxyConfigurations = method.FindStatements(s => s is CSharpMethodChainStatement && s.TryGetMetadata<ServiceProxyModel>("model", out var _))
                    .Cast<CSharpMethodChainStatement>().ToArray();

                foreach (var proxyConfiguration in proxyConfigurations)
                {
                    var proxyModel = proxyConfiguration.GetMetadata<ServiceProxyModel>("model");

                    if (RequiresSecurity(proxyModel, application))
                    {
                        proxyConfiguration.AddChainStatement(new CSharpInvocationStatement($"AddClientAccessTokenHandler")
                            .AddArgument($@"configuration.GetValue<string>(""{HttpClientConfigurationTemplate.GetConfigKey(proxyModel, KeyType.Service, "IdentityClientKey")}"") ?? 
                    configuration.GetValue<string>(""{HttpClientConfigurationTemplate.GetConfigKey(proxyModel, KeyType.Group, "IdentityClientKey")}"") ?? 
                    ""default""").WithoutSemicolon());
                    }
                }
            });
        }

        private static bool RequiresSecurity(ServiceProxyModel proxy, IApplication application)
        {

            // This will work for ALL services, but we need to exempt anonymous operations from this...
            // var targetAppId = proxy.InternalElement.MappedElement?.ApplicationId;
            // var defaultApiSecuritySetting = application.GetSolutionConfig().GetApplicationConfig(targetAppId)
            //     .ModuleSetting
            //     .FirstOrDefault(p => p.Id == "4bd0b4e9-7b53-42a9-bb4a-277abb92a0eb") // APISettings
            //     ?.GetSetting("061a559a-0d54-4eb1-8c70-ed0baa238a59"); //DefaultAPISecurityOptions
            //
            // if (defaultApiSecuritySetting?.Value == "secured")
            // {
            //     return true;
            // }

            var parentSecured = default(bool?);
            return !ServiceProxyHelpers.GetMappedEndpoints(proxy)
                .All(x => !x.RequiresAuthorization && (parentSecured ??= x.InternalElement.ParentElement?.TryGetSecured(out _)) != true);
        }
    }
}