# proxy.version

The API version a front end's proxy operation asks for, against the version of the backend Operation it calls: the `api-version` its request's query string sends, else the version its `HttpClient` registration adds to every request. The kind is `whenSet`: a backend Operation with no version states none. The values compare as versions, so `2` is `2.0`.

The kind is `later`. eShopCustom's proxies name their backend Operation with its own Backend Operation stereotype, so these cases' house pack gives the vocabulary; reading the version sent is the stock rule's.

## eshop-query-version

eShopCustom's catalog proxy reads an item with `?api-version=2`, and the backend Operation's Api Version Settings applies it to version `2.0`.

```json case
{ "phase": "later", "kinds": ["proxy.version"], "traces": ["ESC-FE-62"], "requirements": ["R2", "R28"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "kinds": [
    { "kind": "proxy.version", "label": "proxy API version", "valueType": "text", "comparer": "version", "stated": "whenSet", "existence": false }
  ],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "proxy.version",
          "for": { "element": "Operation", "where": "parent.specialization == 'Service Proxy' && hasStereotype('Backend Operation')" },
          "value": "join(map(stereotype('Backend Operation').Operation.stereotype('Api Version Settings')['Applicable Versions'], name), '')",
          "bindings": { "Proxy": "parent.name", "Operation": "name" },
          "own": ["Proxy", "Operation"],
          "source": "the Applicable Versions of the backend Operation its Backend Operation stereotype names"
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "catalog-api", "name": "Catalog.API",
        "elements": [
          { "id": "catalog-versions", "type": "Api Version", "name": "Catalog",
            "children": [
              { "id": "catalog-v2", "type": "Version", "name": "2.0" }
            ] },
          { "id": "catalog-api-service", "type": "Service", "name": "CatalogApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/catalog" } },
            "children": [
              { "id": "get-item-by-id", "type": "Operation", "name": "GetItemById",
                "stereotypes": {
                  "Http Settings": { "Verb": "GET", "Route": "items/{id}" },
                  "Api Version Settings": { "Applicable Versions": [{ "ref": "catalog-v2" }] }
                },
                "children": [
                  { "id": "get-item-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "User Interface": {
      "packages": [{
        "id": "webappcomponents", "name": "WebAppComponents",
        "elements": [
          { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService",
            "children": [
              { "id": "catalog-service-get-catalog-item", "type": "Operation", "name": "GetCatalogItem",
                "stereotypes": { "Backend Operation": { "Operation": { "ref": "get-item-by-id" } } },
                "children": [
                  { "id": "catalog-service-get-catalog-item-id", "type": "Parameter", "name": "id", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;
using eShop.WebAppComponents.Catalog;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "api/catalog/";

    public Task<CatalogItem?> GetCatalogItem(int id)
    {
        var uri = $"{remoteServiceBaseUrl}items/{id}?api-version=2";
        return httpClient.GetFromJsonAsync<CatalogItem>(uri);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.version", "element": "catalog-service-get-catalog-item", "outcome": "verified",
      "expected": "2.0", "actual": "2", "at": "src/WebAppComponents/Services/CatalogService.cs:13" }
  ]
}
```

## csharp-standard-one-of-the-versions

The standard User Interface designer (2.0.0): a Proxy Operation's Call Service Operation Action targets a backend Operation whose service applies two API versions, `1.0` and `2.0`, and the operation none of its own. The request asks for one of them, so it holds; `GetCatalogTypes` asks for `3.0`, which the service doesn't apply.

```json case
{ "phase": "later", "kinds": ["proxy.version"], "requirements": ["R2", "R14", "R38"], "packs": ["intent.core", "intent.ui"], "language": "csharp" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "webappcomponents", "name": "WebAppComponents", "type": "User Interface Package",
        "elements": [
          { "id": "catalog-proxy", "type": "Proxy", "name": "CatalogService", "children": [
            { "id": "catalog-proxy-get-brands", "type": "Proxy Operation", "name": "GetBrands" },
            { "id": "catalog-proxy-get-types", "type": "Proxy Operation", "name": "GetCatalogTypes" }
          ] }
        ]
      }],
      "associations": [
        { "id": "get-brands-calls-backend", "type": "Call Service Operation Action", "source": { "element": "catalog-proxy-get-brands" }, "target": { "element": "catalog-list-brands" } },
        { "id": "get-types-calls-backend", "type": "Call Service Operation Action", "source": { "element": "catalog-proxy-get-types" }, "target": { "element": "catalog-list-types" } }
      ]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  },
  "references": {
    "Services": {
      "packages": [{
        "id": "catalog-api", "name": "Catalog.API",
        "elements": [
          { "id": "catalog-versions", "type": "Api Version", "name": "Catalog", "children": [
            { "id": "catalog-v1", "type": "Version", "name": "1.0" },
            { "id": "catalog-v2", "type": "Version", "name": "2.0" }
          ] },
          { "id": "catalog-api-service", "type": "Service", "name": "CatalogApi",
            "stereotypes": {
              "Http Service Settings": { "Route": "api/catalog" },
              "Api Version Settings": { "Applicable Versions": [{ "ref": "catalog-v1" }, { "ref": "catalog-v2" }] }
            },
            "children": [
              { "id": "catalog-list-brands", "type": "Operation", "name": "ListItemBrands", "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "catalogbrands" } } },
              { "id": "catalog-list-types", "type": "Operation", "name": "ListItemTypes", "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "catalogtypes" } } }
            ] }
        ]
      }]
    }
  }
}
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient)
{
    public Task<string[]?> GetBrands() => httpClient.GetFromJsonAsync<string[]>("api/catalog/catalogbrands?api-version=2.0");

    public Task<string[]?> GetCatalogTypes() => httpClient.GetFromJsonAsync<string[]>("api/catalog/catalogtypes?api-version=3.0");
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.version", "element": "get-brands-calls-backend", "outcome": "verified", "expected": "1.0, 2.0", "actual": "2.0", "at": "src/WebAppComponents/Services/CatalogService.cs:7" },
    { "kind": "proxy.version", "element": "get-types-calls-backend", "outcome": "contradicted", "expected": "1.0, 2.0", "actual": "3.0", "at": "src/WebAppComponents/Services/CatalogService.cs:9" }
  ],
  "findings": [
    { "rule": "claim/proxy.version", "severity": "error", "subject": "get-types-calls-backend" }
  ]
}
```

## eshop-registration-version

The request sends no `api-version`; the proxy's `AddHttpClient<CatalogService>` registration, in another file, adds version 3 to every request it makes, and the backend Operation is version `2.0`.

```json case
{ "phase": "later", "base": "eshop-query-version", "kinds": ["proxy.version"], "traces": ["ESC-FE-62"], "requirements": ["R2", "R14"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using System.Net.Http.Json;
using eShop.WebAppComponents.Catalog;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "api/catalog/";

    public Task<CatalogItem?> GetCatalogItem(int id)
    {
        var uri = $"{remoteServiceBaseUrl}items/{id}";
        return httpClient.GetFromJsonAsync<CatalogItem>(uri);
    }
}
```

```csharp file src/WebApp/Extensions/Extensions.cs
using eShop.WebAppComponents.Services;

namespace eShop.WebApp.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<CatalogService>(o => o.BaseAddress = new("http://catalog-api"))
            .AddApiVersion(3.0);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.version", "element": "catalog-service-get-catalog-item", "outcome": "contradicted", "expected": "2.0", "actual": "3.0" }
  ],
  "findings": [
    { "rule": "claim/proxy.version", "severity": "error", "subject": "catalog-service-get-catalog-item" }
  ]
}
```
