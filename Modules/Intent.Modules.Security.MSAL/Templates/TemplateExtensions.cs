using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Security.MSAL.Templates.ConfigurationMSALAuthentication;
using Intent.Modules.Security.MSAL.Templates.CurrentUserService;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Security.MSAL.Templates
{
    public static class TemplateExtensions
    {
        public static string GetConfigurationMSALAuthenticationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ConfigurationMSALAuthenticationTemplate.TemplateId);
        }

        public static string GetCurrentUserServiceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(CurrentUserServiceTemplate.TemplateId);
        }

    }
}