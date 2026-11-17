# file.exists

A file an element stands for: a gRPC service's `.proto`, a Vue component's `.vue`, a file a layer's convention requires. The path comes from vocabulary (R28, R30), and no stock pack reads it, so a house module's claim reader (`house.readers`) looks for the file among the files the Drift Scope decides, whatever its language (R38, R46). It is an existence kind: a missing file is contradicted (R8). A file the scope leaves out is can't verify, `outOfScope`. Where the claim also needs something inside the file, such as the service a `.proto` declares, a file that exists but doesn't declare it is contradicted from silence (R3).

## proto-file-verified

eShopCustom's gRPC Service stereotype names the `.proto` that defines the service, relative to the first Source, and the service it declares. `basket.proto` is there and declares `service Basket`.

```json case
{ "phase": 2, "kinds": ["file.exists"], "traces": ["ESC-SVC-75"], "requirements": ["R2", "R28", "R30", "R38", "R46"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "protobuf" }
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
          "kind": "file.exists",
          "for": { "element": "Service", "where": "hasStereotype('gRPC Service')" },
          "bindings": {
            "File": "'src/' + stereotype('gRPC Service')['Proto File']",
            "ProtoService": "stereotype('gRPC Service')['Proto Service']"
          },
          "source": "the gRPC Service stereotype's Proto File and Proto Service",
          "fixHint": "Add the .proto file declaring the service, or change the gRPC Service stereotype."
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
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "basket-grpc", "type": "Service", "name": "BasketService",
            "stereotypes": { "gRPC Service": { "Proto File": "Basket.API/Proto/basket.proto", "Proto Service": "Basket" } } }
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

```protobuf file src/Basket.API/Proto/basket.proto
syntax = "proto3";

option csharp_namespace = "eShop.Basket.API.Grpc";

package BasketApi;

service Basket {
    rpc GetBasket(GetBasketRequest) returns (CustomerBasketResponse) {}
}

message GetBasketRequest {
}

message CustomerBasketResponse {
    repeated BasketItem items = 1;
}

