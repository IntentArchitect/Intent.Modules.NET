# grpc.service.rpcs

A Services `Service` whose class derives from the base protoc generates for a `.proto` `service X` (`X.XBase`, the nested `XBase` of the static class `X`) implements that proto service, so its Operations are the service's rpcs: each rpc an Operation of its name taking the rpc's request message and returning its response, and each Operation an rpc. The claim (`intent.grpc`) is on every Service, as only the code says which ones implement a proto service: a class with no generated gRPC base states nothing (`blocked`, with no finding). The reading is the proto service's declaration, so the service, its rpcs and its `.proto` file are code the model describes, which the opt-in rules don't report. Of several `.proto` services of the name (a client's copy of the file beside the server's), the one protoc generates into the namespace the class's code names the base in is it: the file's `csharp_namespace`, else its package in Pascal case.

The fixtures adapt eShop's `Basket.API` gRPC service.

## implemented

`BasketService` derives from `Basket.BasketBase`, so it implements `basket.proto`'s `service Basket`, whose two rpcs are its two Operations, with the same request and response messages. The proto service, its rpcs and its file are located, so the opt-in rules report none of them.

```json case
{ "phase": 4, "kinds": ["grpc.service.rpcs"], "cases": ["optin/unmodelled.type", "optin/unmodelled.member", "optin/unmodelled.file"], "traces": ["ESC-SVC-78", "ESC-SVC-79"], "requirements": ["R2", "R49"], "packs": ["intent.core", "intent.services", "intent.grpc"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "get-basket-request", "type": "DTO", "name": "GetBasketRequest", "stereotypes": { "Proto Message": {} } },
          { "id": "customer-basket-response", "type": "DTO", "name": "CustomerBasketResponse", "stereotypes": { "Proto Message": {} } },
          { "id": "delete-basket-request", "type": "DTO", "name": "DeleteBasketRequest", "stereotypes": { "Proto Message": {} } },
          { "id": "delete-basket-response", "type": "DTO", "name": "DeleteBasketResponse", "stereotypes": { "Proto Message": {} } },
          { "id": "basket-service", "type": "Service", "name": "BasketService",
            "children": [
              { "id": "basket-service-get-basket", "type": "Operation", "name": "GetBasket", "typeRef": { "element": "customer-basket-response" },
                "children": [
                  { "id": "basket-service-get-basket-request", "type": "Parameter", "name": "request", "typeRef": { "element": "get-basket-request" } }
                ] },
              { "id": "basket-service-delete-basket", "type": "Operation", "name": "DeleteBasket", "typeRef": { "element": "delete-basket-response" },
                "children": [
                  { "id": "basket-service-delete-basket-request", "type": "Parameter", "name": "request", "typeRef": { "element": "delete-basket-request" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Basket", "type": "Root Folder",
        "stereotypes": {
          "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] },
          "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" }, { "Rule": "optin/unmodelled.member" }, { "Rule": "optin/unmodelled.file" } ] }
        } }]
    }
  }
}
```

```protobuf file src/Basket.API/Proto/basket.proto
syntax = "proto3";

option csharp_namespace = "eShop.Basket.API.Grpc";

package BasketApi;

service Basket {
    rpc GetBasket(GetBasketRequest) returns (CustomerBasketResponse) {}
    rpc DeleteBasket(DeleteBasketRequest) returns (DeleteBasketResponse) {}
}

message GetBasketRequest {
}

message CustomerBasketResponse {
}

message DeleteBasketRequest {
}

message DeleteBasketResponse {
}
```

```csharp file src/Basket.API/Grpc/BasketService.cs
namespace eShop.Basket.API.Grpc;

public class BasketService(IBasketRepository repository) : Basket.BasketBase
{
    public override async Task<CustomerBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
    {
        var data = await repository.GetBasketAsync(context.GetUserIdentity());
        return data;
    }

    public override async Task<DeleteBasketResponse> DeleteBasket(DeleteBasketRequest request, ServerCallContext context)
    {
        await repository.DeleteBasketAsync(context.GetUserIdentity());
        return new DeleteBasketResponse();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "grpc.service.rpcs", "element": "basket-service", "outcome": "verified",
      "expected": "DeleteBasket(DeleteBasketRequest): DeleteBasketResponse; GetBasket(GetBasketRequest): CustomerBasketResponse",
      "at": "src/Basket.API/Proto/basket.proto:7" }
  ],
  "findings": [
    { "rule": "claim/grpc.service.rpcs", "count": 0 },
    { "rule": "optin/unmodelled.type", "subject": "src/Basket.API/Proto/basket.proto:7", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/Basket.API/Proto/basket.proto:8", "count": 0 },
    { "rule": "optin/unmodelled.member", "subject": "src/Basket.API/Proto/basket.proto:9", "count": 0 },
    { "rule": "optin/unmodelled.file", "count": 0 }
  ]
}
```

