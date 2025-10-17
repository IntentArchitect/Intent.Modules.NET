using System.Collections.Generic;
using Intent.Modules.Aws.SecretsManager.Templates.AwsSecretsManagerConfiguration;
using Intent.Modules.Aws.SecretsManager.Templates.AwsSecretsManagerConfigurationSource;
using Intent.Modules.Aws.SecretsManager.Templates.AwsSecretsManagerOptions;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Aws.SecretsManager.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAwsSecretsManagerConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AwsSecretsManagerConfigurationTemplate.TemplateId);
        }

        public static string GetAwsSecretsManagerConfigurationSourceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AwsSecretsManagerConfigurationSourceTemplate.TemplateId);
        }

        public static string GetAwsSecretsManagerOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(AwsSecretsManagerOptionsTemplate.TemplateId);
        }

    }
}