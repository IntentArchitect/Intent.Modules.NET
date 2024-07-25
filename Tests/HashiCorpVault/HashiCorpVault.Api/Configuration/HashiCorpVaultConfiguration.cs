using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.HashiCorp.Vault.HashiCorpVaultConfiguration", Version = "1.0")]

namespace HashiCorpVault.Api.Configuration
{
    public static class HashiCorpVaultConfiguration
    {
        public static void ConfigureHashiCorpVault(this IConfigurationBuilder builder, IConfiguration configuration)
        {
            var options = new HashiCorpVaultOptions();
            configuration.GetSection("HashiCorpVault").Bind(options);

            foreach (var vault in options.Vaults)
            {
                var shorthandConfig = configuration.GetChildren().Where(p => p.Key.StartsWith($"{vault.Name}_")).ToArray();
                vault.ApplyShorthandConfig(shorthandConfig);
                builder.Add(new HashiCorpVaultConfigurationSource(vault));
            }
        }
    }
}