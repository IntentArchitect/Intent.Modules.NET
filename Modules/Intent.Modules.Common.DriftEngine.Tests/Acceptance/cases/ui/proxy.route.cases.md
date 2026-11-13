# proxy.route

The path a front end's proxy operation sends its request to, against the route of the backend Operation it calls: the Service's Http Service Settings Route followed by the Operation's Http Settings Route, with `[controller]` standing for the Service's name without its `Service` suffix. In the Java spike's Front End designer, a Proxy Operation has a Calls association to that Operation. The kind is `stated: always` and compares with the `route` comparer, which ignores a leading or trailing `/` and case in literal segments.

The kind is `later`, so these cases' house pack gives the kind, the Front End designer's vocabulary and the rule. The claim sits on the Calls association. The rule reads the path argument of the `request('VERB', path)` call in the proxy method, as text, and turns each `${encodeURIComponent(name)}` hole into the route parameter `{name}`.

## verified

The client builds `/api/products/${encodeURIComponent(id)}`, which reads as `/api/products/{id}`. The model's route is `api/[controller]` on `ProductsService` and `{id}` on its Operation, so `api/Products/{id}`; the two differ only in the leading `/` and the literal segment's case.

```json case
{ "phase": "later", "kinds": ["proxy.route"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R10", "R28", "R30"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "typescript" }
```

```json pack
{
  "id": "house.shop",
  "version": "1.0.0",
  "engine": "^1.0.0",
  "requires": [{ "id": "intent.java", "version": "^1.0.0" }],
  "languages": ["typescript"],
  "designers": ["Front End"],
  "kinds": [
    { "kind": "proxy.route", "label": "proxy route", "valueType": "route", "comparer": "route", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "proxy.route",
          "for": { "association": "Calls" },
          "value": "replace(target.element.parent.stereotype('Http Service Settings').Route, '[controller]', trimEnd(target.element.parent.name, 'Service')) + '/' + target.element.stereotype('Http Settings').Route",
          "bindings": {
            "File": "source.element.parent.stereotype('Implementation').Path",
            "Operation": "source.element.name"
          },
          "own": ["Operation"],
          "source": "the called Operation's Service route and Http Settings Route"
        }
      ]
    }
  },
  "rules": {
    "Front End": {
      "files": ["src/**/*.ts"],
      "facts": {
        "proxy.route": {
          "find": "file:{File}",
          "read": [{ "match": "\\b{Operation}\\([^)]*\\):[^{]*\\{return request<[^>]*>\\('\\w+',[`']([^`']*)[`']" }],
          "replace": [{ "match": "\\$\\{encodeURIComponent\\((\\w+)\\)\\}", "with": "{$1}" }],
          "unstated": "no `request(…, path)` in `{Operation}` in `{File}`"
        }
      }
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
          { "id": "api-proxy", "type": "Proxy", "name": "api",
            "stereotypes": { "Implementation": { "Path": "src/api/client.ts" } },
            "children": [
              { "id": "api-get-product-by-id", "type": "Proxy Operation", "name": "getProductById",
                "children": [
                  { "id": "api-get-product-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "string" } }
                ] }
            ] }
        ]
      }],
      "associations": [
        { "id": "get-product-by-id-calls-backend", "type": "Calls",
          "source": { "element": "api-get-product-by-id" }, "target": { "element": "products-get-product-by-id" } }
      ]
    },
    "Services": {
      "packages": [{
        "id": "catalog-services", "name": "Catalog",
        "elements": [
          { "id": "products-service", "type": "Service", "name": "ProductsService",
            "stereotypes": { "Http Service Settings": { "Route": "api/[controller]" } },
            "children": [
              { "id": "products-get-product-by-id", "type": "Operation", "name": "GetProductById",
                "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "{id}" } },
                "children": [
                  { "id": "products-get-product-by-id-id", "type": "Parameter", "name": "id", "typeRef": { "type": "guid" } }
                ] }
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

```typescript file src/api/client.ts
import type { ProductDto } from './types';

async function request<T>(method: string, path: string, body?: unknown): Promise<T> {
    const res = await fetch(path, { method, body: body === undefined ? undefined : JSON.stringify(body) });
    return (await res.json()) as T;
}

export const api = {
    // GET /api/products/{id}
    getProductById(id: string): Promise<ProductDto> {
        return request<ProductDto>('GET', `/api/products/${encodeURIComponent(id)}`);
    },
};
```

```json expect
{
  "claims": [
    { "kind": "proxy.route", "element": "get-product-by-id-calls-backend", "outcome": "verified",
      "expected": "api/Products/{id}", "at": "src/api/client.ts:10" }
  ]
}
```

## contradicted

The client now sends to `/api/product/…`, a segment the backend's route doesn't have.

```json case
{ "phase": "later", "base": "verified", "kinds": ["proxy.route"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "typescript" }
```

```typescript file src/api/client.ts
import type { ProductDto } from './types';

async function request<T>(method: string, path: string, body?: unknown): Promise<T> {
    const res = await fetch(path, { method, body: body === undefined ? undefined : JSON.stringify(body) });
    return (await res.json()) as T;
}

export const api = {
    // GET /api/products/{id}
    getProductById(id: string): Promise<ProductDto> {
        return request<ProductDto>('GET', `/api/product/${encodeURIComponent(id)}`);
    },
};
```

```json expect
{
  "claims": [
    { "kind": "proxy.route", "element": "get-product-by-id-calls-backend", "outcome": "contradicted",
      "expected": "api/Products/{id}", "at": "src/api/client.ts:10" }
  ],
  "findings": [
    { "rule": "claim/proxy.route", "severity": "error", "subject": "get-product-by-id-calls-backend" }
  ]
}
```
