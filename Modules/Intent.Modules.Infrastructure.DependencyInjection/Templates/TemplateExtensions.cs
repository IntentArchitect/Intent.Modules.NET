using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Infrastructure.DependencyInjection.Templates.DependencyInjection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Infrastructure.DependencyInjection.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDependencyInjectionName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(DependencyInjectionTemplate.TemplateId);
        }

    }
}