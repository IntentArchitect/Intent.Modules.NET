# Intent.Modules.Security.MSAL

The Intent.Security.MSAL module facilitates the integration of Azure Active Directory (Azure AD), EntraID, and Azure B2C to secure ASP.NET Core endpoints using the `Microsoft.Identity.Web` NuGet package.

## Prerequisites

- Azure subscription: You need an Azure subscription. If you don't have one, you can create a [free account](https://azure.microsoft.com/free/).

## Azure Entra ID / AD B2C Setup

### Client Credential Setup

This setup is used when you have one application that needs to invoke endpoints from another application. This may be caused indirectly due to a user request or system generated event.

The following instructions will assume a HostAPI application exposing API endpoints and a Client application consuming them.
The overall configuration will only set the necessary settings for this to work. You will need to interpolate how to adjust the settings based on your needs.

#### Registering the Host Application

1. Sign into the [Azure portal](https://portal.azure.com).
2. Navigate to **Azure Active Directory** > **App registrations**.
3. Click **New registration**.
4. Enter a **Name** for the application (e.g., "HostAPI").
5. Select the **Supported account types**. In the example case choose `Accounts in this organizational directory only (Single tenant)`.
6. No need for **Redirect URI** so leave blank.
7. Click **Register** to create the application.

#### Configure the Host Application

1. After registering, go to the application's **Overview** page.
2. Under **Manage**, select **Owners** and add your Azure user as an owner (please use the appropriate account).
3. Now select **expose an API** and click on `Add a scope`. Have it create the `Application ID URI`.
4. Provide a Scope name, e.g. `Client.Access`.
5. Set the consent to `Admins only`.
6. Populate the display name and description.
7. Ensure its `Enabled` and click on `Add scope`.

#### Register the Client Application

1. Navigate to **Azure Active Directory** > **App registrations**.
2. Click **New registration**.
3. Enter a **Name** for the application (e.g., "Client App").
4. Select the **Supported account types**. In the example case choose `Accounts in this organizational directory only (Single tenant)`.
5. No need for **Redirect URI** so leave blank.
6. Click **Register** to create the application.

#### Configure the Client Application

1. After registering, go to the application's **Overview** page.
2. Under **Manage**, select **Certificates & Secrets** and create a `Client secret`. Copy the value when done as navigating away will obfuscate it for security reasons.
3. Navigate to `API permissions`. Click on `Add a permission`.
4. Go to `My APIs`.
5. Select the app we created before in the list `HostAPI`.
6. Click on `Grant admin consent for Default Directory`.

#### Configure your appsettings.json

Ensure you specify the following properties for `AzureAd`.

```json
"AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "mydomain.onmicrosoft.com",
    "TenantId": "19f43eb9-a915-45a5-9f1f-71091e8c8f1b",
    "ClientId": "325fbbd0-77e7-40f0-a25f-3857263fb265",
    "Audience": "api://325fbbd0-77e7-40f0-a25f-3857263fb265"
}
```

1. `Domain` can be found by navigating to the `Overview` page and copying the `Primary domain`.
2. `Instance` can be found by navigating to the `App registrations` and clicking on `Endpoints`. Copy the domain from the URL located in `OAuth 2.0 authorization endpoint (v2)`, i.e. `https://login.microsoftonline.com`.
3. Navigate to the `HostApi` application in `App registrations` and opening the `Overview` page.
   - Copy `Application (client) ID` for the `ClientId`.
   - Copy `Directory (tenant) ID` for the `TenantId`.
   - Set the `Audience` to `api://<Client-Id>`.
   - Set the `Scopes` to `api://<Client-Id>/.default`.
   - Paste in your `ClientSecret`.

#### Obtain an access token for testing

Perform an HTTP call to obtain the access token that will be used to test that the authorization is working using the Swagger UI:

```http request
POST https://login.microsoftonline.com/19f43eb9-a915-45a5-9f1f-71091e8c8f1b/oauth2/v2.0/token
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials
&client_id=a3f89381-bdcc-4e05-8ef8-c30b174ec7f8
&client_secret=kFG8Q~_TutR_YCYTfrGhjBr7ZeXazMDNXSrM8bLN
&scope=api://325fbbd0-77e7-40f0-a25f-3857263fb265/.default
```

Navigate to the Client application in `App registrations` and opening the Overview page.
- Copy the `Application (client) ID` for the `client_id`.
- Paste in the client secret for `client_secret`.
- Specify the `scope` to be the `Audience` from the `HostAPI` application along with the `/.default` suffix.

This will return with a payload containing the `access_token` which you can paste into your Swagger UI when you click on `Authorize` and pasting it in the `Value` field.

### AD B2C Authorization Code flow

This setup is used when you have a request being initiated by a user from an SPA, Mobile app or Web page and being processed by an externally exposed API.

The following instructions will assume a ConsumerFacingAPI application hosting an API and authenticates the user.
The overall configuration will only set the necessary settings for this to work. You will need to interpolate how to adjust the settings based on your needs.

For more use case documentation and examples on configuring AD B2C itself, have a look at [Woodgrove Groceries demo](https://woodgrovedemo.com/#) and click on the `Select a use case` button.

#### Registering the ConsumerFacingAPI Application

1. Sign into the [Azure portal](https://portal.azure.com).
2. Navigate to **Azure AD B2C** > **App registrations**.
3. Click **New registration**.
4. Enter a **Name** for the application (e.g., "ConsumerFacingAPI").
5. Select the **Supported account types**. In the example case choose `Accounts in any identity provider or organizational directory (for authenticating users with user flows)`.
6. In the `Redirect URI` section, choose `Web` and specify your application's URL and PORT `http://localhost:8080`.
7. Ensure `Grant admin consent to openid and offline_access permissions` is checked.
8. Click **Register** to create the application.

#### Configuring the ConsumerFacingAPI Application

1. After registering, go to the application's Overview page.
2. Under Manage, go to `Certificated & secrets` and click `New client secret`. Copy the value when done as this will be the secret you will use later.
3. Now select **expose an API** and click on `Add a scope`. Have it create the `Application ID URI`.
4. Provide a Scope name, e.g. `FrontEnd.Access`.
5. Select **Owners** and add your Azure user as an owner (please use the appropriate account).
6. Navigate to `API permissions`. Click on `Add a permission`.
7. Go to `My APIs`.
8. Select the app we created before in the list `ConsumerFacingAPI`.
9. Click on `Grant admin consent for Default Directory`.

#### Setup User Flow

1. Navigate to the Azure AD B2C instance.
2. Locate the `User flows` under `Policies`.
3. Click on `New user flow`.
4. Select `Sign up and sign in` and make sure `Recommended` version is selected.
5. Click on Create.
6. Give the flow the name `B2C_1_Consumer`.
7. Check `Email signup` under `Local accounts`.
8. Ensure that the `Display name` is collected and returned under attributes (to demo user details).
9. Scroll to the bottom and click on `Create`.

#### Configure your appsettings.json

Ensure you specify the following properties for `AzureAd`.

```json
"AzureAd": {
    "Instance": "https://myb2c.b2clogin.com",
    "Domain": "myb2c.onmicrosoft.com",
    "ClientId": "0feacea5-6401-45d0-9503-639bebf6ab73",
    "SignUpSignInPolicyId": "B2C_1_Consumer"
}
```

1. `Domain` can be found by navigating to the `Overview` page and copying the `Primary domain`.
2. `Instance` can be found by navigating to the `App registrations` and clicking on `Endpoints`. Copy the domain from the URL located in `OAuth 2.0 authorization endpoint (v2)`, i.e. `https://myb2c.b2clogin.com`.
3. Navigate to the `HostApi` application in `App registrations` and opening the `Overview` page.
   - Copy `Application (client) ID` for the `ClientId`.
   - Paste in `SignUpSignInPolicyId` the name for your user flow e.g. `B2C_1_Consumer`.

#### Obtain an access token for testing

Getting hold of the access token for testing using a HTTP request on Authorization Code flow will be complex. Rather make use of a tool like Postman or Bruno where you can set the Authentication details for acquiring a token. You will need to choose the `OAuth 2.0` scheme, `Authorization Code` grant type and then populate the following fields:

- `Callback URL` - This is the `Redirect URI` when you created the `ConsumerFacingAPI` application.
- `Authorization URL` - In the ConsumerFacingAPI app (under App registrations) there is an `Endpoints` button on the Overview page. Choose the URL under `Azure AD B2C OAuth 2.0 authorization endpoint (v2)` and substitute `<policy-name>` with `B2C_1_Consumer`.
- `Access Token URL` - In the ConsumerFacingAPI app (under App registrations) there is an `Endpoints` button on the Overview page. Choose the URL under `Azure AD B2C OAuth 2.0 token endpoint (v2)` and substitute `<policy-name>` with `B2C_1_Consumer`.
- `Client ID` - Paste in the `Application (client) ID`.
- `Client Secret` - Paste in the secret value when you created it for ConsumerFacingAPI.
- `Scope` - You will need to supply `offline_access` and `openid` and your scope that you've created in `Expose an API`. Example: `offline_access openid https://myb2c.onmicrosoft.com/0feacea5-6401-45d0-9503-639bebf6ab73/FrontEnd.Access`.
- `State` - Give it a random value.

Click on `Get Access Token` and you'll see it presenting to you a web browser where you can sign up / sign in with an account. Once an account is created / logged in, the browser will close and a response payload will appear featuring an `access_token`, `id_token` and `refresh_token`.
Copy the `access_token` and paste into your Swagger UI when you click on `Authorize` and pasting it in the `Value` field.