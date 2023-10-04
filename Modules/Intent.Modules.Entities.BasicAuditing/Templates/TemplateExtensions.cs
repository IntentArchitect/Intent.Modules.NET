using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.BasicAuditing.Templates.AuditableInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.BasicAuditing.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAuditableInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AuditableInterfaceTemplate.TemplateId);
        }

    }
}