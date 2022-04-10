using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDbContextName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(DbContextTemplate.TemplateId);
        }

        public static string GetEntityTypeConfigurationName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Domain.Api.ClassModel
        {
            return template.GetTypeName(EntityTypeConfigurationTemplate.TemplateId, template.Model);
        }

        public static string GetEntityTypeConfigurationName(this IntentTemplateBase template, Intent.Modelers.Domain.Api.ClassModel model)
        {
            return template.GetTypeName(EntityTypeConfigurationTemplate.TemplateId, model);
        }

    }
}