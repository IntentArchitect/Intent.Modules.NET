using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Templates.HttpClient;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientConfiguration;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientRequestException;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates
{
    public static class TemplateExtensions
    {
        public static string GetHttpClientName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyModel
        {
            return template.GetTypeName(HttpClientTemplate.TemplateId, template.Model);
        }

        public static string GetHttpClientName(this IntentTemplateBase template, Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyModel model)
        {
            return template.GetTypeName(HttpClientTemplate.TemplateId, model);
        }

        public static string GetHttpClientConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(HttpClientConfigurationTemplate.TemplateId);
        }

        public static string GetHttpClientRequestExceptionName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(HttpClientRequestExceptionTemplate.TemplateId);
        }

    }
}