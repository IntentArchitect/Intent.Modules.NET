using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.DiffAudit.Templates.DiffAuditInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DiffAudit.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDiffAuditInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DiffAuditInterfaceTemplate.TemplateId);
        }

    }
}