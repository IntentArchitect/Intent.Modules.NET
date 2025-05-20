using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModularMonolith.Host.Templates.ModuleConfiguration;
using Intent.Modules.ModularMonolith.Host.Templates.ModuleInstallerInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Host.Templates
{
    public static class TemplateExtensions
    {
        public static string GetModuleConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(ModuleConfigurationTemplate.TemplateId);
        }

        public static string GetModuleInstallerInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(ModuleInstallerInterfaceTemplate.TemplateId);
        }

    }
}