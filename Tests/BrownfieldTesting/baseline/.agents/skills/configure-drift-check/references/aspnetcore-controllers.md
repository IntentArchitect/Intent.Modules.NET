---
contentHash: 812AAFF6CC4A92E93E8886201634B50BECD847536F75539806322C5E0BD32188
---
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
