using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModularMonolith.ModuleInfrastructure.Templates.ModuleInstaller;
using Intent.Modules.ModularMonolith.ModuleInfrastructure.Templates.ModuleInstallerInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.ModuleInfrastructure.Templates
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