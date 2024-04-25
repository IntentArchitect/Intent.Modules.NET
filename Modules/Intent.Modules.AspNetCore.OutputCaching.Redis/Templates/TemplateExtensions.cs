using System.Collections.Generic;
using Intent.Modules.AspNetCore.OutputCaching.Redis.Templates.OutputCachingConfiguration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OutputCaching.Redis.Templates
{
    public static class TemplateExtensions
    {
        public static string GetOutputCachingConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(OutputCachingConfigurationTemplate.TemplateId);
        }

    }
}