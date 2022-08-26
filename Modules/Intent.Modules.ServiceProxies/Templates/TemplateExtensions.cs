using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ServiceProxies.Templates.ServiceProxyClient;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ServiceProxies.Templates
{
    public static class TemplateExtensions
    {
        public static string GetServiceProxyClientName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyModel
        {
            return template.GetTypeName(ServiceProxyClientTemplate.TemplateId, template.Model);
        }

        public static string GetServiceProxyClientName(this IntentTemplateBase template, Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyModel model)
        {
            return template.GetTypeName(ServiceProxyClientTemplate.TemplateId, model);
        }

    }
}