# proxy.verb

The HTTP verb a front end's proxy operation sends, against the backend Operation it calls. In the Java spike's Front End designer, a Proxy Operation has a Calls association to a Services designer Operation, whose Http Settings stereotype's Verb is the model's value. The kind is `stated: always` and compares exactly.

The kind is `later`, so these cases' house pack gives the kind, the Front End designer's vocabulary and the rule. The claim sits on the Calls association. The rule reads the first argument of the `request('VERB', path)` call in the proxy method, in the Proxy's file, as text.

## verified

The storefront's API client reads one product with a `GET`, and the called backend Operation's Http Settings Verb is `GET`.

```json case
{ "phase": "later", "kinds": ["proxy.verb"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R28", "R30", "R38"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "typescript" }
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
    { "kind": "proxy.verb", "label": "proxy verb", "valueType": "text", "comparer": "exact", "stated": "always", "existence": false }
  ],
  "vocabulary": {
    "Front End": {
      "entries": [
        {
          "kind": "proxy.verb",
          "for": { "association": "Calls" },
          "value": "target.element.stereotype('Http Settings').Verb",
          "bindings": {
            "File": "source.element.parent.stereotype('Implementation').Path",
            "Operation": "source.element.name"
          },
          "own": ["Operation"],
          "source": "the called Operation's Http Settings Verb"
        }
      ]
    }
  },
  "rules": {
    "Front End": {
      "files": ["src/**/*.ts"],
      "facts": {
        "proxy.verb": {
          "find": "file:{File}",
          "read": [{ "match": "\\b{Operation}\\([^)]*\\):[^{]*\\{return request<[^>]*>\\('(\\w+)'" }],
          "unstated": "no `request('VERB', …)` in `{Operation}` in `{File}`"
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
    { "kind": "proxy.verb", "element": "get-product-by-id-calls-backend", "outcome": "verified",
      "expected": "GET", "actual": "GET", "at": "src/api/client.ts:10" }
  ]
}
```

## contradicted

The proxy method now sends a `POST`, and the backend Operation it calls answers `GET`.

```json case
{ "phase": "later", "base": "verified", "kinds": ["proxy.verb"], "traces": ["JAV-FE-11"], "requirements": ["R2", "R14"], "packs": ["intent.core", "intent.java", "house.shop"], "language": "typescript" }
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
        return request<ProductDto>('POST', `/api/products/${encodeURIComponent(id)}`);
    },
};
```

```json expect
{
  "claims": [
    { "kind": "proxy.verb", "element": "get-product-by-id-calls-backend", "outcome": "contradicted",
      "expected": "GET", "actual": "POST", "at": "src/api/client.ts:10" }
  ],
  "findings": [
    { "rule": "claim/proxy.verb", "severity": "error", "subject": "get-product-by-id-calls-backend" }
  ]
}
```
