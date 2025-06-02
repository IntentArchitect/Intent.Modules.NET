using System.Collections.Generic;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManager;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManagerInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.Templates
{
    public static class TemplateExtensions
    {
        public static string GetIdentityServiceManagerName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityServiceManagerTemplate.TemplateId);
        }

        public static string GetIdentityServiceManagerInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityServiceManagerInterfaceTemplate.TemplateId);
        }

    }
}