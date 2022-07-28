using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.DependencyInjection.EntityFrameworkCore.Templates.DbContextConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.DependencyInjection.EntityFrameworkCore.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDbContextConfigurationName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(DbContextConfigurationTemplate.TemplateId);
        }

    }
}