using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Security.JWT.Templates.ConfigurationJWTAuthentication;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Security.JWT.Templates
{
    public static class TemplateExtensions
    {
        public static string GetConfigurationJWTAuthenticationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ConfigurationJWTAuthenticationTemplate.TemplateId);
        }

    }
}