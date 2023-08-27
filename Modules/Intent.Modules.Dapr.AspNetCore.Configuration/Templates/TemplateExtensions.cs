using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Configuration.Templates.DaprConfigurationConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Configuration.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDaprConfigurationConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprConfigurationConfigurationTemplate.TemplateId);
        }

    }
}