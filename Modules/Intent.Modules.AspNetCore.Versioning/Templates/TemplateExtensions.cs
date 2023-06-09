using System.Collections.Generic;
using Intent.Modules.AspNetCore.Versioning.Templates.ApiVersioningConfiguration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Versioning.Templates
{
    public static class TemplateExtensions
    {
        public static string GetApiVersioningConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(ApiVersioningConfigurationTemplate.TemplateId);
        }

    }
}