using System.Collections.Generic;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.HideRouteParametersFromBodyOperationFilter;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.HttpQueryDocumentFilter;
using Intent.Modules.AspNetCore.Swashbuckle.Templates.OpenApi32DocumentSerializer;
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

        public static string GetHideRouteParametersFromBodyOperationFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(HideRouteParametersFromBodyOperationFilterTemplate.TemplateId);
        }

        public static string GetHttpQueryDocumentFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(HttpQueryDocumentFilterTemplate.TemplateId);
        }

        public static string GetOpenApi32DocumentSerializerName(this IIntentTemplate template)
        {
            return template.GetTypeName(OpenApi32DocumentSerializerTemplate.TemplateId);
        }

    }
}