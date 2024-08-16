using System.Collections.Generic;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDependencyInjectionName(this IIntentTemplate template)
        {
            return template.GetTypeName(DependencyInjectionTemplate.TemplateId);
        }

    }
}