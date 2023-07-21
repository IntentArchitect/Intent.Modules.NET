using System.Collections.Generic;
using Intent.Modules.AspNetCore.Cors.Templates.CorsConfiguration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Cors.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCorsConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(CorsConfigurationTemplate.TemplateId);
        }

    }
}