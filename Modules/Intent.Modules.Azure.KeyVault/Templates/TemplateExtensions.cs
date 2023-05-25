using System.Collections.Generic;
using Intent.Modules.Azure.KeyVault.Templates.AzureKeyVaultConfiguration;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Azure.KeyVault.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureKeyVaultConfigurationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AzureKeyVaultConfigurationTemplate.TemplateId);
        }

    }
}