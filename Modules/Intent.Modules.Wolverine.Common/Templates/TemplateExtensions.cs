using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Wolverine.Common.Templates.WolverineConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Wolverine.Common.Templates
{
    public static class TemplateExtensions
    {
        public static string GetWolverineConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(WolverineConfigurationTemplate.TemplateId);
        }

    }
}