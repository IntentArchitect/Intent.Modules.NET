using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.SoftDelete.Templates.SoftDeleteInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.SoftDelete.Templates
{
    public static class TemplateExtensions
    {
        public static string GetSoftDeleteInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(SoftDeleteInterfaceTemplate.TemplateId);
        }

    }
}