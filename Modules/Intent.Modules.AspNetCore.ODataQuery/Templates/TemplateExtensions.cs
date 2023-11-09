using System.Collections.Generic;
using Intent.Modules.AspNetCore.ODataQuery.Templates.ODataQuerySwaggerFilter;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.ODataQuery.Templates
{
    public static class TemplateExtensions
    {
        public static string GetODataQuerySwaggerFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(ODataQuerySwaggerFilterTemplate.TemplateId);
        }

    }
}