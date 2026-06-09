using System.Collections.Generic;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Fakes.Templates.HttpClientFake;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Fakes.Templates
{
    public static class TemplateExtensions
    {
        public static string GetHttpClientFakeName<T>(this IIntentTemplate<T> template) where T : ServiceProxyModel
        {
            return template.GetTypeName(HttpClientFakeTemplate.TemplateId, template.Model);
        }

        public static string GetHttpClientFakeName(this IIntentTemplate template, ServiceProxyModel model)
        {
            return template.GetTypeName(HttpClientFakeTemplate.TemplateId, model);
        }
    }
}
