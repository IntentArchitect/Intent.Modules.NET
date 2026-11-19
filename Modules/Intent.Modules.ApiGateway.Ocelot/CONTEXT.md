# Intent.ApiGateway.Ocelot — Context

Design decisions and invariants behind this module that are not visible in the code.

## `ocelot.json` is merged, not overwritten (1.2.0)

Users hand-edit `ocelot.json` (authentication, rate limiting, priorities), so regeneration must keep those edits.

- **Mechanism:** the template calls `WithJsonWriter()`, which hands the file to the `Intent.Code.Weaving.Json` weaver. Without that module installed the whole file is overwritten, so it is a hard `.imodspec` dependency.
- **Routes are matched on `UpstreamPathTemplate` + `DownstreamHttpMethod`**, via a `// IntentMatchBy("UpstreamPathTemplate", "DownstreamHttpMethod")` comment above `"Routes"` in the template output. Without it the weaver matches array items by position, so removing one route shifts every hand edit onto its neighbour.
- **Why not match on `Key`:** tested in a harness (evidence `2026-11-19-weaver-key-matching`). Matching by `Key` loses a route's edits when the route is renamed, and on upgrade from a file with no keys it duplicates every route, which Ocelot rejects at startup. Path + verb passes every scenario, including the upgrade, where `Key` is added in place.
- **Known limit:** changing a route's upstream path or downstream verb in the designer makes it a new route; its hand edits are dropped. Accepted: those are the route's identity.
- The weaver strips `IntentMatchBy` comments when it creates a new file and reads them from the template output on every run, so the comment never needs to be on disk.

## `Key` per route (1.2.0)

Each route gets `"Key": <route element name>`, so aggregates (`RouteKeys`) can reference it. The designer derives route names from the target endpoint, so two routes to the same endpoint share a name; the template throws an `ElementException` on a duplicate rather than emit duplicate keys.

## `CommentingJsonWriter` (stand-in, retire when possible)

`DataFileJsonWriter` in Intent.Modules.Common (checked up to 3.11.9) ignores `IDataFileValue.Comment`, so the template could not emit the `IntentMatchBy` comment. `CommentingJsonWriter` subclasses it and writes comments for the root object's fields only.

**Retire it** once Common's `DataFileJsonWriter` writes `Comment` itself: switch back to plain `WithJsonWriter()` and delete the class.
