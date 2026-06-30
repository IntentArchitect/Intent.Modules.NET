using System.Collections.Generic;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Integration.HttpClients.Stubs.Templates.HttpClientStub;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Stubs.Templates
{
    public static class TemplateExtensions
    {
        public static string GetHttpClientStubName<T>(this IIntentTemplate<T> template) where T : ServiceProxyModel
        {
            return template.GetTypeName(HttpClientStubTemplate.TemplateId, template.Model);
        }

        public static string GetHttpClientStubName(this IIntentTemplate template, ServiceProxyModel model)
        {
            return template.GetTypeName(HttpClientStubTemplate.TemplateId, model);
        }
    }
}
