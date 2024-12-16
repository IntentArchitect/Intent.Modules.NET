# Intent.Azure.KeyVault

## Overview

The Azure KeyVault module simplifies working with Azure Key Vault in .NET applications by providing seamless integration with the .NET `IConfiguration` service. This enables secure storage and access to sensitive information such as passwords, API keys, and connection strings.

## Configuration

To configure Azure Key Vault, you need to include the following settings in your `appsettings.json` file:

```json
{
  "KeyVault": {
    "Enabled": true,
    "Endpoint": "https://VAULT-NAME-HERE.vault.azure.net/",
    "ClientId": "",
    "Secret": "",
    "TenantId": ""
  }
}
```

### Configuration Parameters

- **Enabled**: Determines whether the Key Vault integration is active.
- **Endpoint**: The address of your Azure Key Vault.
- **ClientId**: The client ID of the Azure AD application used to access Key Vault.
- **Secret**: The client secret of the Azure AD application.
- **TenantId**: The tenant ID of the Azure AD application.

The following combinations will make use the following authentication methods:

- TenantId, ClientId and Secret : Full connection details for connecting to Azure Key Vault.
- ClientId : If the app is already configured, just specify the ClientId.
- None: Performs credential discovery.