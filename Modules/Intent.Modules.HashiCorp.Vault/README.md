# Intent.HashiCorp.Vault

## What is HashiCorp Vault?

HashiCorp Vault provides organizations with identity-based security to automatically authenticate and authorize access to secrets and other sensitive data.

## Connecting to the Vault

In your `appsettings.json` file you should see the following config section.

```json
"HashiCorpVault": {
    "Enabled": true,
    "Vaults": [
      {
        "Name": "Test",
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
```

Setting the "Enabled" property will determine whether the Vault will be integrated into your .NET configuration or not.

You can configure multiple Vaults inside the "Vaults" property.

- Name: This is a friendly name for your reference.
- Url: The address to connect to a specific Vault.
- AuthMethod: Choose how you are going to authenticate against the Vault.
    - Token: Specify a token (i.e. `Root Token` when in `DEV` mode).
        ```json
        "Token": {
            "Token": "root_token"
        }
        ```
    - UserPass: Specify a username and password.
        ```json
        "UserPass": {
            "Username": "username",
            "Password": "password"
        }  
        ```
    - AppRole: Specify a RoleId and SecretId.
        ```json
        "AppRole": {
            "RoleId": "af9bf2b4-d8ab-4451-be44-131263a92d34",
            "SecretId": "5a3d91b6-b657-4a3b-a140-610008f4ab81"
        }
        ```
- Path: Location where secrets are stored.
- MountPoint: Typically "secret".
- CacheTimeoutInSeconds:
    - Positive number: Interval that it will re-fetch secrets from the Vault.
    - Zero: Only fetch secrets at startup of app.

## Running the Vault

The following instructions will get Vault up and running on your developer machine in `DEV` mode.

### Download HashiCorp Vault

Visit [this webpage](https://developer.hashicorp.com/vault/tutorials/getting-started/getting-started-install#install-vault) to learn about installing HashiCorp Vault.

### Starting the Dev Server

Open up your terminal and run the following command:

```powershell
vault server -dev -dev-root-token-id=root_token
```

If you need to execute any CLI commands against the Vault you will need to set this environment variable:

```powershell
PowerShell:
    $env:VAULT_ADDR="http://127.0.0.1:8200"
cmd.exe:
    set VAULT_ADDR=http://127.0.0.1:8200
Linux / Unix:
    export VAULT_ADDR='http://127.0.0.1:8200'
```

This will run HashiCorp Vault in `DEV` mode and the information stored will only be kept in-memory until the application is shutdown in which case you will need to repopulate it with your secrets.

>[!NOTE]
> 
> The `Root Token` has been set to `root_token` for easy access in Development mode. You can connect to the Vault (and its [UI](#browsing-the-vault-ui)) using the `Root Token` directly.

### Adding secrets to the Vault

Once the server is up and the environment variable is set you can start adding secrets to your Vault.

```powershell
vault kv put -mount=secret creds passcode=my-long-passcode
```

For specifying a JSON payload you can do the following:

```powershell
echo '{"username":"joe","password":"pass123","meta":[{"key":"creation","vaule":"3/4/2024 13:05:28"}]}' | vault kv put secret/creds -
```

You can learn more by reading this [page](https://developer.hashicorp.com/vault/docs/commands/kv).

### Enabling App Role authentication

If you need to test your App Role authentication setup for HashiCorp Vault, you will need to follow these steps.

In your terminal type the following.

```powershell
vault auth enable approle
```

This will enable [App Role authentication](https://developer.hashicorp.com/vault/docs/auth/approle).

Next you need to set up a policy to allow this role to access your specific secrets.

```powershell
$policy = @"
path "secret/data/creds" {
  capabilities = [ "read" ]
}
"@ 

$policy | vault policy write dev-policy -
```

You will need to create your role using this command.

```powershell
vault write auth/approle/role/my-role `
    token_policies="dev-policy" `
    token_ttl=1h `
    token_max_ttl=4h
```

Now that it is created you need to get hold of your `Role Id`.

```powershell
vault read auth/approle/role/my-role/role-id
```

Finally you need to create your secret for that role and capture your `Secret Id`.

```powershell
vault write -f auth/approle/role/my-role/secret-id
```

### Setting this up on each startup

To prevent manually going through these steps each time you need to have your Vault up and running before usage, create a script file (e.g. Powershell script) and use the steps mentioned in this document to run the Vault and populate it with all the necessary secrets you need to test against.

### Browsing the Vault UI

You can make use of the UI interface of the Value by opening your browser at [http://127.0.0.1:8200/ui](http://127.0.0.1:8200/ui).

Learn more by viewing this [page](https://developer.hashicorp.com/vault/tutorials/getting-started-ui/getting-started-ui#lab-setup).