using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.HashiCorp.Vault.Templates.HashiCorpVaultConfiguration;
using Intent.Modules.HashiCorp.Vault.Templates.HashiCorpVaultConfigurationSource;
using Intent.Modules.HashiCorp.Vault.Templates.HashiCorpVaultOptions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.HashiCorp.Vault.Templates
{
    public static class TemplateExtensions
    {
        public static string GetHashiCorpVaultConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(HashiCorpVaultConfigurationTemplate.TemplateId);
        }

        public static string GetHashiCorpVaultConfigurationSourceName(this IIntentTemplate template)
        {
            return template.GetTypeName(HashiCorpVaultConfigurationSourceTemplate.TemplateId);
        }

        public static string GetHashiCorpVaultOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(HashiCorpVaultOptionsTemplate.TemplateId);
        }

    }
}