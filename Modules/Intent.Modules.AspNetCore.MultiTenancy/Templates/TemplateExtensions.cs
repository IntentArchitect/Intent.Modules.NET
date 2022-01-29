using System.Collections.Generic;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenancyConfiguration;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenantStoreDbContext;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMultiTenancyConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MultiTenancyConfigurationTemplate.TemplateId);
        }

        public static string GetMultiTenantStoreDbContextName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MultiTenantStoreDbContextTemplate.TemplateId);
        }

    }
}