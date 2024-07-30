using System.Linq;
using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Contracts.Clients.Http.Shared;
using Intent.Modules.Metadata.WebApi.Models;


namespace Intent.Modules.Integration.HttpClients.Shared
{
    public class HttpClientHeaderConfiguratorHelper
    {

        public static void UpdateProxyAuthHeaderPopulation(IApplication application, string HttpClientConfigurationTemplateId)
        {
            var httpClientConfigurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(HttpClientConfigurationTemplateId);

            if (httpClientConfigurationTemplate == null) return;

            httpClientConfigurationTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                var method = @class.FindMethod("AddHttpClients");
                if (method == null) return;
                method.InsertStatement(0, "services.AddHttpContextAccessor();");


                var proxyConfigurations = method.FindStatements(s => s is CSharpMethodChainStatement && s.TryGetMetadata<ServiceProxyModel>("model", out var _))
                    .Cast<CSharpMethodChainStatement>().ToArray();

                foreach (var proxyConfiguration in proxyConfigurations)
                {
                    var proxyModel = proxyConfiguration.GetMetadata<ServiceProxyModel>("model");

                    if (proxyModel.HasMappedEndpoints() && (proxyModel.GetMappedEndpoints().Any(e => e.RequiresAuthorization) ||
                                                            (proxyModel.InternalElement.ParentElement?.TryGetSecured(out _) ?? false)))
                    {
                        proxyConfiguration.AddChainStatement(new CSharpInvocationStatement("AddHeaders")
                            .AddArgument(new CSharpLambdaBlock("config"), a =>
                            {
                                var options = (CSharpLambdaBlock)a;
                                options.AddStatement("config.AddFromHeader(\"Authorization\");");
                            }).WithoutSemicolon()
                        );
                    }
                }
            });
        }

        public static void ImplementAuthorizationHeaderProvider(IApplication application, string HttpClientConfigurationTemplateId, string extensionMethodTemplateId)
        {
            var httpClientConfigurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(HttpClientConfigurationTemplateId);

            if (httpClientConfigurationTemplate == null) return;

            httpClientConfigurationTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();

                var method = @class.FindMethod("AddHttpClients");
                if (method == null) return;


                //Ensure the using clause is added
                httpClientConfigurationTemplate.GetTypeName(extensionMethodTemplateId);

                var proxyConfigurations = method.FindStatements(s => s is CSharpMethodChainStatement && s.TryGetMetadata<ServiceProxyModel>("model", out var _))
                    .Cast<CSharpMethodChainStatement>().ToArray();

                foreach (var proxyConfiguration in proxyConfigurations)
                {
                    var proxyModel = proxyConfiguration.GetMetadata<ServiceProxyModel>("model");

                    if (proxyModel.HasMappedEndpoints() && (proxyModel.GetMappedEndpoints().Any(e => e.RequiresAuthorization) ||
                                                            (proxyModel.InternalElement.ParentElement?.TryGetSecured(out _) ?? false)))
                    {
                        proxyConfiguration.AddChainStatement(new CSharpInvocationStatement("AddAuthorizationHeader").WithoutSemicolon());
                    }
                }
            });
        }
    }
}