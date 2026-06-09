using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Fakes.Templates.HttpClientFake;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientConfiguration;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Fakes.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class HttpClientFakeRegistrationFactoryExtension : FactoryExtensionBase
    {
        private const string ServiceProxiesDesignerId = "2799aa83-e256-46fe-9589-b96f7d6b09f7";
        private const string UseFakeKey = "UseFake";
        private readonly IMetadataManager _metadataManager;

        public HttpClientFakeRegistrationFactoryExtension(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string Id => "Intent.Integration.HttpClients.Fakes.HttpClientFakeRegistrationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var groupNames = GetServiceProxyModels(application)
                .Select(x => x.GetGroupName())
                .Distinct();

            foreach (var groupName in groupNames)
            {
                application.EventDispatcher.Publish(
                    new AppSettingRegistrationRequest(
                        HttpClientSettingsHelper.GetConfigKey(groupName, UseFakeKey),
                        false));
            }
        }

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var httpClientConfigurationTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                HttpClientConfigurationTemplate.TemplateId);
            if (httpClientConfigurationTemplate is null)
            {
                return;
            }

            httpClientConfigurationTemplate.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var addHttpClientsMethod = @class.FindMethod("AddHttpClients");
                if (addHttpClientsMethod is null)
                {
                    return;
                }

                @class.AddMethod("bool", "UseFakeHttpClient", m =>
                {
                    m.Private()
                        .Static()
                        .AddParameter("IConfiguration", "configuration")
                        .AddParameter("string", "groupName")
                        .AddParameter("string", "serviceName");

                    m.AddReturn($$"""configuration.GetValue<bool?>($"{{HttpClientSettingsHelper.HttpClientsSection}}:{serviceName}:{{UseFakeKey}}") ?? configuration.GetValue<bool?>($"{{HttpClientSettingsHelper.HttpClientsSection}}:{groupName}:{{UseFakeKey}}") ?? false""");
                });

                var proxyConfigurations = addHttpClientsMethod.FindStatements(s => s.TryGetMetadata<IServiceProxyModel>("model", out _))
                    .ToArray();

                foreach (var proxyConfiguration in proxyConfigurations)
                {
                    var proxyModel = proxyConfiguration.GetMetadata<IServiceProxyModel>("model");
                    var groupName = proxyModel.GetGroupName();
                    var serviceName = proxyModel.Name.ToPascalCase();
                    var serviceContractName = httpClientConfigurationTemplate.GetTypeName(ServiceContractTemplate.TemplateId, proxyModel);
                    var fakeName = httpClientConfigurationTemplate.GetTypeName(HttpClientFakeTemplate.TemplateId, proxyModel);
                    var httpClientRegistration = GetRenderedStatementForPreservation(proxyConfiguration);

                    addHttpClientsMethod.AddIfStatement($"UseFakeHttpClient(configuration, \"{groupName}\", \"{serviceName}\")", inside =>
                    {
                        inside.AddStatement($"services.AddTransient<{serviceContractName}, {fakeName}>();");
                    })
                    .AddElseStatement(@else => @else.AddStatements(httpClientRegistration.ConvertToStatements()));

                    proxyConfiguration.Remove();
                }
            }, 1100);
        }

        private IServiceProxyModel[] GetServiceProxyModels(IApplication application)
        {
            return _metadataManager.GetServiceProxyModels(
                application.Id,
                application,
                applicationId => _metadataManager.GetDesigner(applicationId, ServiceProxiesDesignerId),
                _metadataManager.Services).ToArray();
        }

        private static string GetRenderedStatementForPreservation(ICSharpStatement statement)
        {
            // Preserve the fully-built HttpClient registration because other extensions can
            // append chained calls before this final wrapper is applied.
            return statement.GetText("").Trim();
        }
    }
}
