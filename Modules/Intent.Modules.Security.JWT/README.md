# Intent.Security.JWT

The `Intent.Security.JWT` module facilitates the integration of an Open ID Connect Identity Provider to secure ASP.NET Core endpoints through OAuth using the `Microsoft.AspNetCore.Authentication.JwtBearer` NuGet package.

## Prerequisites

- **Open ID Connect Identity Provider**: You will need a compliant Open ID Connect Identity Provider in order to make use of this module. There are various providers to choose from:
  - Cloud Identity Providers
    - [auth0.com](https://auth0.com/)
    - [okta.com](https://www.okta.com/)
  - Self Hosted Providers
    - [Keycloak](https://www.keycloak.org/)
    - [Hashicorp Vault](https://developer.hashicorp.com/vault/tutorials/auth-methods/oidc-auth)
  - Framework Providers
    - [Duende: Identity Server](https://duendesoftware.com/products/identityserver)
    - [OpenIddict](https://documentation.openiddict.com/)

>[!NOTE]
> 
> If you were looking to make use of an Azure EntraID or Azure AD B2C solution, you will need to look at the `Intent.Security.MSAL` module instead.

## Getting Started

To understand how to set up your API authorization, it will be done in a tutorial fashion to help get the overall picture.
This guide provides a step-by-step walkthrough for setting up a locally hosted [Keycloak](https://www.keycloak.org/) server and integrating it with an ASP.NET Core API to authorize JWT tokens which is useful for testing API Authorization locally on a developer machine.

>[!NOTE]
> 
>This is not limited only to Keycloak. Keycloak only serves as a test platform for local development. The concepts described here are universal to OAuth and Open ID so it can be applied to any other Identity Provider that supports those protocols.

## Step 1: Install Keycloak

### Install Keycloak without SSL

To run a Keycloak instance without SSL (simplest to get up and running), execute the following command:

```powershell
docker run --name keycloak -p 8080:8080 -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin quay.io/keycloak/keycloak start-dev
```

This will host Keycloak on `localhost:8080`.

### Install Keycloak with SSL

For a more secure setup with SSL (and keeping close to production based configuration), we'll use `dotnet dev-certs` CLI command to create/obtain a trusted `localhost` certificate that can be used for Keycloak.

- If you haven't created a trusted `localhost` certificate for your development machine, run the following command:

```powershell
dotnet dev-certs https
```

- Obtain the public certificate and private key for Keycloak:

```powershell
dotnet dev-certs https -ep C:\DockerCerts\server.pem.crt -np --trust --format Pem
```

- Install and run Keycloak with the SSL certificates:

```powershell
docker run --name keycloak -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin -e KC_HTTPS_CERTIFICATE_FILE=/opt/keycloak/conf/server.pem.crt -e KC_HTTPS_CERTIFICATE_KEY_FILE=/opt/keycloak/conf/server.pem.key -v C:\DockerCerts\server.pem.crt:/opt/keycloak/conf/server.pem.crt -v C:\DockerCerts\server.pem.key:/opt/keycloak/conf/server.pem.key -p 8443:8443 quay.io/keycloak/keycloak start-dev --verbose
```

This will host Keycloak on `localhost:8443`.

## Step 2: Configure Keycloak

### Access Keycloak Admin Portal

- Open your browser and go to [http://localhost:8080](http://localhost:8080) / https://localhost:8443.
- Log in with:
    - **Username:** admin
    - **Password:** admin

### Create a Realm

- Click on **Add Realm** and create a new realm named `test-realm`.
- Switch to the newly created realm.

### Note on Realm Endpoints

- The endpoint for your realm configuration will be something like: [http://localhost:8080/realms/test-realm/.well-known/openid-configuration](http://localhost:8080/realms/test-realm/.well-known/openid-configuration) / https://localhost:8443/realms/test-realm/.well-known/openid-configuration.

## Step 3: Create a Client

There are two options for creating a client: a **Web Login Client** and a **Client Secret**. Choose the option that best fits your needs.
### Option 1: Create a Web Login Client

#### Create a User

- Navigate to **Users** and click on **Add User**.
    - **Username:** testuser
    - **Email:** testuser@test.com
    - Ensure **Email Verified** is checked.
- Click on **Save**.

#### Set User Credentials

- Go to the **Credentials** tab for the created user.
    - **Password:** 123
    - **Temporary:** Off
- Click on **Set Password**.

#### Test User Login

- Go to [http://localhost:8080/realms/test-realm/account](http://localhost:8080/realms/test-realm/account) / https://localhost:8443/realms/test-realm/account
- Log in with:
    - **Username:** testuser
    - **Password:** 123
    - Log out again for the next section.

#### Create a Client

- Navigate to **Clients** and click on **Create client**.
    - **Client Type:** OpenID Connect
    - **Client ID:** public-client
    - Click **Next**.
- Set the following configurations:
    - **Client Authentication:** Off
    - **Authentication Flow:** Standard Flow
    - **Direct Access Grants:** Off
- Click **Next**.
- Set **Valid Redirect URIs** to:
    - `https://www.keycloak.org/app/*`
- Click **Save**.

#### Test Login with Keycloak Web App

- Ensure you are logged out from the realm.
- Open [https://www.keycloak.org/app/](https://www.keycloak.org/app/).
- Enter the following details:
    - **URL:** http://localhost:8080 / https://localhost:8443
    - **Realm:** test-realm
    - **Client:** public-client
- Sign in with `testuser`.
- We have verified that it is working.

### Option 2: Create a Client-Credential Client

- Navigate to **Clients** and click on **Create client**.
    - **Client Type:** OpenID Connect
    - **Client ID:** confidential-client
    - Click **Next**.
- Set the following configurations:
    - **Client Authentication:** On
    - **Authorization Enabled:** Off
    - **Standard Flow Enabled:** Off
    - **Direct access grants Enabled:** off.
    - **Service Accounts Roles Enabled:** On
- Click **Save**.
- After saving, navigate to the **Client** settings and click on the **Credentials** tab to copy the **Client Secret**.

## Step 4: Create a scope for your App
### Setting Audience for the Scope

Secure your API application resource so that only tokens that have the `api` audience are allowed to access it.

- Create a client scope by navigating to `Client scopes > Create client scope` (if you already created `api` just select it and proceed to `Mappers`).
- Give it a name of `api`.
- Click on Save.
- Click on the tab `Mappers` and click on `Configure new mapper` (or `By configuration` if there is already a mapper present).
- Select `Audience`.
- Give the mapping a name `public-client-api` (for a Web Login Client) or `confidential-client-api` (for a Client Credential Client).
- We don't want to set our Client Id as our audience so leave the `Include Client Audience` as blank and let's generalize it by making it `api`.
- Click on Save.
- Go to `Clients > public-client / confidential-client > Client scopes (tab) > Add client scope > select api`
- Choose Optional.

> [!NOTE]
> 
> If you have previewed the JWT token for your configured Client in visualizer like [jwt.io](https://jwt.io) you would have seen that the `aud` claim is set to `account`. This is to allow the user to access their profile information on Keycloak. This section covered how to extend the allowed audiences for accessing other secured API resources.

## Step 5: Integrate with ASP.NET Core

### Configure `appsettings.json`

Add the following configurations to your `appsettings.json` file:

#### Non SSL Option

```json
"Security.Bearer": {
  "Authority": "http://localhost:8080/realms/test-realm",
  "Audience": "api"
}
```

#### SSL Option

```json
"Security.Bearer": {
  "Authority": "https://localhost:8443/realms/test-realm",
  "Audience": "api"
}
```

### Update Startup Configuration for Non SSL option

In `Startup.cs` or `Program.cs` (depending on your setup), configure JWT Bearer authentication:

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)  
    .AddJwtBearer(  
        JwtBearerDefaults.AuthenticationScheme,  
        options =>  
        {  
            options.Authority = configuration.GetSection("Security.Bearer:Authority").Get<string>();  
            options.Audience = configuration.GetSection("Security.Bearer:Audience").Get<string>();  
  
            options.TokenValidationParameters.RoleClaimType = "role";  
            options.SaveToken = true;
            //IntentIgnore  
            options.RequireHttpsMetadata = false; // Not recommended for Production use!
        });
```

This is so that restrictions are relaxed for JWT Authorization security over normal HTTP call.

## Test your API Authorization

For the following, use an API Client like: [Postman](https://www.postman.com/downloads), [Insomnia](https://insomnia.rest/download) or [Bruno](https://www.usebruno.com/downloads)
### Test Login with an API Client for a Web Login client

- Create a new request and open the **Auth** tab.
- Set the following details:
    - **Auth Type:** OAuth 2.0
    - **Grant Type:** Authorization Code
    - **Authorization URL:** http://localhost:8080/realms/test-realm/protocol/openid-connect/auth / https://localhost:8443/realms/test-realm/protocol/openid-connect/auth
    - **Access Token URL:** http://localhost:8080/realms/test-realm/protocol/openid-connect/token / https://localhost:8443/realms/test-realm/protocol/openid-connect/token
    - **Client ID:** public-client
    - **Redirect URL:** https://www.keycloak.org/app/
    - **Scope:** openid email profile api
- Fetch tokens.
- Now you have the access token to test your API in the Swagger UI.

### Test Login with an API Client for a Client-Credential client

- Create a new request and open the **Auth** tab.
- Set the following details:
    - **Auth Type:** OAuth 2.0
    - **Grant Type:** Client Credentials
    - **Access Token URL:** http://localhost:8080/realms/test-realm/protocol/openid-connect/token / https://localhost:8443/realms/test-realm/protocol/openid-connect/token
    - **Client ID:** confidential-client
    - **Client Secret:** Paste in the Client Credential Secret that you copied before.
    - **Scope:** openid email profile api
- Fetch tokens.
- Now you have the access token to test your API in the Swagger UI.
