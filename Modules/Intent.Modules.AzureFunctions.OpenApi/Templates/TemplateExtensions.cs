using System.Collections.Generic;
using Intent.Modules.AzureFunctions.OpenApi.Templates.OpenApiConfiguration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.OpenApi.Templates
{
    public static class TemplateExtensions
    {
        public static string GetOpenApiConfigurationTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(OpenApiConfigurationTemplate.TemplateId);
        }

    }
}