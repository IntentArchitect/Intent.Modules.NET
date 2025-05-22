using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
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

                application.EventDispatcher.Publish(new RemoveNugetPackageEvent(
                NugetPackages.IdentityModelAspNetCorePackageName, file.Template.OutputTarget));
                file.Template.AddNugetDependency(NuGetPackages.DuendeAccessTokenManagement(file.Template.OutputTarget));

                method.InsertStatement(0,
                    """
                    var clientCredentialsBuilder = services.AddClientCredentialsTokenManagement();
                    foreach (var clientCredentials in configuration.GetSection("IdentityClients").GetChildren())
                    {
                        clientCredentialsBuilder.AddClient(clientCredentials.Key, clientCredentials.Bind);
                    }

                    """);

                var proxyConfigurations = method.FindStatements(s => s is CSharpMethodChainStatement && s.TryGetMetadata<ServiceProxyModel>("model", out var _))
                    .Cast<CSharpMethodChainStatement>().ToArray();

                foreach (var proxyConfiguration in proxyConfigurations)
                {
                    var proxyModel = proxyConfiguration.GetMetadata<ServiceProxyModel>("model");

                    if (RequiresSecurity(proxyModel, application))
                    {
                        proxyConfiguration.AddChainStatement(new CSharpInvocationStatement($"AddClientCredentialsTokenHandler")
                            .AddArgument($@"configuration.GetValue<string>(""{HttpClientConfigurationTemplate.GetConfigKey(proxyModel, KeyType.Service, "IdentityClientKey")}"") ?? 
                    configuration.GetValue<string>(""{HttpClientConfigurationTemplate.GetConfigKey(proxyModel, KeyType.Group, "IdentityClientKey")}"") ?? 
                    ""default""").WithoutSemicolon());
                    }
                }
            });
        }

        [IntentIgnore]
        private static bool RequiresSecurity(ServiceProxyModel proxy, IApplication application)
        {
            var parentSecured = default(bool?);
            return !ServiceProxyHelpers.GetMappedEndpoints(proxy, application)
                .All(x => !x.RequiresAuthorization && (parentSecured ??= x.InternalElement.ParentElement?.TryGetSecured(out _)) != true);
        }
    }
}