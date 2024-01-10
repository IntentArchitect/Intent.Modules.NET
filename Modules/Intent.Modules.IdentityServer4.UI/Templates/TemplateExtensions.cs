using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.IdentityServer4.UI.Templates.Controllers.AccountController;
using Intent.Modules.IdentityServer4.UI.Templates.Controllers.ExternalController;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAccountControllerName(this IIntentTemplate template)
        {
            return template.GetTypeName(AccountControllerTemplate.TemplateId);
        }

        public static string GetExternalControllerName(this IIntentTemplate template)
        {
            return template.GetTypeName(ExternalControllerTemplate.TemplateId);
        }

    }
}