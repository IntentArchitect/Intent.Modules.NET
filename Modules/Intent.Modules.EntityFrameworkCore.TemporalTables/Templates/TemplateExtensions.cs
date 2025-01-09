using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.TemporalTables.Templates.TemporalHistory;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.TemporalTables.Templates
{
    public static class TemplateExtensions
    {
        public static string GetTemporalHistoryName(this IIntentTemplate template)
        {
            return template.GetTypeName(TemporalHistoryTemplate.TemplateId);
        }

    }
}