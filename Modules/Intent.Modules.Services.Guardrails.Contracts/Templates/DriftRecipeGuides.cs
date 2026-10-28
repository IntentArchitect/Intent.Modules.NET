namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The configure-drift-check skill's guides to the Services designer's recipes for ASP.NET Core controllers, minimal
    /// APIs and FastEndpoints: a tested recipe file per technology, the decisions that change it, how each code form is
    /// modelled, and how to adapt it. Written beside the skill, in <c>configure-drift-check/references/</c>, and read
    /// only when the code uses the technology. The recipe files are tested against code in every style a guide covers
    /// (the test project's <c>ServicesGuideTests</c>), so change one only with its tests. Edit the text with a plain-text
    /// editor: Intent's file tools re-indent raw strings.
    /// </summary>
    internal static class DriftRecipeGuides
    {
        public const string AspNetCoreControllers = """"
# ASP.NET Core controllers: the Services recipe file

Start `guardrails/services.drift.json` from this guide when the code serves its HTTP endpoints from ASP.NET Core controllers. The recipe file below is tested against code in each style it covers. Copy it, make the decisions below, set `files` to the projects that hold the controllers and their base classes, then adapt it to this codebase's own conventions. Don't write the recipes again from scratch.

## Recognise it

Classes deriving from `ControllerBase` or `Controller`, usually with `[ApiController]`, whose actions have `[HttpGet]`, `[HttpPost]` and so on. Each controller is a Service in the model, named as the class is (`OrdersController`), and each action an operation.

## Decide before copying

| Question | How to tell | What to change |
| --- | --- | --- |
| Is every endpoint secured unless it says otherwise? | `AddAuthorization(o => o.FallbackPolicy = …)`, or `RequireAuthorization()` on `MapControllers()` | `security`'s `default` becomes `Secured`, and its `unstated` names the fallback policy |
| Does the code use its own authorization attributes? | Attributes deriving from `AuthorizeAttribute` or `IAuthorizationFilter` (`[Authorization]`, `[HasPermission(…)]`) | Add a read case to `security` for each that secures an endpoint, after `AllowAnonymous`'s: `{ "part": "declaration", "match": "[\\[,]Authorization\\b", "value": "Secured" }` |
| Do actions keep an `Async` suffix the route uses? | `[action]` in a route, with actions named `…Async` | Nothing, unless `SuppressAsyncSuffixInActionNames` is `false`: then the `[action]` replacement is right as it is. Otherwise add `|trimEnd:Async` to its `{Operation}` |

## The recipe file

```json
{
  "files": "src/**/*.cs",
  "facts": {
    "service.route": {
      "find": "class:{Service}",
      "read": [
        { "part": "declaration", "match": "[\\[,]Route\\(\"([^\"]*)\"" },
        { "part": "declaration", "match": "[\\[,]Route\\((?!\"|Name=|Order=)[^\\]]*\\]", "unreadable": true }
      ],
      "inherit": ["base"],
      "replace": [{ "match": "(?i)\\[controller\\]", "with": "{Service|trimEnd:Controller}" }],
      "default": "",
      "unstated": "no `[Route]` on the controller or its base classes"
    },
    "operation.verb": {
      "find": "class:{Service} > method:{Operation}",
      "read": [
        { "part": "declaration", "match": "[\\[,]Http(Get|Post|Put|Delete|Patch)\\b", "case": "upper" },
        { "part": "declaration", "match": "[\\[,]AcceptVerbs\\(\"(\\w+)\"", "case": "upper" }
      ]
    },
    "operation.route": {
      "find": "class:{Service} > method:{Operation}",
      "read": [
        { "part": "declaration", "match": "[\\[,]Route\\(\"([^\"]*)\"" },
        { "part": "declaration", "match": "[\\[,]Http\\w+\\(\"([^\"]*)\"" },
        { "part": "declaration", "match": "[\\[,](?:Route|Http\\w+)\\((?!\"|Name=|Order=)[^\\]]*\\]", "unreadable": true }
      ],
      "replace": [
        { "match": "(?i)\\[action\\]", "with": "{Operation}" },
        { "match": "(?i)\\[controller\\]", "with": "{Service|trimEnd:Controller}" }
      ],
      "default": "",
      "unstated": "no route template on the action"
    },
    "security": {
      "find": "class:{Service} > method:{Operation}",
      "read": [
        { "part": "declaration", "match": "[\\[,]AllowAnonymous\\b", "value": "Unsecured" },
        { "part": "declaration", "match": "[\\[,]Authorize\\b", "value": "Secured" }
      ],
      "inherit": ["class", "base"],
      "default": "None",
      "unstated": "no `[Authorize]` or `[AllowAnonymous]` on the action, its controller or their base classes"
    },
    "security.roles": {
      "find": "class:{Service} > method:{Operation}",
      "read": [{ "part": "declaration", "match": "[\\[,]Authorize\\([^\\]]*?Roles=\"([^\"]*)\"" }],
      "replace": [{ "match": "\\s", "with": "" }],
      "inherit": ["class", "base"],
      "default": "",
      "unstated": "no `Roles` in an `[Authorize]` on the action, its controller or their base classes"
    },
    "security.policy": {
      "find": "class:{Service} > method:{Operation}",
      "read": [
        { "part": "declaration", "match": "[\\[,]Authorize\\(\"([^\"]*)\"" },
        { "part": "declaration", "match": "[\\[,]Authorize\\([^\\]]*?Policy=\"([^\"]*)\"" }
      ],
      "replace": [{ "match": "\\s", "with": "" }],
      "inherit": ["class", "base"],
      "default": "",
      "unstated": "no policy in an `[Authorize]` on the action, its controller or their base classes"
    },
    "parameter.source": {
      "find": "class:{Service} > method:{Operation} > parameter:{Parameter}",
      "read": [
        { "match": "[\\[,]From(Body|Route|Query|Form|Header)\\b", "value": "From $1" },
        { "in": "method", "part": "declaration", "match": "(?i)\\{{Parameter}(?::[^}]*)?\\??\\}", "value": "From Route" },
        { "in": "class", "part": "declaration", "match": "(?i)\\{{Parameter}(?::[^}]*)?\\??\\}", "value": "From Route" },
        { "match": "^(?:IFormFile|IFormFileCollection)\\b", "value": "From Form" },
        { "match": "^(?:string|int|long|short|byte|bool|decimal|double|float|char|DateTime|DateTimeOffset|DateOnly|TimeOnly|TimeSpan|Guid)\\b", "value": "From Query" }
      ],
      "default": "From Body",
      "unstated": "no `[From…]` attribute or route parameter, and not a simple type, so `[ApiController]` binds it from the body"
    }
  },
  "own": [
    { "kind": "endpoint", "find": "class with (?:[\\[,]ApiController\\b|:\\w*Controller(?:Base)?\\b) > method with [\\[,](?:Http(?:Get|Post|Put|Delete|Patch)|AcceptVerbs)\\b", "name": "{Class}.{Method}" }
  ]
}
```

## How each form is modelled

The recipes read ASP.NET Core in the Services designer's terms, so the model must use the same terms. Model each form as this table says. The last column is where validate-drift-recipes should show each kind of fact's values coming from: one that comes from `default` where the table says code means the recipe doesn't read this codebase's form yet. Fix the recipe, not the model.

| Fact | Code form, and how it is modelled | Values come from |
| --- | --- | --- |
| `service.route` | The controller's `[Route("…")]`, or its base class's, with `[controller]` as the Service's name without `Controller`: `Http Service Settings` → `Route` | Code. `default` (empty) only for a controller with no route of its own |
| `operation.verb` | `[HttpGet]`, `[HttpPost]`, …: `Http Settings` → `Verb` | Code |
| `operation.route` | The action's `[Route("…")]` or `[HttpGet("…")]` template, relative to the controller's: `Http Settings` → `Route`, as written (`~/` and `/` make it absolute) | Code, or `default` (empty) for an action with no template |
| `security` | `[AllowAnonymous]`: `Unsecured`. `[Authorize]`, on the action, its controller or a base class: `Secured` | Code for every endpoint, unless nothing in the codebase secures one |
| `security.roles`, `security.policy` | `[Authorize(Roles = "…")]`, `[Authorize("…")]` or `[Authorize(Policy = "…")]`: `Secured` → `Roles` or `Policy` | Code |
| `parameter.source` | `[FromBody]`, `[FromRoute]`, …, else what `[ApiController]` infers: a route parameter `From Route`, a simple type `From Query`, `IFormFile` `From Form`, anything else `From Body`. Model the source explicitly, never `Default` | Code, or `default` for a complex type with no attribute |

## Adapting to house style

| You see | Change |
| --- | --- |
| A route from a constant (`[Route(Routes.Orders)]`) | Nothing: the `unreadable` cases report it as can't be verified |
| Several `[Authorize]` attributes on one endpoint, each naming a role or policy | The recipes read the first: give the endpoint an `unreadable` case for the second (`[\\[,]Authorize\\([^\\]]*Roles=[^\\]]*\\]\\[Authorize\\(`) |
| An enum parameter with no attribute | `[ApiController]` binds it from the query string, but the recipe can't tell an enum from a class: add a read case for this codebase's enums before the default, such as `^(?:OrderStatus|Priority)\\b`, value `From Query` |
| API versioning (`[ApiVersion("1.0")]`, `api/v{version:apiVersion}/[controller]`) | Nothing: routes are read as written, so model them that way |

## Ownership

The `own` entry reports every action of a controller the model doesn't describe: a class with `[ApiController]`, or one deriving from a `…Controller` or `…ControllerBase` class.

## What it can't read

- A route, role or policy built at run time (from a constant or a convention): the `unreadable` cases report the routes; give the others one too, and tell the developer.
"""";

        public const string MinimalApis = """"
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
"""";

        public const string FastEndpoints = """"
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
"""";
    }
}
