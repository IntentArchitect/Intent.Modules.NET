using System.Collections.Generic;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClient;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClientConfiguration;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClientDaprHandler;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClientHeaderHandler;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClientRequestException;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.JsonResponse;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.ProblemDetailsWithErrors;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates
{
    public static class TemplateExtensions
    {
        public static string GetHttpClientName<T>(this IIntentTemplate<T> template) where T : ServiceProxyModel
        {
            return template.GetTypeName(HttpClientTemplate.TemplateId, template.Model);
        }

        public static string GetHttpClientName(this IIntentTemplate template, ServiceProxyModel model)
        {
            return template.GetTypeName(HttpClientTemplate.TemplateId, model);
        }

        public static string GetHttpClientConfigurationTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(HttpClientConfigurationTemplate.TemplateId);
        }

        public static string GetHttpClientDaprHandlerTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(HttpClientDaprHandlerTemplate.TemplateId);
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