---
contentHash: 280DAFC17D4DA05989DC0FD8E8500ECE8B51D06ED0A4F1C58A51E80B8794616C
---
# Minimal APIs: the Services recipe file

Start `guardrails/services.drift.json` from this guide when the code maps its HTTP endpoints with ASP.NET Core minimal APIs. The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, set `files` to the projects that map the endpoints, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

`app.MapGet(…)`, `MapPost`, `MapPut`, `MapDelete` and `MapPatch` calls, often on a route group (`var group = app.MapGroup("api/orders")`). Endpoints come in three forms, which the recipes cover:

- **A handler method**, mapped by name: `group.MapGet("/{id}", GetOrder)`, with `GetOrder` a method of the same class. The class is a Service in the model, its group's prefix the Service's route, and each handler an operation whose route is relative to it.
- **A named lambda:** `group.MapGet("/{id}", async (int id, IOrderService orders) => …).WithName("GetOrder")`. Also a Service operation, named by `WithName`.
- **A lambda that takes a Command or Query:** `group.MapPost("/", async (CreateOrderCommand command, ISender sender) => …)`. The Command or Query is the endpoint, with its full route: the group's prefix and the call's route together.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Is every endpoint secured unless it says otherwise? | `AddAuthorization(o => o.FallbackPolicy = …)`, or `RequireAuthorization()` on a group every endpoint is in | `security`'s `default` becomes `Secured`, and its `unstated` names the fallback policy |
| Is a group made in one place and its endpoints mapped in another (`app.MapGroup("api").MapOrderEndpoints()`)? | A `MapGroup` whose result is passed to, or extended by, another method | A Service's route: give `service.route` that prefix as its `absent`. A Command's or Query's: add a `replace` to `operation.route`, `{ "match": "^", "with": "api/" }`, when every one shares the prefix |

