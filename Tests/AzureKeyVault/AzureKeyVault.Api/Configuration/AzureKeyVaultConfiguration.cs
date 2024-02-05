using System;
using Azure.Core;
using Azure.Identity;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Azure.KeyVault.AzureKeyVaultConfiguration", Version = "1.0")]

namespace AzureKeyVault.Api.Configuration;

public static class AzureKeyVaultConfiguration
{
    public static void ConfigureAzureKeyVault(this IConfigurationBuilder builder, IConfiguration configuration)
    {
        var credential = GetTokenCredential(configuration);

        if (string.IsNullOrWhiteSpace(configuration["KeyVault:Endpoint"]))
        {
            throw new InvalidOperationException("Configuration 'KeyVault:Endpoint' is not set");
        }
        builder.AddAzureKeyVault(new Uri(configuration["KeyVault:Endpoint"]), credential);
    }

    private static TokenCredential GetTokenCredential(IConfiguration configuration)
    {
        if (!string.IsNullOrWhiteSpace(configuration["KeyVault:TenantId"]) &&
                !string.IsNullOrWhiteSpace(configuration["KeyVault:ClientId"]) &&
                !string.IsNullOrWhiteSpace(configuration["KeyVault:Secret"]))
        {
            // Manually specify the connection details for Azure Key Vault.
            // Its recommended to store the 'Secret' inside the .NET User Secret's secrets.json file.
            return new ClientSecretCredential(configuration["KeyVault:TenantId"], configuration["KeyVault:ClientId"], configuration["KeyVault:Secret"]);
        }

        if (!string.IsNullOrWhiteSpace(configuration["KeyVault:ClientId"]))
        {
            // Connect to Azure Key Vault using the configured App Client Id.
            return new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = configuration["KeyVault:ClientId"]
            });
        }
        // Use the default discovery mechanisms to connect to Azure Key Vault.
        return new DefaultAzureCredential();
    }
}