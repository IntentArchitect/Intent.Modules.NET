using System.Collections.Generic;
using Intent.Modules.Aws.Common.Templates.AwsConfiguration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Aws.Common.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAwsConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AwsConfigurationTemplate.TemplateId);
        }

    }
}