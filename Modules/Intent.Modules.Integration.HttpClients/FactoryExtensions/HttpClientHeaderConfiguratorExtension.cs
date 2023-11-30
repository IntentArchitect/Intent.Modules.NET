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
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientConfiguration;
using System.Buffers;
using System.Reflection;
using System.Runtime.InteropServices;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.Modules.Common.Templates;

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
            
            UpdateProxyHeaderPopulation(application);
        }

        private void UpdateProxyHeaderPopulation(IApplication application)
        {
            var httpClientConfigurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(HttpClientConfigurationTemplate.TemplateId);

            if (httpClientConfigurationTemplate == null) return;

            httpClientConfigurationTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                var method = @class.FindMethod("AddHttpClients");
                if (method == null) return;
                method.InsertStatement(0, "services.AddHttpContextAccessor();");

                method.InsertStatement(1, @"services.AddAccessTokenManagement(options =>
            {
                configuration.GetSection(""IdentityClients"").Bind(options.Client.Clients);
            }).ConfigureBackchannelHttpClient();
");

                var proxyConfigurations = method.Statements.Where(s => s is CSharpInvocationStatement && s.TryGetMetadata<ServiceProxyModel>("model", out var _)).Cast<CSharpInvocationStatement>().ToArray();

                foreach (var proxyConfiguration in proxyConfigurations)
                {
                    var proxyModel = proxyConfiguration.GetMetadata<ServiceProxyModel>("model");

                    if (RequiresMessageHandler(proxyModel))
                    {
                        proxyConfiguration.InsertBelow($".AddClientAccessTokenHandler(configuration.GetValue<string>(\"{GetConfigKey(proxyModel, "IdentityClientKey")}\") ?? \"default\"){(RequireSemiColon(proxyConfiguration) ? ";" : "")}");
                    }

                    if (proxyModel.HasMappedEndpoints() && proxyModel.GetMappedEndpoints().Any(e => e.RequiresAuthorization))
                    {
                        proxyConfiguration.WithoutSemicolon();
                        var stmt = new CSharpInvocationStatement(".AddHeaders");
                        stmt.AddArgument(new CSharpLambdaBlock("config"), a =>
                        {
                            var options = (CSharpLambdaBlock)a;
                            options.AddStatement("config.AddFromHeader(\"Authorization\");");
                        });

                        if (!RequireSemiColon(proxyConfiguration))
                        {
                            stmt.WithoutSemicolon();
                        }
                        proxyConfiguration.InsertBelow(stmt);
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


        private bool RequireSemiColon(CSharpStatement stmt)
        {
            int index = stmt.Parent.Statements.IndexOf(stmt);
            if (stmt.Parent.Statements.Count == index + 1)
            {
                return true;
            }
            return !stmt.Parent.Statements[index + 1].ToString().StartsWith(".");
        }
    }
}