## rpc-renamed

The `.proto` renames `DeleteBasket` to `RemoveBasket`, a planted drift: the proto service no longer has the model's Operations as its rpcs. It is one finding, on the Service. The renamed rpc is in the proto service the reading located, so `optin/unmodelled.member` doesn't report it a second time.

```json case
{ "phase": 4, "base": "implemented", "kinds": ["grpc.service.rpcs"], "cases": ["optin/unmodelled.member"], "traces": ["ESC-SVC-78"], "requirements": ["R2", "R11"], "packs": ["intent.core", "intent.services", "intent.grpc"], "language": "csharp" }
```

```protobuf file src/Basket.API/Proto/basket.proto
syntax = "proto3";

option csharp_namespace = "eShop.Basket.API.Grpc";

package BasketApi;

service Basket {
    rpc GetBasket(GetBasketRequest) returns (CustomerBasketResponse) {}
    rpc RemoveBasket(DeleteBasketRequest) returns (DeleteBasketResponse) {}
}

message GetBasketRequest {
}

message CustomerBasketResponse {
}

message DeleteBasketRequest {
}

message DeleteBasketResponse {
}
```

```json expect
{
  "claims": [
    { "kind": "grpc.service.rpcs", "element": "basket-service", "outcome": "contradicted",
      "expected": "DeleteBasket(DeleteBasketRequest): DeleteBasketResponse; GetBasket(GetBasketRequest): CustomerBasketResponse",
      "actual": "GetBasket(GetBasketRequest): CustomerBasketResponse; RemoveBasket(DeleteBasketRequest): DeleteBasketResponse",
      "at": "src/Basket.API/Proto/basket.proto:7" }
  ],
  "findings": [
    { "rule": "claim/grpc.service.rpcs", "severity": "error", "subject": "basket-service",
      "messageContains": "`BasketService` implements the proto service `Basket` (src/Basket.API/Proto/basket.proto) through `Basket.BasketBase`, but its rpc `RemoveBasket` is no Operation of `BasketService`; its Operation `DeleteBasket` is no rpc of `Basket`." },
    { "rule": "optin/unmodelled.member", "subject": "src/Basket.API/Proto/basket.proto:9", "count": 0 }
  ]
}
```

## response-changed

The `.proto`'s `DeleteBasket` now returns `CustomerBasketResponse`, where the model's Operation returns `DeleteBasketResponse`, as the C# override still does: one finding, naming the rpc's signature on both sides.

```json case
{ "phase": 4, "base": "implemented", "kinds": ["grpc.service.rpcs"], "traces": ["ESC-SVC-79"], "requirements": ["R2", "R11"], "packs": ["intent.core", "intent.services", "intent.grpc"], "language": "csharp" }
```

```protobuf file src/Basket.API/Proto/basket.proto
syntax = "proto3";

option csharp_namespace = "eShop.Basket.API.Grpc";

package BasketApi;

service Basket {
    rpc GetBasket(GetBasketRequest) returns (CustomerBasketResponse) {}
    rpc DeleteBasket(DeleteBasketRequest) returns (CustomerBasketResponse) {}
}

message GetBasketRequest {
}

message CustomerBasketResponse {
}

message DeleteBasketRequest {
}

message DeleteBasketResponse {
}
```

```json expect
{
  "claims": [
    { "kind": "grpc.service.rpcs", "element": "basket-service", "outcome": "contradicted",
      "actual": "DeleteBasket(DeleteBasketRequest): CustomerBasketResponse; GetBasket(GetBasketRequest): CustomerBasketResponse" }
  ],
  "findings": [
    { "rule": "claim/grpc.service.rpcs", "severity": "error", "subject": "basket-service",
      "messageContains": "`DeleteBasket` is `(DeleteBasketRequest): CustomerBasketResponse` in the .proto and `(DeleteBasketRequest): DeleteBasketResponse` in the model" }
  ]
}
```

