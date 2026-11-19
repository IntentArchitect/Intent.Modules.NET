### Version 1.2.0

- Improvement: Hand edits to `ocelot.json` (authentication, rate limiting, priorities and so on) now survive regeneration and stay with their route when routes are added or removed. Routes are matched on their upstream path and downstream HTTP method. This module now depends on `Intent.Code.Weaving.Json`.
- Improvement: Each generated route now has a `Key`, set to the route's name, so it can be referenced from Ocelot aggregates.
- Improvement: Two API Gateway Routes with the same name now fail the Software Factory with an error on the route, instead of generating duplicate keys.

### Version 1.1.0

- Improvement: Updated NuGet package versions:
  - `Ocelot` 24.1.0 → 25.0.2 (net10.0, net9.0, net8.0)

  Ocelot 25 is a major release that adds .NET 10 and moves its extension packages into their own repositories. The generated gateway code is unaffected; custom code that uses Ocelot directly should be checked against the [Ocelot 25.0.0 release notes](https://github.com/ThreeMammals/Ocelot/releases/tag/25.0.0).
- New Feature: Initial release of Ocelot API Gateway module.
