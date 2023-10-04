using System.Collections.Generic;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenancyConfiguration;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.MultiTenantStoreDbContext;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.Swashbuckle.TenantHeaderOperationFilter;
using Intent.Modules.AspNetCore.MultiTenancy.Templates.TenantExtendedInfo;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMultiTenancyConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(MultiTenancyConfigurationTemplate.TemplateId);
        }

        public static string GetMultiTenantStoreDbContextName(this IIntentTemplate template)
        {
            return template.GetTypeName(MultiTenantStoreDbContextTemplate.TemplateId);
        }

        public static string GetTenantExtendedInfoName(this IIntentTemplate template)
        {
            return template.GetTypeName(TenantExtendedInfoTemplate.TemplateId);
        }

        public static string GetTenantHeaderOperationFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(TenantHeaderOperationFilterTemplate.TemplateId);
        }

    }
}