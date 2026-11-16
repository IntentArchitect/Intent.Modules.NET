# type.location

Where the type's file must be. The value comes from vocabulary, not from the element alone (R28, R41): the codebase's house pack mirrors the element's designer folders under a source folder, as eShopCustom's modules mirrored them under their first Source and the Java spike under its domain package. The type is located anywhere in scope, so a type in the wrong folder is found (`type.exists` verified) and its location contradicted, rather than reported missing.

The structure reading is the path of the file that declares the type. eShopCustom's Domain module checked the whole path, `<folders>/<Name>.cs`, so its cases compare paths. Its Services and Front End modules checked the folder only, because a file may declare several types (eShop's `CatalogItem.cs` declares `CatalogBrand` too), so their modules replace the reading with a claim reader of their own (`house.readers`) that reads the declaring file's folder (R46, R48).

## verified

`Order` and its repository interface sit in the folders that mirror `AggregatesModel/OrderAggregate`, in files named after them.

```json case
{ "phase": 2, "kinds": ["type.location"], "traces": ["ESC-DOM-14", "ESC-DOM-70"], "requirements": ["R2", "R28", "R30", "R41", "R53"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "type.location",
          "for": { "element": "Class" },
          "value": "'src/Ordering.Domain/' + if(exists(folderPath), folderPath + '/', '') + name + '.cs'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Class's folders in the Domain designer",
          "fixHint": "Move the file to the folder the model's folders give, or move the Class in the designer."
        },
        {
          "kind": "type.exists",
          "for": { "element": "Repository" },
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "source": "the Repository"
        },
        {
          "kind": "type.location",
          "for": { "element": "Repository" },
          "value": "'src/Ordering.Domain/' + if(exists(folderPath), folderPath + '/', '') + 'I' + name + '.cs'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Repository's folders in the Domain designer",
          "fixHint": "Move the repository interface's file to the folder the model's folders give."
        }
      ]
    }
  },
  "conventions": {
    "Domain": {
      "types": {
        "Repository": { "typeKinds": ["interface"], "name": "I{Name}", "note": "eShopCustom describes a Repository by its interface" }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Ordering.Domain",
        "elements": [
          { "id": "aggregates-model", "type": "Folder", "name": "AggregatesModel",
            "children": [
              { "id": "order-aggregate", "type": "Folder", "name": "OrderAggregate",
                "children": [
                  { "id": "order", "type": "Class", "name": "Order" },
                  { "id": "order-repository", "type": "Repository", "name": "OrderRepository", "typeRef": { "element": "order" } }
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

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
{
    public int Id { get; private set; }
}
```

```csharp file src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs
namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public interface IOrderRepository
{
    Order Add(Order order);
}
```

```json expect
{
  "claims": [
    { "kind": "type.location", "element": "order", "outcome": "verified",
      "expected": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs", "actual": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs",
      "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs:3" },
    { "kind": "type.location", "element": "order-repository", "outcome": "verified",
      "expected": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs",
      "at": "src/Ordering.Domain/AggregatesModel/OrderAggregate/IOrderRepository.cs:3" }
  ]
}
```

## contradicted-moved

`Order` was moved to the project's root folder. It is still found, so `type.exists` is verified; its file isn't where the model's folders put it, so `type.location` is contradicted, naming both paths (R11, R14).

```json case
{ "phase": 2, "base": "verified", "without": ["src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs"], "kinds": ["type.exists", "type.location"], "traces": ["ESC-DOM-14"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```csharp file src/Ordering.Domain/Order.cs
namespace eShop.Ordering.Domain;

