using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Dapr.AspNetCore.Secrets.Templates.DaprSecretsConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.Secrets.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDaprSecretsConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(DaprSecretsConfigurationTemplate.TemplateId);
        }

    }
}