message BasketItem {
    int32 product_id = 2;
    int32 quantity = 6;
}
```

```json expect
{
  "claims": [
    { "kind": "file.exists", "element": "basket-grpc", "outcome": "verified", "expected": "present", "actual": "present",
      "at": "src/Basket.API/Proto/basket.proto:7" }
  ]
}
```

## proto-file-missing

The `.proto` the stereotype names isn't in the code. The find finds nothing, and the kind is an existence kind, so the claim is contradicted (R8).

```json case
{ "phase": 2, "base": "proto-file-verified", "without": ["src/Basket.API/Proto/basket.proto"], "kinds": ["file.exists"], "traces": ["ESC-SVC-75"], "requirements": ["R1", "R8", "R11"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "protobuf" }
```

```json expect
{
  "claims": [
    { "kind": "file.exists", "element": "basket-grpc", "outcome": "contradicted", "expected": "present" }
  ],
  "findings": [
    { "rule": "claim/file.exists", "severity": "error", "subject": "basket-grpc", "messageContains": "`BasketService` isn't in the code." }
  ]
}
```

## proto-service-not-declared

The `.proto` is there, but its service was renamed `BasketApi`. The file doesn't declare the Proto Service the model names, so the recipe's `default` applies and the claim is contradicted from silence (R3).

```json case
{ "phase": 2, "base": "proto-file-verified", "kinds": ["file.exists"], "traces": ["ESC-SVC-75"], "requirements": ["R1", "R3"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "protobuf" }
```

```protobuf file src/Basket.API/Proto/basket.proto
syntax = "proto3";

option csharp_namespace = "eShop.Basket.API.Grpc";

package BasketApi;

service BasketApi {
    rpc GetBasket(GetBasketRequest) returns (CustomerBasketResponse) {}
}

message GetBasketRequest {
}

message CustomerBasketResponse {
}
```

```json expect
{
  "claims": [
    { "kind": "file.exists", "element": "basket-grpc", "outcome": "contradicted", "expected": "present", "actual": "absent",
      "fromSilence": true }
  ],
  "findings": [
    { "rule": "claim/file.exists", "severity": "error", "subject": "basket-grpc" }
  ]
}
```

## proto-file-out-of-scope

The `.proto` files are out of scope: a row excludes the contracts shared with other services. Nothing in scope matches the `file:` step, but the step's path names the excluded file, so the claim is can't verify, `outOfScope`, naming the row, rather than contradicted. The excluded file is never read (R22, R25).

```json case
{ "phase": 2, "base": "proto-file-verified", "kinds": ["file.exists"], "requirements": ["R22", "R25"], "packs": ["intent.core", "intent.services", "house.eshop", "house.readers"], "language": "protobuf" }
```

```json model
{
  "designers": {
    "Services": {
      "packages": [{
        "id": "services", "name": "Basket.API",
        "elements": [
          { "id": "basket-grpc", "type": "Service", "name": "BasketService",
            "stereotypes": { "gRPC Service": { "Proto File": "Basket.API/Proto/basket.proto", "Proto Service": "Basket" } } }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [
          { "Glob": "src/**", "Scope": "In" },
          { "Glob": "src/**/Proto/**", "Scope": "Out", "Reason": "Contracts shared with other services, checked where they are owned" }
        ] } } }]
    }
  }
}
```

```json expect
{
  "claims": [
    { "kind": "file.exists", "element": "basket-grpc", "outcome": "cantVerify", "reason": "outOfScope", "scopeRow": "root#2" }
  ],
  "findings": [
    { "rule": "claim/file.exists", "severity": "error", "subject": "basket-grpc", "messageContains": "src/**/Proto/**" }
  ],
  "filesRead": { "exclude": ["src/Basket.API/Proto/basket.proto"] }
}
```

## vue-files-verified

The Java spike's Front End designer has Layouts, Pages, Components, Stores, Composables, Utilities and Proxies, each standing for a `.vue` or `.ts` file named after it in the folder its designer folders give, or at its Implementation stereotype's Path. The TypeScript reader, which reads Vue single-file components as `component` types, arrives later with intent.typescript; until then the storefront's house pack finds the file by path and reads it as text (R38). A file's existence needs nothing more. `StockPill` is in `components`; the cart Store names its file.

```json case
{ "phase": 4, "kinds": ["file.exists"], "traces": ["JAV-FE-02"], "requirements": ["R2", "R28", "R38", "R46"], "packs": ["intent.core", "intent.java", "house.storefront", "house.readers"], "language": "typescript" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "languages": ["typescript"],
  "designers": ["Front End"],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "file.exists",
          "for": { "element": "Component" },
          "bindings": { "File": "stereotype('Implementation').Path ?? ('src/' + if(exists(folderPath), folderPath + '/', '') + name + '.vue')" },
          "source": "the Component's folders and name, or its Implementation stereotype's Path",
          "fixHint": "Add the component's .vue file, or set its Implementation Path."
        },
        {
          "kind": "file.exists",
          "for": { "element": "Store" },
          "bindings": { "File": "stereotype('Implementation').Path ?? ('src/' + if(exists(folderPath), folderPath + '/', '') + name + '.ts')" },
          "source": "the Store's folders and name, or its Implementation stereotype's Path",
          "fixHint": "Add the store's .ts file, or set its Implementation Path."
        }
      ]
    }
  }
}
```

```json model
{
  "designers": {
    "Front End": {
      "packages": [{
        "id": "storefront", "name": "Storefront",
        "elements": [
          { "id": "components-folder", "type": "Folder", "name": "components",
            "children": [
              { "id": "stock-pill", "type": "Component", "name": "StockPill" }
            ] },
          { "id": "stores-folder", "type": "Folder", "name": "stores",
            "children": [
              { "id": "cart-store", "type": "Store", "name": "CartStore",
                "stereotypes": { "Implementation": { "Path": "src/stores/cart.ts" } } }
            ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "Storefront", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```vue file src/components/StockPill.vue
<script setup lang="ts">
defineProps<{ inStock: boolean }>();
</script>

<template>
  <span :class="inStock ? 'pill in' : 'pill out'">{{ inStock ? 'In stock' : 'Out of stock' }}</span>
</template>
```

```typescript file src/stores/cart.ts
import { defineStore } from 'pinia';

export const useCartStore = defineStore('cart', {
  state: () => ({ lines: [] as { productId: string; quantity: number }[] }),
});
```

```json expect
{
  "claims": [
    { "kind": "file.exists", "element": "stock-pill", "outcome": "verified", "at": "src/components/StockPill.vue:1" },
    { "kind": "file.exists", "element": "cart-store", "outcome": "verified", "at": "src/stores/cart.ts:1" }
  ]
}
```

## vue-file-missing

`StockPill` was renamed `StockBadge` in the code and not in the model. Nothing is at `src/components/StockPill.vue`, so the claim is contradicted.

```json case
{ "phase": 4, "base": "vue-files-verified", "without": ["src/components/StockPill.vue"], "kinds": ["file.exists"], "traces": ["JAV-FE-02"], "requirements": ["R1", "R8", "R11"], "packs": ["intent.core", "intent.java", "house.storefront", "house.readers"], "language": "typescript" }
```

```vue file src/components/StockBadge.vue
<script setup lang="ts">
defineProps<{ inStock: boolean }>();
</script>

<template>
  <span :class="inStock ? 'badge in' : 'badge out'">{{ inStock ? 'In stock' : 'Out of stock' }}</span>
</template>
```

```json expect
{
  "claims": [
    { "kind": "file.exists", "element": "stock-pill", "outcome": "contradicted" },
    { "kind": "file.exists", "element": "cart-store", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/file.exists", "severity": "error", "subject": "stock-pill", "messageContains": "`StockPill` isn't in the code." }
  ]
}
```

## file-convention

The Java spike's architecture gave each layer a path and file conventions: every element a convention selects (by designer, element type, stereotype and folder) has a file at `<Layer Path>/<pattern>`, with `{Name}`, `{name}`, `{Singular}`, `{singular}` and `{Folder}` in the pattern and an optional suffix removed from the name first. Here the API layer's convention for Services is `{Name}Controller.java` with the suffix `Service` removed.

The storefront's house pack holds the convention: the entry's `for` is its selection, and its `File` binding is the layer's path and the pattern, written with the expression functions (`trimEnd` for Remove Suffix). The spike's model held conventions as File Convention elements under each Layer, selecting another designer's elements; vocabulary selects only within the designer it is for, so the pack, not the model, states them.

```json case
{ "phase": 4, "kinds": ["file.exists"], "traces": ["JAV-ARC-04"], "requirements": ["R2", "R28", "R30", "R51", "R53"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc", "house.storefront", "house.readers"], "language": "java" }
```

```json pack
{
  "id": "house.storefront",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.services", "version": "^1.0.0" }, { "id": "intent.java", "version": "^1.0.0" }],
  "languages": ["java"],
  "designers": ["Services"],
  "vocabulary": {
    "Services": {
      "entries": [
        {
          "kind": "file.exists",
          "for": { "element": "Service" },
          "bindings": { "File": "'src/main/java/com/shop/api/' + trimEnd(name, 'Service') + 'Controller.java'" },
          "source": "the API layer's file convention for Services, {Name}Controller.java with the suffix Service removed",
          "fixHint": "Add the controller the convention names, or rename the Service.",
          "note": "The API layer's path is src/main/java/com/shop/api"
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
        "id": "services", "name": "Shop.Services",
        "elements": [
          { "id": "orders-service", "type": "Service", "name": "OrdersService" },
          { "id": "products-service", "type": "Service", "name": "ProductsService" }
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

```java file src/main/java/com/shop/api/OrdersController.java
package com.shop.api;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/orders")
public class OrdersController {
}
```

```java file src/main/java/com/shop/api/ProductsController.java
package com.shop.api;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/products")
public class ProductsController {
}
```

```json expect
{
  "claims": [
    { "kind": "file.exists", "element": "orders-service", "outcome": "verified", "at": "src/main/java/com/shop/api/OrdersController.java:1" },
    { "kind": "file.exists", "element": "products-service", "outcome": "verified", "at": "src/main/java/com/shop/api/ProductsController.java:1" }
  ]
}
```

## file-convention-missing

The products controller isn't in the API layer, so the file its convention requires is missing.

```json case
{ "phase": 4, "base": "file-convention", "without": ["src/main/java/com/shop/api/ProductsController.java"], "kinds": ["file.exists"], "traces": ["JAV-ARC-04"], "requirements": ["R1", "R8", "R11"], "packs": ["intent.core", "intent.services", "intent.java", "intent.springmvc", "house.storefront", "house.readers"], "language": "java" }
```

```json expect
{
  "claims": [
    { "kind": "file.exists", "element": "products-service", "outcome": "contradicted" },
    { "kind": "file.exists", "element": "orders-service", "outcome": "verified" }
  ],
  "findings": [
    { "rule": "claim/file.exists", "severity": "error", "subject": "products-service", "messageContains": "`ProductsService` isn't in the code." }
  ]
}
```

## owned-files

The standard User Interface designer (2.0.0): an element's Implementation stereotype lists the extra files it owns, each a path from the front end's own root. The engine's own rule reads `file.exists` from a `File` binding, with no house reader: each owned file must be in the code, at that path or one ending with it. The page's stylesheet is there and its test file isn't, so that one is contradicted.

```json case
{ "phase": 6, "kinds": ["file.exists"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.ui"], "language": "typescript" }
```

```json model
{
  "designers": {
    "User Interface": {
      "packages": [{
        "id": "storefront", "name": "Storefront", "type": "User Interface Package",
        "elements": [
          { "id": "pages", "type": "Folder", "name": "pages", "children": [
            { "id": "cart-page", "type": "Page", "name": "CartPage",
              "stereotypes": {
                "Page": { "Route": "/cart" },
                "Implementation": { "Owned Files": [ { "Path": "pages/CartPage.css" }, { "Path": "pages/CartPage.spec.ts" } ] }
              } }
          ] }
        ]
      }]
    },
    "Codebase Structure": {
      "packages": [{ "id": "root", "name": "shop", "type": "Root Folder",
        "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
    }
  }
}
```

```vue file src/pages/CartPage.vue
<script setup lang="ts">
const title = 'Cart';
</script>
```

```text file src/pages/CartPage.css
.cart { display: grid; }
```

```json expect
{
  "claims": [
    { "kind": "file.exists", "element": "cart-page", "key": "pages/CartPage.css", "outcome": "verified", "at": "src/pages/CartPage.css:1" },
    { "kind": "file.exists", "element": "cart-page", "key": "pages/CartPage.spec.ts", "outcome": "contradicted" }
  ],
  "findings": [
    { "rule": "claim/file.exists", "severity": "error", "subject": "cart-page" }
  ]
}
```
