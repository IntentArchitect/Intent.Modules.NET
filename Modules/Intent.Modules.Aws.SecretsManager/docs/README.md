# Intent.Aws.SecretsManager

## Overview

The AWS Secrets Manager module integrates Secrets Manager with .NET `IConfiguration`, so secrets can be read just like values from appsettings.json. Typical use cases include passwords, API keys, and connection strings.

## Configuration

To connect to Secrets Manager, include the following configuration in your `appsettings.json` file (the module will automatically scaffold this)

```json
"SecretsManager": {
    "Enabled": true,
    "Secrets": [
      {
        "Region": "us-east-1",
        "SecretName": "demo/sample/secrets"
      }
    ]
  }
```

### Configuration Parameters

- **Enabled**: Determines whether the Secrets Manager integration is active.
- **Secrets**: One or more secrets to load.
  - **Region**: AWS region where the secret is stored (e.g., us-east-1).
  - **SecretName**: The name/ARN of the secret.

### Authentication

In addition to the above, valid credentials need to be configured to authenticate with AWS. The credential and profile resolution, and subsequent authentication happens automatically by the underlying AWS SDK - [details available here.](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/creds-assign.html)

Two typical methods are (refer to the AWS documentation for detailed information on the authentication methods):

- **AWS IAM Identity Center (SSO) / Profiles** (recommended for local dev)
  Configure a profile, run `aws sso login`, then run the app with `AWS_PROFILE=<your-profile>`.
- **Environment variables** (common in CI/containers)
  `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, and `AWS_SESSION_TOKEN` (required for temporary creds). Also set `AWS_REGION` or specify Region in config.

## Accessing Secrets

Given this JSON stored as a secret:

```json
{
  "AccessKey" : "123456789",
  "ConnectionStrings" : [
    {
      "Name": "dbOne",
      "ConnectionString" : "connection-string-one"
    },
    {
      "Name": "dbTwo",
      "ConnectionString" : "connection-string-two"
    }
  ]
}
```

The values can be read via `IConfiguration`

``` csharp
// Scalars
var accessKey = configuration["AccessKey"]; // "123456789"

// Arrays (indexer-style)
var firstConn = configuration["ConnectionStrings:0:ConnectionString"]; // "connection-string-one"

// Strongly typed binding
var conns = configuration.GetSection("ConnectionStrings").Get<List<DbConn>>();
```

Where

``` csharp
public sealed class DbConn
{
    public string Name { get; set; } = default!;
    public string ConnectionString { get; set; } = default!;
}
```
