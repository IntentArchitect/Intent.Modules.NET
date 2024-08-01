using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.NetTopologySuite.Templates.GeoDestructureSerilogPolicy;
using Intent.Modules.NetTopologySuite.Templates.GeoJsonSchemaSwaggerFilter;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.NetTopologySuite.Templates
{
    public static class TemplateExtensions
    {
        public static string GetGeoDestructureSerilogPolicyName(this IIntentTemplate template)
        {
            return template.GetTypeName(GeoDestructureSerilogPolicyTemplate.TemplateId);
        }
        public static string GetGeoJsonSchemaSwaggerFilterName(this IIntentTemplate template)
        {
            return template.GetTypeName(GeoJsonSchemaSwaggerFilterTemplate.TemplateId);
        }

    }
}