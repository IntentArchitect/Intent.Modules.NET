### Version 1.2.0

- New Feature: Gateway routes are secured by default. `ocelot.json` gets a global `Bearer` authentication setting, so the gateway application must register an authentication scheme named `Bearer`. Apply `Unsecured` to an API Gateway Route to open that route (`"AllowAnonymous": true`), or to the gateway's Services package to open every route. Route-level `Unsecured` needs `Intent.Metadata.Security` 1.1.7 or later, now the minimum this module installs.
- New Feature: `Rate Limit` stereotype on API Gateway Routes. It only marks the route: all marked routes share one rule in `ocelot.json`, `GlobalConfiguration.RateLimitOptions`, which lists them by `Key`. The rule's values (`Limit`, `Period`, `ClientIdHeader`, `ClientWhitelist`) are written with starting values (100 requests per minute) only when missing, so you set them in `ocelot.json` and they are kept. Unmarking the last route removes the rule, values included. Callers of a limited route must send the client header (`Oc-Client` by default).
- Improvement: Hand edits to `ocelot.json` now survive regeneration. Options and comments you add to a route stay with it when routes are added, removed or reordered, and sections and `GlobalConfiguration` keys you add are kept. Values generated from the model stay in step with it; mark one `// IntentIgnore` to keep your own value, or a hand-written route to keep it. The file is re-indented with 2 spaces on every run, and a file that is not valid JSON stops the Software Factory with an error giving its line, instead of being overwritten. No extra module is needed.
- Improvement: Each generated route has a `Key`, set to the route's name, so Ocelot aggregates can reference it. Two API Gateway Routes with the same name fail the Software Factory with an error on the route, instead of generating duplicate keys.
- Improvement: A route such as `/api/customers` gets `"Priority": 2` when a route with the same verb has `/api/customers/{id}`, so the bare path is no longer served by the placeholder route.
- Improvement: Installing this module now also brings `Intent.Metadata.ApiGateway` up to 1.0.0 or later, so **Manage Gateway Routes...** shows each route as secured or unsecured, and marks rate-limited routes.
- Fixed: A route whose target has no Http Settings fails the Software Factory with an error naming the route and the endpoint, instead of a `NullReferenceException`.

### Version 1.1.0

- Improvement: Updated NuGet package versions:
  - `Ocelot` 24.1.0 → 25.0.2 (net10.0, net9.0, net8.0)

  Ocelot 25 is a major release that adds .NET 10 and moves its extension packages into their own repositories. The generated gateway code is unaffected; custom code that uses Ocelot directly should be checked against the [Ocelot 25.0.0 release notes](https://github.com/ThreeMammals/Ocelot/releases/tag/25.0.0).
- New Feature: Initial release of Ocelot API Gateway module.