public class Order
{
    public int Id { get; private set; }
}
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "verified", "at": "src/Ordering.Domain/Order.cs:3" },
    { "kind": "type.location", "element": "order", "outcome": "contradicted",
      "expected": "src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs", "actual": "src/Ordering.Domain/Order.cs",
      "at": "src/Ordering.Domain/Order.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.location", "severity": "error", "subject": "order", "messageContains": "src/Ordering.Domain/Order.cs" }
  ]
}
```

## blocked

`Order` isn't in the code, so it has no file to compare. `type.location` is can't verify, `blocked` behind the contradicted `type.exists` (R5).

```json case
{ "phase": 2, "base": "verified", "without": ["src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs"], "kinds": ["type.exists", "type.location"], "cases": ["engine/blocked"], "requirements": ["R5", "R8"], "packs": ["intent.core", "intent.domain", "house.eshop"], "language": "csharp" }
```

```json expect
{
  "claims": [
    { "kind": "type.exists", "element": "order", "outcome": "contradicted" },
    { "kind": "type.location", "element": "order", "outcome": "cantVerify", "reason": "blocked", "blockedBy": "type.exists" }
  ],
  "findings": [
    { "rule": "claim/type.exists", "severity": "error", "subject": "order", "messageContains": "isn't in the code" },
    { "rule": "claim/type.location", "subject": "order", "count": 0, "note": "A blocked claim has no finding of its own" }
  ]
}
```

## services-folder

eShopCustom's Services module checked that a type's file is in the folder mirroring its model folders, whatever the file's name. `OrderDraftDto` is declared in `CreateOrderDraftCommand.cs`, beside the command that returns it, which is in the right folder, so both claims are verified.

```json case
{ "phase": 2, "kinds": ["type.location"], "traces": ["ESC-SVC-12"], "requirements": ["R2", "R28", "R46", "R48"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }],
  "languages": ["csharp"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        {
          "kind": "type.location",
          "for": { "element": "Command" },
          "value": "'src/Ordering.API' + if(exists(folderPath), '/' + folderPath, '')",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Command's folders in the Services designer"
        },
        {
          "kind": "type.location",
          "for": { "element": "DTO" },
          "value": "'src/Ordering.API' + if(exists(folderPath), '/' + folderPath, '')",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the DTO's folders in the Services designer"
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
        "id": "services", "name": "Ordering.API",
        "elements": [
          { "id": "application", "type": "Folder", "name": "Application",
            "children": [
              { "id": "commands", "type": "Folder", "name": "Commands",
                "children": [
                  { "id": "create-order-draft", "type": "Command", "name": "CreateOrderDraftCommand", "typeRef": { "element": "order-draft-dto" } },
                  { "id": "order-draft-dto", "type": "DTO", "name": "OrderDraftDto" }
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

```csharp file src/Ordering.API/Application/Commands/CreateOrderDraftCommand.cs
namespace eShop.Ordering.API.Application.Commands;

public record CreateOrderDraftCommand(string BuyerId) : IRequest<OrderDraftDto>;

public record OrderDraftDto(decimal Total);
```

```json expect
{
  "claims": [
    { "kind": "type.location", "element": "create-order-draft", "outcome": "verified",
      "expected": "src/Ordering.API/Application/Commands", "actual": "src/Ordering.API/Application/Commands",
      "at": "src/Ordering.API/Application/Commands/CreateOrderDraftCommand.cs:3" },
    { "kind": "type.location", "element": "order-draft-dto", "outcome": "verified",
      "expected": "src/Ordering.API/Application/Commands", "actual": "src/Ordering.API/Application/Commands",
      "at": "src/Ordering.API/Application/Commands/CreateOrderDraftCommand.cs:5" }
  ]
}
```

## front-end-folders

eShopCustom's Front End module checked components, model definitions and service proxies the same way, against the User Interface designer's folders under each project. No stock pack before phase 6 knows that designer, so the house pack locates its elements too (R28, R53). `CatalogBrand` is declared in `CatalogItem.cs`, in its model folder.

```json case
{ "phase": 2, "kinds": ["type.location"], "traces": ["ESC-FE-09", "ESC-FE-40", "ESC-FE-51"], "requirements": ["R2", "R28", "R30", "R37", "R46", "R53"], "packs": ["intent.core", "house.eshop", "house.readers"], "language": "csharp" }
```

```json pack
{
  "id": "house.eshop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["csharp"],
  "designers": ["User Interface"],
  "vocabulary": {
    "User Interface": {
      "entries": [
        { "kind": "type.exists", "for": { "element": "Component" }, "bindings": { "Type": "name" }, "own": ["Type"], "source": "the Component" },
        { "kind": "type.exists", "for": { "element": "Model Definition" }, "bindings": { "Type": "name" }, "own": ["Type"], "source": "the Model Definition" },
        { "kind": "type.exists", "for": { "element": "Service Proxy" }, "bindings": { "Type": "name" }, "own": ["Type"], "source": "the Service Proxy" },
        {
          "kind": "type.location",
          "for": { "element": "Component" },
          "value": "'src/' + package.name + if(exists(folderPath), '/' + folderPath, '')",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Component's folders in the User Interface designer"
        },
        {
          "kind": "type.location",
          "for": { "element": "Model Definition" },
          "value": "'src/' + package.name + if(exists(folderPath), '/' + folderPath, '')",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Model Definition's folders in the User Interface designer"
        },
        {
          "kind": "type.location",
          "for": { "element": "Service Proxy" },
          "value": "'src/' + package.name + if(exists(folderPath), '/' + folderPath, '')",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Service Proxy's folders in the User Interface designer"
        }
      ]
    }
  },
  "conventions": {
    "User Interface": {
      "types": {
        "Component": { "typeKinds": ["component"] },
        "Model Definition": { "typeKinds": ["record", "class", "struct"] },
        "Service Proxy": { "typeKinds": ["class"] }
      }
    }
  }
}
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "web-app-components", "name": "WebAppComponents",
        "elements": [
          { "id": "catalog-folder", "type": "Folder", "name": "Catalog",
            "children": [
              { "id": "catalog-list-item", "type": "Component", "name": "CatalogListItem" },
              { "id": "catalog-item", "type": "Model Definition", "name": "CatalogItem" },
              { "id": "catalog-brand", "type": "Model Definition", "name": "CatalogBrand" }
            ] },
          { "id": "services-folder", "type": "Folder", "name": "Services",
            "children": [
              { "id": "catalog-service", "type": "Service Proxy", "name": "CatalogService" }
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

```razor file src/WebAppComponents/Catalog/CatalogListItem.razor
<div class="catalog-item">
    <span class="catalog-product-name">@Item.Name</span>
    <span class="catalog-product-price">@Item.Price.ToString("0.00")</span>
</div>

@code {
    [Parameter, EditorRequired]
    public required CatalogItem Item { get; set; }
}
```

```csharp file src/WebAppComponents/Catalog/CatalogItem.cs
namespace eShop.WebAppComponents.Catalog;

public record CatalogItem(int Id, string Name, decimal Price, CatalogBrand CatalogBrand);

public record CatalogBrand(int Id, string Brand);
```

```csharp file src/WebAppComponents/Services/CatalogService.cs
using eShop.WebAppComponents.Catalog;

namespace eShop.WebAppComponents.Services;

public class CatalogService(HttpClient httpClient)
{
    public Task<CatalogItem?> GetCatalogItem(int id)
        => httpClient.GetFromJsonAsync<CatalogItem>($"api/catalog/items/{id}");
}
```

```json expect
{
  "claims": [
    { "kind": "type.location", "element": "catalog-list-item", "outcome": "verified",
      "expected": "src/WebAppComponents/Catalog", "actual": "src/WebAppComponents/Catalog" },
    { "kind": "type.location", "element": "catalog-item", "outcome": "verified",
      "expected": "src/WebAppComponents/Catalog", "at": "src/WebAppComponents/Catalog/CatalogItem.cs:3" },
    { "kind": "type.location", "element": "catalog-brand", "outcome": "verified",
      "expected": "src/WebAppComponents/Catalog", "actual": "src/WebAppComponents/Catalog",
      "at": "src/WebAppComponents/Catalog/CatalogItem.cs:5" },
    { "kind": "type.location", "element": "catalog-service", "outcome": "verified",
      "expected": "src/WebAppComponents/Services", "at": "src/WebAppComponents/Services/CatalogService.cs:5" }
  ]
}
```

## front-end-proxy-moved

The service proxy was moved into the `Catalog` folder. Its model folder is `Services`, so its location is contradicted.

```json case
{ "phase": 2, "base": "front-end-folders", "without": ["src/WebAppComponents/Services/CatalogService.cs"], "kinds": ["type.location"], "traces": ["ESC-FE-51"], "requirements": ["R1", "R11", "R14"], "packs": ["intent.core", "house.eshop", "house.readers"], "language": "csharp" }
```

```csharp file src/WebAppComponents/Catalog/CatalogService.cs
namespace eShop.WebAppComponents.Catalog;

public class CatalogService(HttpClient httpClient)
{
    public Task<CatalogItem?> GetCatalogItem(int id)
        => httpClient.GetFromJsonAsync<CatalogItem>($"api/catalog/items/{id}");
}
```

```json expect
{
  "claims": [
    { "kind": "type.location", "element": "catalog-service", "outcome": "contradicted",
      "expected": "src/WebAppComponents/Services", "actual": "src/WebAppComponents/Catalog",
      "at": "src/WebAppComponents/Catalog/CatalogService.cs:3" }
  ],
  "findings": [
    { "rule": "claim/type.location", "severity": "error", "subject": "catalog-service", "messageContains": "src/WebAppComponents/Services" }
  ]
}
```

## java-sub-package

The Java spike checked that a domain type is in the sub-package its designer folder names. The storefront's house pack mirrors the folders under the domain package's folder; the folders are named as the packages are (R33).

```json case
{ "phase": 4, "kinds": ["type.location"], "traces": ["JAV-DOM-03"], "requirements": ["R2", "R28", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "house.storefront"], "language": "java" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.domain", "version": "^1.0.0" }, { "id": "intent.java", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Domain"],
  "vocabulary": {
    "Domain": {
      "entries": [
        {
          "kind": "type.location",
          "for": { "element": "Class" },
          "value": "'src/main/java/com/shop/domain/' + if(exists(folderPath), folderPath + '/', '') + name + '.java'",
          "bindings": { "Type": "name" },
          "own": ["Type"],
          "dependsOn": "type.exists",
          "source": "the Class's folders in the Domain designer",
          "fixHint": "Move the class to the sub-package its designer folder names."
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Domain": {
      "packages": [{
        "id": "domain", "name": "Shop.Domain",
        "elements": [
          { "id": "ordering", "type": "Folder", "name": "ordering",
            "children": [
              { "id": "order", "type": "Class", "name": "Order" }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/main/java/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```java file src/main/java/com/shop/domain/ordering/Order.java
package com.shop.domain.ordering;

public class Order {
}
```

```json expect
{
  "claims": [
    { "kind": "type.location", "element": "order", "outcome": "verified",
      "expected": "src/main/java/com/shop/domain/ordering/Order.java", "actual": "src/main/java/com/shop/domain/ordering/Order.java",
      "at": "src/main/java/com/shop/domain/ordering/Order.java:3" }
  ]
}
```

## java-wrong-package

`Order` is in the domain package itself, not in `ordering`.

```json case
{ "phase": 4, "base": "java-sub-package", "without": ["src/main/java/com/shop/domain/ordering/Order.java"], "kinds": ["type.location"], "traces": ["JAV-DOM-03"], "requirements": ["R1", "R11", "R33"], "packs": ["intent.core", "intent.domain", "intent.java", "house.storefront"], "language": "java" }
```

```java file src/main/java/com/shop/domain/Order.java
package com.shop.domain;

public class Order {
}
```

```json expect
{
  "claims": [
    { "kind": "type.location", "element": "order", "outcome": "contradicted",
      "expected": "src/main/java/com/shop/domain/ordering/Order.java", "actual": "src/main/java/com/shop/domain/Order.java",
      "at": "src/main/java/com/shop/domain/Order.java:3" }
  ],
  "findings": [
    { "rule": "claim/type.location", "severity": "error", "subject": "order" }
  ]
}
```
