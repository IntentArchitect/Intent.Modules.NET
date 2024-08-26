# Intent.Security.MSAL

The `Intent.Security.MSAL` module facilitates the integration of Azure Active Directory (Azure AD), EntraID, and Azure B2C to secure ASP.NET Core endpoints using the `Microsoft.Identity.Web` NuGet package.

## Prerequisites

- **Azure subscription**: You need an Azure subscription. If you don't have one, you can create a [free account](https://azure.microsoft.com/free/).

>[!NOTE]
> 
> If you were looking to make use of a different Open ID Connect Identity Provider solution, you will need to look at the `Intent.Security.JWT` module instead.

## Azure Entra ID / AD B2C Setup

### Client Credential Setup

This setup is used when one application needs to invoke endpoints from another, indirectly initiated by a user request or system-generated event.

The following instructions assume a `HostAPI` application exposing API endpoints and a `Client` application consuming them. Adjust these settings based on your specific needs.

#### Registering the Host Application

1. Sign into the [Azure portal](https://portal.azure.com).
2. Navigate to **Azure Active Directory** > **App registrations**.
3. Click **New registration**.
4. Enter a **Name** for the application (e.g., "HostAPI").
5. Select the **Supported account types** as `Accounts in this organizational directory only (Single tenant)`.
6. Leave the **Redirect URI** blank.
7. Click **Register** to create the application.

#### Configure the Host Application

1. Go to the application's **Overview** page.
2. Under **Manage**, select **Owners** and add your Azure user as an owner.
3. Select **Expose an API** and click on `Add a scope`. Have it create the `Application ID URI`.
4. Provide a Scope name, e.g. `Client.Access`.
5. Set the consent to `Admins only`.
6. Populate the display name and description.
7. Ensure it's `Enabled` and click on `Add scope`.

#### Register the Client Application

1. Navigate to **Azure Active Directory** > **App registrations**.
2. Click **New registration**.
3. Enter a **Name** for the application (e.g., "Client App").
4. Select the **Supported account types** as `Accounts in this organizational directory only (Single tenant)`.
5. Leave the **Redirect URI** blank.
6. Click **Register** to create the application.

#### Configure the Client Application

1. After registering, go to the application's **Overview** page.
2. Under **Manage**, select **Certificates & Secrets** and create a `Client secret`. Copy the value as it will be obfuscated later.
3. Navigate to `API permissions`. Click on `Add a permission`.
4. Go to `My APIs`.
5. Select `HostAPI`.
6. Click on `Grant admin consent for Default Directory`.

#### Configure `appsettings.json`

Ensure you specify the necessary properties for `AzureAd`.

```json
"AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "mydomain.onmicrosoft.com",
    "TenantId": "19f43eb9-a915-45a5-9f1f-71091e8c8f1b",
    "ClientId": "325fbbd0-77e7-40f0-a25f-3857263fb265",
    "Audience": "api://325fbbd0-77e7-40f0-a25f-3857263fb265"
}
```

1. `Domain` can be found on the `Overview` page by copying the `Primary domain`.
2. `Instance` can be found by clicking on `Endpoints` in `App registrations`. Copy the domain from the `OAuth 2.0 authorization endpoint (v2)` URL, i.e., `https://login.microsoftonline.com`.
3. Retrieve `ClientId` and `TenantId` from `HostApi`'s `Overview` page.
4. `Audience` can be specified as this `api://<ClientId>`.

#### Obtain an Access Token for Testing

Perform an HTTP call to obtain the access token for testing via Swagger UI:

```http
POST https://login.microsoftonline.com/19f43eb9-a915-45a5-9f1f-71091e8c8f1b/oauth2/v2.0/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=a3f89381-bdcc-4e05-8ef8-c30b174ec7f8
&client_secret=kFG8Q~_TutR_YCYTfrGhjBr7ZeXazMDNXSrM8bLN
&scope=api://325fbbd0-77e7-40f0-a25f-3857263fb265/.default
```

Use the `client_id` and `client_secret` from your Client application. The `scope` should be the `Audience` from the `HostAPI` plus `/.default`.

You can then paste the resulting `access_token` into the Swagger UI under `Authorize`.

### AD B2C Authorization Code Flow

This setup is used for requests initiated by a user from an SPA, Mobile app, or web page and processed by an externally exposed API.

#### Registering the ConsumerFacingAPI Application

1. Sign into the [Azure portal](https://portal.azure.com).
2. Navigate to **Azure AD B2C** > **App registrations**.
3. Click **New registration**.
4. Enter a **Name** for the application (e.g., "ConsumerFacingAPI").
5. Select `Accounts in any identity provider or organizational directory (for authenticating users with user flows)` for **Supported account types**.
6. In `Redirect URI`, choose `Web` and specify your application's URL and port, e.g., `http://localhost:8080`.
7. Ensure `Grant admin consent to openid and offline_access permissions` is checked.
8. Click **Register** to create the application.

#### Configuring the ConsumerFacingAPI Application

1. After registering, go to the application's Overview page.
2. Under `Manage`, navigate to `Certificates & secrets` and create a `Client secret`. Copy the value.
3. Select `Expose an API` and click on `Add a scope`. Create the `Application ID URI`.
4. Provide a Scope name, e.g., `FrontEnd.Access`.
5. In the `Owners` section, add your Azure user.
6. Navigate to `API permissions`, click on `Add a permission`.
7. Go to `My APIs` and select `ConsumerFacingAPI`.
8. Click on `Grant admin consent for Default Directory`.

#### Setup User Flow

1. Navigate to the Azure AD B2C instance.
2. Locate `User flows` under `Policies`.
3. Click on `New user flow`.
4. Select `Sign up and sign in` and the `Recommended` version.
5. Click `Create`.
6. Name the flow `B2C_1_Consumer`.
7. Check `Email signup` under `Local accounts`.
8. Ensure `Display name` is selected under attributes (for collected and returned scenarios).
9. Click `Create`.

#### Configure `appsettings.json`

Configure the settings for `AzureAd`.

```json
"AzureAd": {
    "Instance": "https://myb2c.b2clogin.com",
    "Domain": "myb2c.onmicrosoft.com",
    "ClientId": "0feacea5-6401-45d0-9503-639bebf6ab73",
    "SignUpSignInPolicyId": "B2C_1_Consumer"
}
```

Retrieve `Domain`, `Instance`, `ClientId`, and `SignUpSignInPolicyId` from the Azure portal as previously described.

#### Obtain an Access Token for Testing

For Authorization Code flow testing, use tools like [Postman](https://www.postman.com/downloads), [Insomnia](https://insomnia.rest/download) or [Bruno](https://www.usebruno.com/downloads) (they will have an `Auth` section) with the following configurations:

- **Callback URL**: `Redirect URI` of the `ConsumerFacingAPI` application.
- **Authorization URL**: `OAuth 2.0 authorization endpoint (v2)` URL from `Endpoints`, replacing `<policy-name>` with `B2C_1_Consumer`.
- **Access Token URL**: `OAuth 2.0 token endpoint (v2)` URL from `Endpoints`, replacing `<policy-name>` with `B2C_1_Consumer`.
- **Client ID**: `Application (client) ID`.
- **Client Secret**: Secret value from `ConsumerFacingAPI`.
- **Scope**: Include `offline_access`, `openid`, and your API scope, like: `offline_access openid https://myb2c.onmicrosoft.com/0feacea5-6401-45d0-9503-639bebf6ab73/FrontEnd.Access`.

Click `Get Access Token`, sign in, and copy the `access_token` to paste into the `Authorize` field in Swagger UI for testing.
