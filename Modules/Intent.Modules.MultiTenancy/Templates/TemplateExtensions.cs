using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.MultiTenancy.Templates.MultiTenantConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.MultiTenancy.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMultiTenantConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MultiTenantConfigurationTemplate.TemplateId);
        }

    }
}