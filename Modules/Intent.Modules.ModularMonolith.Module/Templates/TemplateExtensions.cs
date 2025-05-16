using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModularMonolith.Module.Templates.ModuleInstaller;
using Intent.Modules.ModularMonolith.Module.Templates.ModuleInstallerInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Merge, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Module.Templates
{
    public static class TemplateExtensions
    {
        public static string GetModuleInstallerTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(ModuleInstallerTemplate.TemplateId);
        }

        [IntentIgnore]
        public static string GetModuleInstallerInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(ModuleInstallerInterfaceTemplate.TemplateId);
        }

    }
}