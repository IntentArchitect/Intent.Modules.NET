using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.IdentityServer4.SecureTokenServer.Templates.IdentityServerConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.SecureTokenServer.Templates
{
    public static class TemplateExtensions
    {
        public static string GetIdentityServerConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityServerConfigurationTemplate.TemplateId);
        }

    }
}