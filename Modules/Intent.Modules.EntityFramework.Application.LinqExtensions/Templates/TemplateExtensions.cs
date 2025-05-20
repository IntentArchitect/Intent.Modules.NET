using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFramework.Application.LinqExtensions.Templates.QueryableMarkerExtensions;
using Intent.Modules.EntityFramework.Application.LinqExtensions.Templates.QueryableMarkerImplementation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFramework.Application.LinqExtensions.Templates
{
    public static class TemplateExtensions
    {
        public static string GetQueryableMarkerExtensionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(QueryableMarkerExtensionsTemplate.TemplateId);
        }

        public static string GetQueryableMarkerImplementationName(this IIntentTemplate template)
        {
            return template.GetTypeName(QueryableMarkerImplementationTemplate.TemplateId);
        }

    }
}