## no-generated-base

`BasketService` no longer derives from `Basket.BasketBase`, so it implements no proto service and states nothing: the claim is `blocked`, with no finding. The proto service is then code nothing in the model describes, so `optin/unmodelled.type` reports it again.

```json case
{ "phase": 4, "base": "implemented", "kinds": ["grpc.service.rpcs"], "cases": ["optin/unmodelled.type"], "requirements": ["R5", "R49"], "packs": ["intent.core", "intent.services", "intent.grpc"], "language": "csharp" }
```

```csharp file src/Basket.API/Grpc/BasketService.cs
namespace eShop.Basket.API.Grpc;

public class BasketService(IBasketRepository repository)
{
    public async Task<CustomerBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
    {
        var data = await repository.GetBasketAsync(context.GetUserIdentity());
        return data;
    }

    public async Task<DeleteBasketResponse> DeleteBasket(DeleteBasketRequest request, ServerCallContext context)
    {
        await repository.DeleteBasketAsync(context.GetUserIdentity());
        return new DeleteBasketResponse();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "grpc.service.rpcs", "element": "basket-service", "outcome": "cantVerify", "reason": "blocked" }
  ],
  "findings": [
    { "rule": "claim/grpc.service.rpcs", "count": 0 },
    { "rule": "optin/unmodelled.type", "severity": "error", "subject": "src/Basket.API/Proto/basket.proto:7", "messageContains": "`Basket` isn't in the model" }
  ]
}
```

## other-service-states-nothing

An ordinary application service, `OrderingService`, sits beside the gRPC one, and an unrelated class of the code derives from a base named like a generated one (`Workflow.WorkflowBase`) that no `.proto` declares a service for. Neither implements a proto service, so each Service's claim is `blocked`, with no finding.

```json case
{ "phase": 4, "base": "implemented", "kinds": ["grpc.service.rpcs"], "requirements": ["R5"], "packs": ["intent.core", "intent.services", "intent.grpc"], "language": "csharp" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "get-basket-request", "type": "DTO", "name": "GetBasketRequest", "stereotypes": { "Proto Message": {} } },
          { "id": "customer-basket-response", "type": "DTO", "name": "CustomerBasketResponse", "stereotypes": { "Proto Message": {} } },
          { "id": "delete-basket-request", "type": "DTO", "name": "DeleteBasketRequest", "stereotypes": { "Proto Message": {} } },
          { "id": "delete-basket-response", "type": "DTO", "name": "DeleteBasketResponse", "stereotypes": { "Proto Message": {} } },
          { "id": "basket-service", "type": "Service", "name": "BasketService",
            "children": [
              { "id": "basket-service-get-basket", "type": "Operation", "name": "GetBasket", "typeRef": { "element": "customer-basket-response" },
                "children": [
                  { "id": "basket-service-get-basket-request", "type": "Parameter", "name": "request", "typeRef": { "element": "get-basket-request" } }
                ] },
              { "id": "basket-service-delete-basket", "type": "Operation", "name": "DeleteBasket", "typeRef": { "element": "delete-basket-response" },
                "children": [
                  { "id": "basket-service-delete-basket-request", "type": "Parameter", "name": "request", "typeRef": { "element": "delete-basket-request" } }
                ] }
            ] },
          { "id": "ordering-service", "type": "Service", "name": "OrderingService",
            "children": [
              { "id": "ordering-service-cancel", "type": "Operation", "name": "Cancel",
                "children": [
                  { "id": "ordering-service-cancel-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
                ] }
            ] },
          { "id": "workflow-service", "type": "Service", "name": "ApprovalWorkflow",
            "children": [
              { "id": "workflow-service-approve", "type": "Operation", "name": "Approve",
                "children": [
                  { "id": "workflow-service-approve-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "int" } }
                ] }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Basket", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```csharp file src/Basket.API/Services/OrderingService.cs
namespace eShop.Basket.API.Services;

public class OrderingService
{
    public void Cancel(int orderId)
    {
    }
}
```

```csharp file src/Basket.API/Services/ApprovalWorkflow.cs
namespace eShop.Basket.API.Services;

