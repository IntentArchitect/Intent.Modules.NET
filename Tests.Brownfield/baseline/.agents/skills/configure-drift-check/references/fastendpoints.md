---
contentHash: E4C36F856CE36D205782CCC240844F812D20497C17787507F5A95693D5258A17
---
# FastEndpoints: the Services recipe file

Start `guardrails/services.drift.json` from this guide when the code serves its HTTP endpoints with FastEndpoints. The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, set `files` to the projects that hold the endpoint classes and their request types, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

A `FastEndpoints` package, and classes deriving from `Endpoint<TRequest>`, `Endpoint<TRequest, TResponse>` or `EndpointWithoutRequest<TResponse>`, each configuring one endpoint in its `Configure()` method (`Post("/orders")`, `Roles("Admin")`, `AllowAnonymous()`). Each endpoint is a Command or Query in the model: the request type (`CreateOrderCommand`), or, for an endpoint with no request, a Query named as the class is without `Endpoint`. Its fields are the request type's properties.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| A global route prefix? | `app.UseFastEndpoints(c => c.Endpoints.RoutePrefix = "api")` | Add a `replace` to `operation.route`: `{ "match": "^", "with": "api/" }` |
| Endpoint groups with a prefix of their own? | `Group<OrdersGroup>()` in `Configure`, with `class OrdersGroup : Group` calling `Configure("orders", …)` | The recipe reports those routes as can't be verified (`unreadable`): the route is split between two classes. Tell the developer |

## The recipe file

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "operation.verb": {
      "find": ["class with :\\w*Endpoint<{Operation}[,>] > method:Configure", "class:{Operation}Endpoint > method:Configure"],
      "read": [
        { "match": "\\b(Get|Post|Put|Delete|Patch)\\(\"", "case": "upper" },
        { "match": "\\bVerbs\\(Http\\.(\\w+)", "case": "upper" }
      ]
    },
    "operation.route": {
      "find": ["class with :\\w*Endpoint<{Operation}[,>] > method:Configure", "class:{Operation}Endpoint > method:Configure"],
      "read": [
        { "match": "\\bGroup<\\w+>\\(\\)", "unreadable": true },
        { "match": "\\b(?:Get|Post|Put|Delete|Patch|Routes)\\(\"/?([^\"]*)\"" },
        { "match": "\\b(?:Get|Post|Put|Delete|Patch|Routes)\\((?!\")[^)]*\\)", "unreadable": true }
      ]
    },
    "security": {
      "find": ["class with :\\w*Endpoint<{Operation}[,>] > method:Configure", "class:{Operation}Endpoint > method:Configure"],
      "read": [{ "match": "\\bAllowAnonymous\\(", "value": "Unsecured" }],
      "default": "Secured",
      "unstated": "no `AllowAnonymous()` in `Configure`, so FastEndpoints requires an authenticated user"
    },
    "security.roles": {
      "find": ["class with :\\w*Endpoint<{Operation}[,>] > method:Configure", "class:{Operation}Endpoint > method:Configure"],
      "read": [{ "match": "\\bRoles\\(((?:\"[^\"]*\",)*\"[^\"]*)\"\\)" }],
      "replace": [{ "match": "[\"\\s]", "with": "" }],
      "default": "",
      "unstated": "no `Roles(…)` in `Configure`"
    },
    "security.policy": {
      "find": ["class with :\\w*Endpoint<{Operation}[,>] > method:Configure", "class:{Operation}Endpoint > method:Configure"],
      "read": [{ "match": "\\bPolicies\\(((?:\"[^\"]*\",)*\"[^\"]*)\"\\)" }],
      "replace": [{ "match": "[\"\\s]", "with": "" }],
      "default": "",
      "unstated": "no `Policies(…)` in `Configure`"
    },
    "parameter.source": {
      "find": "class:{Operation} > property:{Parameter}",
      "read": [
        { "part": "declaration", "match": "[\\[,]FromHeader\\b", "value": "From Header" },
        { "part": "declaration", "match": "[\\[,]QueryParam\\b", "value": "From Query" },
        { "part": "declaration", "match": "[\\[,]RouteParam\\b", "value": "From Route" },
        { "part": "declaration", "match": "[\\[,]FromBody\\b", "value": "From Body" },
        { "part": "declaration", "match": "[\\[,]FromForm\\b", "value": "From Form" },
        { "match": "(?<=\\(\"[^\"]*)(?i:\\{{Parameter}(?::[^}]*)?\\??\\})", "value": "From Route" },
        { "match": "\\b(?:Get|Delete)\\(\"", "value": "From Query" }
      ],
      "inherit": ["class with :\\w*Endpoint<{Operation}[,>] > method:Configure"],
      "default": "From Body",
      "unstated": "no binding attribute or route parameter for `{Parameter}`, and the endpoint isn't a GET or DELETE, so FastEndpoints binds it from the JSON body"
    }
  },
  "own": [
    { "kind": "endpoint", "find": "class with :\\w*Endpoint(?:WithoutRequest)?<", "name": "{Class}" }
  ]
}
```

## How each form is modelled

The recipes read FastEndpoints in the Services designer's terms, so the model must use the same terms. Model each form as this table says. The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `operation.verb` | `Get(…)`, `Post(…)`, … or `Verbs(Http.GET)` in `Configure`: `Http Settings` → `Verb` | Code |
| `operation.route` | The route in `Configure`, without a leading `/`: `Http Settings` → `Route` | Code |
| `security` | `AllowAnonymous()`: `Unsecured`. Anything else: `Secured`, since FastEndpoints secures every endpoint unless it says otherwise | Code for `Unsecured`, `default` for `Secured` |
| `security.roles`, `security.policy` | `Roles("…")`, `Policies("…")`: `Secured` → `Roles` or `Policy` | Code |
| `parameter.source` | `[FromHeader]`, `[QueryParam]`, `[RouteParam]`, `[FromBody]`, `[FromForm]` on the request's property; else a route parameter `From Route`, a field of a GET or DELETE `From Query`, anything else `From Body`. Model the source explicitly, never `Default` | Code, or `default` for a field bound from the body |

## Adapting to house style

| You see | Change |
| --- | --- |
| A base endpoint class (`class CreateOrderEndpoint : ApiEndpoint<CreateOrderCommand, OrderDto>`) | Nothing: the finds accept any base class whose name ends in `Endpoint` |
| `Permissions(…)`, `Claims(…)` or `AuthSchemes(…)` | They secure the endpoint, which `default` already reads as `Secured`; the model has no field for them |
| A route from a constant (`Post(Routes.Orders)`) | Nothing: the `unreadable` case reports it as can't be verified |

## Ownership

The `own` entry reports every endpoint class the model doesn't describe.

## What it can't read

- A route split between an endpoint and its `Group<T>` (above).
- `Configure` that calls a helper to set the route or security: give the fact an `unreadable` case for the call and tell the developer.
