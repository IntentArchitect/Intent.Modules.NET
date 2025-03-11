# Intent.Security.MSAL

The `Intent.Security.MSAL` module facilitates the integration of Azure Active Directory (Azure AD), EntraID, and Azure B2C to secure ASP.NET Core endpoints using the `Microsoft.Identity.Web` NuGet package.

## Prerequisites

- **Azure subscription**: You need an Azure subscription. If you don't have one, you can create a [free account](https://azure.microsoft.com/free/).

>[!NOTE]
> 
> If you were looking to make use of a different Open ID Connect Identity Provider solution, you will need to look at the `Intent.Security.JWT` module instead.

## Azure Entra ID / AD B2C Setup

### Client Credential Setup

This setup is used when a back-end facing API (`Client`) needs to invoke another downstream API (`HostAPI`) with a secure credential where a user initiated the request (indirectly) or a system-driven event initiated it.

The following instructions assume a `HostAPI` application exposing API endpoints and a `Client` application consuming them. Adjust these settings based on your specific needs.

#### Registering the Host Application

1. Sign in to the [Azure portal](https://portal.azure.com).
2. Navigate to **Microsoft Entra ID** > **App registrations**.
3. Click **New registration**.
4. Enter a **Name** for the application (e.g., "HostAPI").
5. Select the **Supported account types** as `Accounts in this organizational directory only (Single tenant)`.
6. Leave the **Redirect URI** blank.
7. Click **Register** to create the application.

#### Configure the Host Application

1. Go to the application's **Overview** page.
2. Under **Manage**, select **Owners** and add your Azure user as an owner.
3. Select **App roles** and click on `Create app role`.
4. Provide it a Display name: `Client Access Role`.
5. Set the `Allowed member types` to `Application`.
6. Se the Value to `Client.Access`.
7. Give it a display name and description.
8. Ensure that the `app role` is enabled by having the box checked at the end.
9. Click on `Apply`.
10. Select **Expose an API** and click on `Add` next to `Application ID URI`.
11. Keep the URI as-is (for tutorial purposes) and click `Save`.

#### Register the Client Application

1. Navigate to **Microsoft Entra ID** > **App registrations**.
2. Click **New registration**.
3. Enter a **Name** for the application (e.g., "Client App").
4. Select the **Supported account types** as `Accounts in this organizational directory only (Single tenant)`.
5. Leave the **Redirect URI** blank.
6. Click **Register** to create the application.

#### Configure the Client Application

1. After registering, go to the application's **Overview** page.
2. Under **Manage**, select **Certificates & Secrets** and create a `Client secret`. **COPY** the `Value` and store it somewhere as it will be obfuscated once you start navigating.
3. Navigate to `API permissions`. Click on `Add a permission`.
4. Go to `My APIs`.
5. Select `HostAPI`.
6. Select the `Client.Access` role while ensuring that `Application permissions` are also set.
7. Click on `Add permissoins`.
8. Click on `Grant admin consent for Default Directory`.

#### Configure `appsettings.json`

Your API service hosting the endpoints for consumption needs to be configured using your `Host Application` configuration from Microsoft Entra ID.
Ensure you specify the necessary properties for `AzureAd`.

```json
"AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "mydomain.onmicrosoft.com",
    "TenantId": "19f43eb9-a915-45a5-9f1f-71091e8c8f1b",
    "ClientId": "2825b206-549d-43c0-96c8-10a6a196d679",
    "Audience": "api://2825b206-549d-43c0-96c8-10a6a196d679"
}
```

1. `Domain` can be found in `Microsoft Entra ID` on its `Overview` page by copying the `Primary domain`.
2. `Instance` can be found by clicking on `Endpoints` in `App registrations`. Copy the `domain part` from the `OAuth 2.0 authorization endpoint (v2)` URI, i.e., `https://login.microsoftonline.com`.
3. Retrieve `ClientId` and `TenantId` from `HostApi`'s `Overview` page.
4. `Audience` is the `Application ID URI` from the `HostApi`'s `Expose an API` page.

#### Obtain an Access Token for Testing

For obtaining an Access Token, use tools like [Postman](https://www.postman.com/downloads), [Insomnia](https://insomnia.rest/download) or [Bruno](https://www.usebruno.com/downloads) (they will have an `Auth` section). Below is an example HTTP message to request an Access Token.

```http
POST https://login.microsoftonline.com/19f43eb9-a915-45a5-9f1f-71091e8c8f1b/oauth2/v2.0/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=d348ba62-a064-476a-bac4-41af07d60d5a
&client_secret=Fay8Q~1MQUg4ZWcBM_5HEOQq9oJVvWbfmruJTaZy
&scope=api://2825b206-549d-43c0-96c8-10a6a196d679/.default
```

The URI can be found by accessing the `Endpoints` page from `HostApi`'s `Overview` page and copying the `OAuth 2.0 token endpoint (v2)` URI.
The `client_id` is your `Client` application's Application Client ID and `client_secret` will be the secret you copied earlier. 
The `scope` should be the `Audience` from the `HostAPI` having the suffix of `/.default`.

Now you can test your token authorization by pasting the resulting `access_token` into the Swagger UI under `Authorize` (if you're using Swagger).

### AD B2C Authorization Code Flow

This setup is used for requests initiated by a user from an SPA, Mobile app, or web page and processed by an externally exposed API (`ConsumerAPI`).

#### Registering the ConsumerAPI Application

1. Sign in to the [Azure portal](https://portal.azure.com).
2. Navigate to **Azure AD B2C** > **App registrations**.
3. Click **New registration**.
4. Enter a **Name** for the application (e.g., "ConsumerAPI").
5. Select `Accounts in any identity provider or organizational directory (for authenticating users with user flows)` for **Supported account types**.
6. In `Redirect URI`, choose `Web` and specify your application's URL and port, e.g., `http://localhost:8080`.
7. Ensure `Grant admin consent to openid and offline_access permissions` is checked.
8. Click **Register** to create the application.

#### Configuring the ConsumerAPI Application

1. After registering, go to the application's Overview page.
2. Under `Manage`, navigate to `Certificates & secrets` and create a `Client secret`. **COPY** the `Value` and store it somewhere as it will be obfuscated once you start navigating.
3. Select `Expose an API` and click on `Add a scope`. Keep the `Application ID URI` as-is and click on `Save and continue` (for tutorial purposes).
4. Provide a Scope name, e.g., `FrontEnd.Access`.
5. Give it a display name and description.
6. Click on `Add scope`.
7. In the `Owners` section, add your Azure user.
8. Navigate to `API permissions`, click on `Add a permission`.
9. Go to `My APIs` and select `ConsumerAPI`. Select your `FrontEnd.Access` permission and click on `Add permissions`.
10. Click on `Grant admin consent for Default Directory`.

#### Setup User Flow

1. Navigate to the Azure AD B2C instance.
2. Locate `User flows` under `Policies`.
3. Click on `New user flow`.
4. Select `Sign up and sign in` and the `Recommended` version.
5. Click `Create`.
6. Name the flow `B2C_1_Consumer`.
7. Check `Email signup` under `Local accounts`.
8. In the `User attributes and token claims` section, ensure `Display name` is selected under attributes (for collected and returned scenarios).
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

1. `Domain` can be found in `Azure AD B2C` on its `Overview` page by copying the `Primary domain`.
2. `Instance` can be found by clicking on `Endpoints` in `App registrations`. Copy the `domain part` from the `OAuth 2.0 authorization endpoint (v2)` URI, i.e., `https://myb2c.b2clogin.com`.
3. Retrieve `ClientId` and `TenantId` from `ConsumerAPI`'s `Overview` page.
4. Use the `B2C_1_Consumer` flow created earlier for the `SignUpSignInPolicyId` setting.

#### Obtain an Access Token for Testing

For Authorization Code flow testing, use tools like [Postman](https://www.postman.com/downloads), [Insomnia](https://insomnia.rest/download) or [Bruno](https://www.usebruno.com/downloads) (they will have an `Auth` section) with the following configurations:

- **Authorization URL**: `OAuth 2.0 authorization endpoint (v2)` URL from `Endpoints`, replacing `<policy-name>` with `B2C_1_Consumer`.
- **Access Token URL**: `OAuth 2.0 token endpoint (v2)` URL from `Endpoints`, replacing `<policy-name>` with `B2C_1_Consumer`.
- **Client ID**: `Application (client) ID`.
- **Client Secret**: Will be the secret you copied earlier.
- **Callback URL**: `Redirect URI` of the `ConsumerAPI` application.
- **Scope**: Include `offline_access`, `openid`, and your API scope (inside `Expose an API` you can copy the created scope name), like: `offline_access openid https://myb2c.onmicrosoft.com/0feacea5-6401-45d0-9503-639bebf6ab73/FrontEnd.Access`.

Click `Get Access Token`, sign in, and copy the `access_token` to paste into the `Authorize` field in Swagger UI for testing.
