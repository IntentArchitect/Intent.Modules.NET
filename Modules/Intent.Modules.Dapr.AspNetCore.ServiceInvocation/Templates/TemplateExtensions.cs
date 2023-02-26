using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClient;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.HttpClientRequestException;
using Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates.JsonResponse;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.ServiceInvocation.Templates
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

        public static string GetHttpClientRequestExceptionName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(HttpClientRequestExceptionTemplate.TemplateId);
        }

        public static string GetJsonResponseName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(JsonResponseTemplate.TemplateId);
        }

    }
}