public class ApprovalWorkflow : Workflow.WorkflowBase
{
    public void Approve(int orderId)
    {
    }
}
```

```json expect
{
  "claims": [
    { "kind": "grpc.service.rpcs", "element": "basket-service", "outcome": "verified" },
    { "kind": "grpc.service.rpcs", "element": "ordering-service", "outcome": "cantVerify", "reason": "blocked" },
    { "kind": "grpc.service.rpcs", "element": "workflow-service", "outcome": "cantVerify", "reason": "blocked" }
  ],
  "findings": [
    { "rule": "claim/grpc.service.rpcs", "count": 0 }
  ]
}
```

## through-own-base

`BasketService` derives from the code's own `BasketServiceBase`, which derives from `Basket.BasketBase`: the generated base is found through the class's in-scope bases, so it implements the proto service all the same.

```json case
{ "phase": 4, "base": "implemented", "kinds": ["grpc.service.rpcs"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.grpc"], "language": "csharp" }
```

```csharp file src/Basket.API/Grpc/BasketService.cs
namespace eShop.Basket.API.Grpc;

public abstract class BasketServiceBase : Basket.BasketBase
{
}

public class BasketService(IBasketRepository repository) : BasketServiceBase
{
    public override async Task<CustomerBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
    {
        var data = await repository.GetBasketAsync(context.GetUserIdentity());
        return data;
    }

    public override async Task<DeleteBasketResponse> DeleteBasket(DeleteBasketRequest request, ServerCallContext context)
    {
        await repository.DeleteBasketAsync(context.GetUserIdentity());
        return new DeleteBasketResponse();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "grpc.service.rpcs", "element": "basket-service", "outcome": "verified", "at": "src/Basket.API/Proto/basket.proto:7" }
  ]
}
```

## client-copy-beside-server

A client's copy of the `.proto` declares `service Basket` too, as a front end keeps one to generate its gRPC client. Neither file says which is the server's by its name, so the namespace decides: the server's file has no `csharp_namespace`, so protoc generates `Basket.BasketBase` into its package in Pascal case, `Shop.BasketApi`, the namespace `BasketService` is declared in; the client's file generates into `Shop.WebApp.BasketClient`, which `BasketService`'s file doesn't reach. The claim reads the server's file. The client's copy states other rpcs, which would contradict the claim had it been read.

```json case
{ "phase": 4, "base": "implemented", "kinds": ["grpc.service.rpcs"], "requirements": ["R2"], "packs": ["intent.core", "intent.services", "intent.grpc"], "language": "csharp" }
```

```protobuf file src/Basket.API/Proto/basket.proto
syntax = "proto3";

package shop.basket_api;

service Basket {
    rpc GetBasket(GetBasketRequest) returns (CustomerBasketResponse) {}
    rpc DeleteBasket(DeleteBasketRequest) returns (DeleteBasketResponse) {}
}

message GetBasketRequest {
}

message CustomerBasketResponse {
}

message DeleteBasketRequest {
}

message DeleteBasketResponse {
}
```

```protobuf file src/WebApp/Protos/basket.proto
syntax = "proto3";

option csharp_namespace = "Shop.WebApp.BasketClient";

package shop.basket_api;

service Basket {
    rpc GetBasket(GetBasketRequest) returns (CustomerBasketResponse) {}
}

message GetBasketRequest {
}

message CustomerBasketResponse {
}
```

```csharp file src/Basket.API/Grpc/BasketService.cs
namespace Shop.BasketApi;

public class BasketService(IBasketRepository repository) : Basket.BasketBase
{
    public override async Task<CustomerBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
    {
        var data = await repository.GetBasketAsync(context.GetUserIdentity());
        return data;
    }

    public override async Task<DeleteBasketResponse> DeleteBasket(DeleteBasketRequest request, ServerCallContext context)
    {
        await repository.DeleteBasketAsync(context.GetUserIdentity());
        return new DeleteBasketResponse();
    }
}
```

```json expect
{
  "claims": [
    { "kind": "grpc.service.rpcs", "element": "basket-service", "outcome": "verified", "at": "src/Basket.API/Proto/basket.proto:5" }
  ],
  "findings": [
    { "rule": "claim/grpc.service.rpcs", "count": 0 }
  ]
}
```
