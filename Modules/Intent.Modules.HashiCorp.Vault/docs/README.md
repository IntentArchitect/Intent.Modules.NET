# Intent.HashiCorp.Vault

## Overview

HashiCorp Vault provides organizations with identity-based security to automatically authenticate and authorize access to secrets and other sensitive data.

## Connecting to HashiCorp Vault

To connect to HashiCorp Vault, include the following configuration in your `appsettings.json` file:

```json
{
  "HashiCorpVault": {
    "Enabled": true,
    "Vaults": [
      {
        "Name": "DevVault",
        "Url": "http://127.0.0.1:8200",
        "AuthMethod": {
          "Token": {
            "Token": "root_token"
          }
        },
        "Path": "creds",
        "MountPoint": "secret",
        "CacheTimeoutInSeconds": 5
      }
    ]
  }
}
```

### Configuration Parameters

- **Enabled**: Determines whether the Vault integration is active.
- **Vaults**: A list of Vault configurations.
  - **Name**: A friendly name for reference.
  - **Url**: The address to connect to a specific Vault.
  - **AuthMethod**: The method for authenticating against the Vault:
    - [**Token**](https://developer.hashicorp.com/vault/docs/auth/token): Specify a token for DEV mode.
      ```json
      {
        "Token": {
          "Token": "root_token"
        }
      }
      ```
    - [**UserPass**](https://developer.hashicorp.com/vault/docs/auth/userpass): Specify a username and password.
      ```json
      {
        "UserPass": {
          "Username": "username",
          "Password": "password"
        }
      }
      ```
    - [**AppRole**](https://developer.hashicorp.com/vault/docs/auth/approle): Specify a RoleId and SecretId.
      ```json
      {
        "AppRole": {
          "RoleId": "af9bf2b4-d8ab-4451-be44-131263a92d34",
          "SecretId": "5a3d91b6-b657-4a3b-a140-610008f4ab81"
        }
      }
      ```
  - **Path**: Location where secrets are stored.
  - **MountPoint**: Typically set to "secret".
  - **CacheTimeoutInSeconds**:
    - Positive value: Interval to re-fetch secrets.
    - Zero: Fetch secrets only at startup.

#### Shorthand configuration

Once you have the vaults configured in your `appsettings.json` file, you can override their values by supplying configuration from a different source (such as Environment variables) using the following notation:

`{Vault Name}_{Property}` = `{Value}`

Using the [appsettings.json here](#connecting-to-hashicorp-vault), here are a few examples:

- DevVault_Url = "http://dev.host.com:8200"
- DevVault_Token = "alternate_token"
- DevVault_SecretId = "84921e64-d8d5-4d6e-99d8-71486c2ade10"

## Running HashiCorp Vault Locally

Follow these instructions to get Vault up and running in `DEV` mode on your local machine.

### Installation

Download and install HashiCorp Vault from [here](https://developer.hashicorp.com/vault/tutorials/getting-started/getting-started-install#install-vault).

### Starting the Dev Server

Run the following command in your terminal:

```powershell
vault server -dev -dev-root-token-id=root_token
```

Set the environment variable for CLI commands:

```powershell
# PowerShell
$env:VAULT_ADDR="http://127.0.0.1:8200"
```

```bash
# Bash
export VAULT_ADDR='http://127.0.0.1:8200'
```

> [!NOTE]
> 
> The server is using an In Memory database so upon shut down all changes will be lost. To avoid manual setup each time, create a script that starts Vault and configures all necessary secrets.
> The `Root Token` is set to `root_token` for easy access in development mode. Use this token to connect to Vault and its [UI](#browsing-the-vault-ui).

### Adding Secrets to Vault

Add secrets with the following command:

```powershell
vault kv put -mount=secret creds passcode=my-long-passcode
```

You can also specify a JSON payload:

```powershell
# Powershell
echo '{"username":"joe","password":"pass123","meta":[{"key":"creation","value":"3/4/2024 13:05:28"}]}' | vault kv put secret/creds -
```

```bash
# Bash
echo '{"username":"joe","password":"pass123","meta":[{"key":"creation","value":"3/4/2024 13:05:28"}]}' | vault kv put secret/creds -
```

Learn more about these commands [here](https://developer.hashicorp.com/vault/docs/commands/kv).

### Enabling App Role Authentication

Enable [App Role authentication](https://developer.hashicorp.com/vault/docs/auth/approle) with:

```powershell
vault auth enable approle
```

Set up the access policy:

```powershell
# Powershell
$policy = @"
path "secret/data/creds" {
  capabilities = [ "read" ]
}
"@

$policy | vault policy write dev-policy -
```

```bash
# Bash
policy='
path "secret/data/creds" {
  capabilities = ["read"]
}
'

echo "$policy" | vault policy write dev-policy -
```

Create the role:

```powershell
vault write auth/approle/role/my-role `
    token_policies="dev-policy" `
    token_ttl=1h `
    token_max_ttl=4h
```

Retrieve the `Role Id`:

```powershell
vault read auth/approle/role/my-role/role-id
```

Create the secret and obtain the `Secret Id`:

```powershell
vault write -f auth/approle/role/my-role/secret-id
```

### Browsing the Vault UI

Access the Vault UI at [http://127.0.0.1:8200/ui](http://127.0.0.1:8200/ui). Learn more about the UI [here](https://developer.hashicorp.com/vault/tutorials/getting-started-ui/getting-started-ui#lab-setup).
