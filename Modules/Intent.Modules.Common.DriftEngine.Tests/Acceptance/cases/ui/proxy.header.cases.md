# proxy.header

The headers a front end's proxy operation's request sets, against the backend Operation's Parameters bound from a header. The kind is `whenSet`, compared `subset`: every header the backend Operation reads must be sent. A header the request sets that the backend doesn't read isn't drift (principle 1).

The kind is `later`. eShopCustom's proxies name their backend Operation with its own Backend Operation stereotype, so these cases' house pack gives the vocabulary; reading the headers the request message sets is the stock rule's.

## eshop-request-header

eShopCustom's ordering proxy builds an `HttpRequestMessage` and adds `x-requestid`, the header the backend's `CreateOrderAsync` reads its `requestId` from.

```json case
{ "phase": "later", "kinds": ["proxy.header"], "traces": ["ESC-FE-63"], "requirements": ["R2", "R28"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshopcustom",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "kinds": [
    { "kind": "proxy.header", "label": "proxy header", "valueType": "set", "comparer": "subset", "stated": "whenSet", "existence": false }
  ],
  "vocabulary": {
    "User Interface": {
      "entries": [
        {
          "kind": "proxy.header",
          "for": { "element": "Operation", "where": "parent.specialization == 'Service Proxy' && hasStereotype('Backend Operation')" },
          "value": "join(map(where(children('Parameter', stereotype('Backend Operation').Operation), stereotype('Parameter Settings').Source == 'From Header'), stereotype('Parameter Settings')['Header Name'] ?? name), ', ')",
          "bindings": { "Proxy": "parent.name", "Operation": "name" },
          "own": ["Proxy", "Operation"],
          "source": "the From Header Parameters of the backend Operation its Backend Operation stereotype names"
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
        "id": "ordering-api", "name": "Ordering.API",
        "elements": [
          { "id": "orders-api", "type": "Service", "name": "OrdersApi",
            "stereotypes": { "Http Service Settings": { "Route": "api/orders" } },
            "children": [
              { "id": "create-order", "type": "Operation", "name": "CreateOrderAsync",
                "stereotypes": { "Http Settings": { "Verb": "POST" } },
                "children": [
                  { "id": "create-order-request-id", "type": "Parameter", "name": "requestId", "typeRef": { "type": "guid" },
                    "stereotypes": { "Parameter Settings": { "Source": "From Header", "Header Name": "x-requestid" } } }
                ] }
            ] }
        ]
      }]
    },
    "User Interface": {
      "packages": [{
        "id": "webappcomponents", "name": "WebAppComponents",
        "elements": [
          { "id": "ordering-service", "type": "Service Proxy", "name": "OrderingService",
            "children": [
              { "id": "ordering-service-create-order", "type": "Operation", "name": "CreateOrder",
                "stereotypes": { "Backend Operation": { "Operation": { "ref": "create-order" } } },
                "children": [
                  { "id": "ordering-service-create-order-request-id", "type": "Parameter", "name": "requestId", "typeRef": { "type": "guid" } }
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

```csharp file src/WebAppComponents/Services/OrderingService.cs
using System.Net.Http.Json;

namespace eShop.WebAppComponents.Services;

public class OrderingService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "api/orders/";

    public Task CreateOrder(Guid requestId)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, remoteServiceBaseUrl);
        requestMessage.Headers.Add("x-requestid", requestId.ToString());
        return httpClient.SendAsync(requestMessage);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.header", "element": "ordering-service-create-order", "outcome": "verified",
      "expected": "x-requestid", "actual": "x-requestid", "at": "src/WebAppComponents/Services/OrderingService.cs:12" }
  ]
}
```

## eshop-request-header-missing

The request no longer sets `x-requestid`, so the backend's required `requestId` is never sent.

```json case
{ "phase": "later", "base": "eshop-request-header", "kinds": ["proxy.header"], "traces": ["ESC-FE-63"], "requirements": ["R2", "R14"], "packs": ["intent.core", "house.eshopcustom"], "language": "csharp" }
```

```csharp file src/WebAppComponents/Services/OrderingService.cs
using System.Net.Http.Json;

namespace eShop.WebAppComponents.Services;

public class OrderingService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "api/orders/";

    public Task CreateOrder(Guid requestId)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, remoteServiceBaseUrl);
        return httpClient.SendAsync(requestMessage);
    }
}
```

```json expect
{
  "claims": [
    { "kind": "proxy.header", "element": "ordering-service-create-order", "outcome": "contradicted", "expected": "x-requestid" }
  ],
  "findings": [
    { "rule": "claim/proxy.header", "severity": "error", "subject": "ordering-service-create-order", "messageContains": "x-requestid" }
  ]
}
```
