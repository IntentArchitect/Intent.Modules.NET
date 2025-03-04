using System.Collections.Generic;
using Intent.Modules.ApiGateway.Ocelot.Templates.OcelotConfiguration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.ApiGateway.Ocelot.Templates
{
    public static class TemplateExtensions
    {
        public static string GetOcelotConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(OcelotConfigurationTemplate.TemplateId);
        }

    }
}