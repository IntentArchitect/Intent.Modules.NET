# Intent.AspNetCore.Cors

This module provides patterns for [enabling Cross-Origin Requests (CORS) in ASP.NET Core](https://learn.microsoft.com/aspnet/core/security/cors).

During application startup `services.AddCors(...)` and `app.UseCors()` are called to load a default policy. In order to be able to change the CORS configuration without having to recompile your application and also have its policies be configured differently per deployment environment, this module generates code which reads the configuration policies from the `CorsPolicies` section of your ASP.NET Core application configuration (such as specified in `appsettings.json`).

By default the following configuration is added to your `appsettings.json` file:

```json
{
  "CorsPolicies": {
    "Default": {
      "Origins": [
        "*"
      ],
      "Methods": [
        "*"
      ],
      "Headers": [
        "*"
      ]
    }
  }
}
```

As this default configuration is completely open, it's advised to update it to be more restrictive based on your application URLs. The configuration supports optional `Default` and `Named` sections, here is an example of a more complex configuration as JSON:

```json
{
  "CorsPolicies": {
    "Default": {
      "Origins": [
        "https://application1.example.com/",
        "https://application2.example.com/"
      ],
      "Methods": [
        ["POST", "GET"]
      ],
      "Headers": [
        "*"
      ],
      "ExposedHeaders": [
        "*"
      ],
      "AllowCredentials": true
    },
    "Named": {
      "CustomPolicy1": {
        "Origins": [
          "https://application3.example.com/"
        ],
        "Methods": [
          ["GET"]
        ],
        "Headers": [
          "*"
        ],
        "ExposedHeaders": [
          "Content-Encoding"
        ]
      },
      "CustomPolicy2": {
        "Origins": [
          "https://application4.example.com/"
        ],
        "Methods": [
          ["PUT"]
        ],
        "Headers": [
          "*"
        ],
        "PreflightMaxAge": "00:01:00"
      }
    }
  }
}
```

For reference, the configuration is deserialized into the following C# classes so will need to match their structure:

```csharp
public class CorsPolicies
{
    public PolicyOptions? Default { get; set; }
    public Dictionary<string, PolicyOptions>? Named { get; set; }
}

public class PolicyOptions
{
    public string[]? Origins { get; set; }
    public string[]? Methods { get; set; }
    public string[]? Headers { get; set; }
    public string[]? ExposedHeaders { get; set; }
    public bool AllowCredentials { get; set; }
    public TimeSpan? PreflightMaxAge { get; set; }
}
```
