using System.Collections.Generic;
using Intent.Modules.AspNetCore.Identity.Templates.AspNetCoreIdentityConfiguration;
using Intent.Modules.AspNetCore.Identity.Templates.IdentityServiceCollectionExtensions;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAspNetCoreIdentityConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(AspNetCoreIdentityConfigurationTemplate.TemplateId);
        }

        public static string GetIdentityServiceCollectionExtensionsName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(IdentityServiceCollectionExtensionsTemplate.TemplateId);
        }

    }
}