## The recipe file

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "service.route": {
      "find": "class:{Service} > statement with \\.MapGroup\\(",
      "read": [
        { "match": "\\.MapGroup\\(\"/?([^\"]*)\"\\)" },
        { "match": "\\.MapGroup\\((?!\")[^)]*\\)", "unreadable": true }
      ],
      "absent": "",
      "unstated": "no `MapGroup` in `{Service}`, so its endpoints' routes are their own"
    },
    "operation.verb": {
      "find": [
        "class:{Service} > statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",(?:\\w+\\.)*{Operation}\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\([^;]*?\\.WithName\\(\"{Operation}\"\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",[^;]*?[(,\\]]{Operation} \\w+[,)]"
      ],
      "read": [{ "match": "\\.Map(Get|Post|Put|Delete|Patch)\\(", "case": "upper" }]
    },
    "operation.route": {
      "find": [
        "class:{Service} > statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",(?:\\w+\\.)*{Operation}\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\([^;]*?\\.WithName\\(\"{Operation}\"\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",[^;]*?[(,\\]]{Operation} \\w+[,)]"
      ],
      "read": [
        { "in": "method", "match": "\\.MapGroup\\(\"/?([^\"]+)\"\\)(?:(?!\\.MapGroup\\().)*?\\.Map(?:Get|Post|Put|Delete|Patch)\\(\"/?([^\"]*)\",[^;]*?[(,\\]]{Operation} \\w+[,)]", "value": "$1/$2" },
        { "match": "\\.Map\\w+\\(\"/?([^\"]*)\"" },
        { "match": "\\.Map\\w+\\((?!\")[^,]*,", "unreadable": true }
      ],
      "replace": [{ "match": "/$", "with": "" }]
    },
    "security": {
      "find": [
        "class:{Service} > statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",(?:\\w+\\.)*{Operation}\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\([^;]*?\\.WithName\\(\"{Operation}\"\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",[^;]*?[(,\\]]{Operation} \\w+[,)]"
      ],
      "read": [
        { "in": "statement", "match": "\\.AllowAnonymous\\(\\)", "value": "Unsecured" },
        { "in": "statement", "match": "\\.RequireAuthorization\\(", "value": "Secured" },
        { "part": "declaration", "match": "[\\[,]AllowAnonymous\\b", "value": "Unsecured" },
        { "part": "declaration", "match": "[\\[,]Authorize\\b", "value": "Secured" },
        { "match": "\\.MapGroup\\([^;]*?\\.AllowAnonymous\\(\\)(?:(?!\\.MapGroup\\().)*?\\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\"[^;]*?(?:[(,\\]](?:\\w+\\.)*{Operation}(?: \\w+)?[,)]|\\.WithName\\(\"{Operation}\"\\))", "value": "Unsecured" },
        { "match": "\\.MapGroup\\([^;]*?\\.RequireAuthorization\\((?:(?!\\.MapGroup\\().)*?\\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\"[^;]*?(?:[(,\\]](?:\\w+\\.)*{Operation}(?: \\w+)?[,)]|\\.WithName\\(\"{Operation}\"\\))", "value": "Secured" }
      ],
      "inherit": ["method", "class:{Service} > method:{Operation}"],
      "default": "None",
      "unstated": "no `RequireAuthorization` or `AllowAnonymous` on the endpoint or its group, and no `[Authorize]` on its handler"
    },
    "security.roles": {
      "find": [
        "class:{Service} > statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",(?:\\w+\\.)*{Operation}\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\([^;]*?\\.WithName\\(\"{Operation}\"\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",[^;]*?[(,\\]]{Operation} \\w+[,)]"
      ],
      "read": [
        { "in": "statement", "match": "\\.RequireAuthorization\\(\\w+=>\\w+\\.RequireRole\\(((?:\"[^\"]*\",)*\"[^\"]*)\"\\)" },
        { "part": "declaration", "match": "[\\[,]Authorize\\([^\\]]*?Roles=\"([^\"]*)\"" },
        { "match": "\\.MapGroup\\([^;]*?\\.RequireAuthorization\\(\\w+=>\\w+\\.RequireRole\\(((?:\"[^\"]*\",)*\"[^\"]*)\"\\)(?:(?!\\.MapGroup\\().)*?\\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\"[^;]*?(?:[(,\\]](?:\\w+\\.)*{Operation}(?: \\w+)?[,)]|\\.WithName\\(\"{Operation}\"\\))" }
      ],
      "replace": [{ "match": "[\"\\s]", "with": "" }],
      "inherit": ["method", "class:{Service} > method:{Operation}"],
      "default": "",
      "unstated": "no `RequireRole` on the endpoint or its group, and no `Roles` on its handler's `[Authorize]`"
    },
    "security.policy": {
      "find": [
        "class:{Service} > statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",(?:\\w+\\.)*{Operation}\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\([^;]*?\\.WithName\\(\"{Operation}\"\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",[^;]*?[(,\\]]{Operation} \\w+[,)]"
      ],
      "read": [
        { "in": "statement", "match": "\\.RequireAuthorization\\(((?:\"[^\"]*\",)*\"[^\"]*)\"\\)" },
        { "part": "declaration", "match": "[\\[,]Authorize\\((?:Policy=)?\"([^\"]*)\"" },
        { "match": "\\.MapGroup\\([^;]*?\\.RequireAuthorization\\(((?:\"[^\"]*\",)*\"[^\"]*)\"\\)(?:(?!\\.MapGroup\\().)*?\\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\"[^;]*?(?:[(,\\]](?:\\w+\\.)*{Operation}(?: \\w+)?[,)]|\\.WithName\\(\"{Operation}\"\\))" }
      ],
      "replace": [{ "match": "[\"\\s]", "with": "" }],
      "inherit": ["method", "class:{Service} > method:{Operation}"],
      "default": "",
      "unstated": "no policy in `RequireAuthorization` on the endpoint or its group, or in its handler's `[Authorize]`"
    },
    "parameter.source": {
      "find": [
        "class:{Service} > statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",(?:\\w+\\.)*{Operation}\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\([^;]*?\\.WithName\\(\"{Operation}\"\\)",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",[^;]*?[(,\\]]{Operation} \\w+[,)]"
      ],
      "read": [
        { "match": "[\\[,]From(Body|Route|Query|Form|Header)\\b", "value": "From $1" },
        { "match": "(?<!\\[AsParameters\\])[(,]{Operation} \\w+[,)]", "value": "From Body" },
        { "match": "(?<=\\(\"[^\"]*)(?i:\\{{Parameter}(?::[^}]*)?\\??\\})", "value": "From Route" },
        { "match": "^(?:\\[[^\\]]*\\])*(?:IFormFile|IFormFileCollection)\\b", "value": "From Form" },
        { "match": "(?<=^(?:\\[[^\\]]*\\])*(?:public (?:required )?)?)(?:string|int|long|short|byte|bool|decimal|double|float|char|DateTime|DateTimeOffset|DateOnly|TimeOnly|TimeSpan|Guid)\\b", "value": "From Query" }
      ],
      "inherit": [
        "class:{Service} > method:{Operation} > parameter:{Parameter}",
        "statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\([^;]*?\\.WithName\\(\"{Operation}\"\\) > parameter:{Parameter}",
        "class:{Operation} > property:{Parameter}"
      ],
      "default": "From Body",
      "unstated": "no `[From…]` attribute or route parameter, and not a simple type, so minimal APIs bind it from the body"
    }
  },
  "own": [
    { "kind": "endpoint", "find": "statement with \\.Map(Get|Post|Put|Delete|Patch)\\(\"([^\"]*)\"", "name": "{Class}: Map$1 $2" }
  ]
}
```

## How each form is modelled

The recipes read minimal APIs in the Services designer's terms, so the model must use the same terms. Model each form as this table says. The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` or `absent` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `service.route` | The class's `MapGroup("…")` prefix, without a leading `/`: `Http Service Settings` → `Route` | Code, or `absent` (empty) for a class with no group |
| `operation.verb` | `MapGet`, `MapPost`, …: `Http Settings` → `Verb` | Code |
| `operation.route` | A Service operation: its call's route, relative to the group, without a leading `/`. A Command or Query: the group's prefix and the call's route together (`api/orders/{id}`). `Http Settings` → `Route` | Code |
| `security` | `AllowAnonymous()` or `[AllowAnonymous]`: `Unsecured`. `RequireAuthorization(…)` on the endpoint or its group, or `[Authorize]` on its handler: `Secured` | Code for every endpoint, unless nothing in the codebase secures one |
| `security.roles`, `security.policy` | `RequireAuthorization(p => p.RequireRole("…"))`, `RequireAuthorization("policy")`, `[Authorize(Roles = "…")]`: `Secured` → `Roles` or `Policy` | Code |
| `parameter.source` | `[FromBody]`, `[FromRoute]`, …; else a route parameter `From Route`, a simple type `From Query`, `IFormFile` `From Form`, anything else `From Body`. A Command or Query taken whole is `From Body` for every field, unless it is `[AsParameters]`. Model the source explicitly, never `Default` | Code, or `default` for a complex type with no attribute |

## Adapting to house style

| You see | Change |
| --- | --- |
| A route from a constant (`group.MapGet(Routes.Order, GetOrder)`) | Nothing: the `unreadable` case reports it as can't be verified |
| Several groups in one class | `service.route` matches several places: the endpoints in it are better modelled as one Service per group, each in a class of its own. Where the code can't be split, give `service.route` an `unreadable` case and tell the developer |
| A lambda that calls a Service's method (`group.MapGet("/{id}", (int id, IOrderService s) => s.GetOrder(id))`) with no `WithName` | A find path before the others: `class:{Service} > statement with \\.Map(?:Get|Post|Put|Delete|Patch)\\([^;]*?\\.{Operation}\\(` |
| `MapMethods(…, new[] { "PATCH" })` | A read case for `operation.verb`: `\\.MapMethods\\(\"[^\"]*\",new\\[\\]\\{\"(\\w+)\"`, `"case": "upper"`, and the same find paths widened to `MapMethods` |

## Ownership

The `own` entry reports every `Map…` call the model doesn't describe.

## What it can't read

- A route group made in one file and its endpoints mapped in another, with prefixes that differ between groups: a Command's or Query's full route can't be read as one value. Give `operation.route` an `unreadable` case for those calls and tell the developer.
