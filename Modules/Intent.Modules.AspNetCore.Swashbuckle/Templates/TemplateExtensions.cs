using System.Collections.Generic;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.SwashbuckleConfiguration;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.TypeSchemaFilter;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Templates
{
    public static class TemplateExtensions
    {

        public static string GetSwashbuckleConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(SwashbuckleConfigurationTemplate.TemplateId);
        }

        public static string GetTypeSchemaFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(TypeSchemaFilterTemplate.TemplateId);
        }

    }
}