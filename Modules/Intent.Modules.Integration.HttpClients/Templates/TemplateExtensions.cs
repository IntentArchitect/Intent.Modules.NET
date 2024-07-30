using System.Collections.Generic;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Templates.AuthorizationHeaderProviderInterface;
using Intent.Modules.Integration.HttpClients.Templates.HttpClient;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientAuthorizationHeaderHandler;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientConfiguration;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientHeaderHandler;
using Intent.Modules.Integration.HttpClients.Templates.HttpClientRequestException;
using Intent.Modules.Integration.HttpClients.Templates.JsonResponse;
using Intent.Modules.Integration.HttpClients.Templates.ProblemDetailsWithErrors;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAuthorizationHeaderProviderInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AuthorizationHeaderProviderInterfaceTemplate.TemplateId);
        }
        public static string GetHttpClientName<T>(this IIntentTemplate<T> template) where T : ServiceProxyModel
        {
            return template.GetTypeName(HttpClientTemplate.TemplateId, template.Model);
        }

        public static string GetHttpClientName(this IIntentTemplate template, ServiceProxyModel model)
        {
            return template.GetTypeName(HttpClientTemplate.TemplateId, model);
        }

        public static string GetHttpClientAuthorizationHeaderHandlerName(this IIntentTemplate template)
        {
            return template.GetTypeName(HttpClientAuthorizationHeaderHandlerTemplate.TemplateId);
        }

        public static string GetHttpClientConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(HttpClientConfigurationTemplate.TemplateId);
        }

        public static string GetHttpClientHeaderHandlerTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(HttpClientHeaderHandlerTemplate.TemplateId);
        }

        public static string GetHttpClientRequestExceptionName(this IIntentTemplate template)
        {
            return template.GetTypeName(HttpClientRequestExceptionTemplate.TemplateId);
        }

        public static string GetJsonResponseName(this IIntentTemplate template)
        {
            return template.GetTypeName(JsonResponseTemplate.TemplateId);
        }

        public static string GetProblemDetailsWithErrorsName(this IIntentTemplate template)
        {
            return template.GetTypeName(ProblemDetailsWithErrorsTemplate.TemplateId);
        }

    }
}