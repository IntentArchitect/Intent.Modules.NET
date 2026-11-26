# Intent.ApiGateway.Ocelot — Context

Design decisions and invariants behind this module that are not visible in the code.

## `ocelot.json` is merged, not overwritten (1.2.0)

Users hand-edit `ocelot.json` (authentication, rate limiting, priorities), so regeneration must keep those edits.

- **Mechanism: the template merges the file itself, with no other module** (the user's call, 2026-11-24: "make the ocelot.json template more clever so that it does some form of weaving"). The template does not call `WithJsonWriter()`, so the file is `CodeGenType.Basic` and the Software Factory writes exactly what the template returns. `WithWriter(() => new OcelotJsonWriter(...))` turns the Data File Builder's tree into the merged text: it reads the file on disk (`TryGetExistingFileContent`, in the root-object callback), parses it with comments (`JsoncParser`), merges (`OcelotJsonMerger`) and writes it back (`JsoncWriter`). Before this, 1.2.0-pre.0 depended on `Intent.Code.Weaving.Json`; that was dropped because the user wanted no weaver dependency, and owning the merge also lets the template seed rate-limit values only when missing, which the weaver could not do.
- **Ownership is per key, written down in `OcelotJsonMerger`.** On a matched route the model owns `Key`, the upstream and downstream paths and verbs, `DownstreamHostAndPorts` and `Priority` (set when generated, removed when not), and `AuthenticationOptions.AllowAnonymous` only, so a hand `AllowedScopes` beside it survives and an object left empty is dropped. Every other key and comment on the route is kept. `GlobalConfiguration` keeps hand keys such as `BaseUrl`, and the model owns `AuthenticationProviderKeys` and the rate limit's `EnableRateLimiting` and `RouteKeys` (see Rate Limit). Top-level keys the template does not write (`Aggregates`, ...) are kept; a generated key missing from disk is inserted after its preceding generated sibling.
- **Routes are matched on `UpstreamPathTemplate` + `DownstreamHttpMethod`** (case-insensitive), and written in the model's order. A route on disk with no match is removed, unless it carries `// IntentIgnore`, in which case it is re-inserted after the route it followed.
- **Why not match on `Key`:** tested in a harness (evidence `2026-11-19-weaver-key-matching`). Matching by `Key` loses a route's edits when the route is renamed, and on upgrade from a file with no keys it duplicates every route, which Ocelot rejects at startup. Path + verb passes every scenario, including the upgrade, where `Key` is added in place. The self-merge kept the same identity for that reason.
- **Known limit:** changing a route's upstream path or downstream verb in the designer makes it a new route; its hand edits are dropped. Accepted: those are the route's identity.
- **`// IntentIgnore` (or `/* IntentIgnore */`) above any member or route keeps its content as written.** Only its layout changes, since the whole file is normalised (below); the README says the same. Kept from the weaver era so files and habits carry over; it is now this module's own rule, implemented in the merger, not Intent's JSON code management. An ignored key stops following the model.
- **Output is normalised:** 2-space indentation, one value per line, trailing commas dropped, comments kept where they were (a same-line comment stays on its line). Inline `{ }`/`[ ]` written by hand are re-indented. Accepted: a stable shape is what makes a second run a no-op.
- **Invalid JSON stops the run** with an `ElementException` on the routes' package naming the 1-based line, and the file is left untouched. Overwriting it would lose the hand edits the merge exists to keep. The parser reports the line of the offending token, which for a missing comma is the line after it.
- Verified 2026-11-24 in OcelotTest, with `Intent.Code.Weaving.Json` uninstalled (evidence `2026-11-24-ocelot-json-self-merge`, M0–M6, s19, s20).

## `Key` per route (1.2.0)

Each route gets `"Key": <route element name>`, so aggregates (`RouteKeys`) can reference it. The designer derives route names from the target endpoint, so two routes to the same endpoint share a name; the template throws an `ElementException` on a duplicate rather than emit duplicate keys.

## Non-HTTP route targets (1.2.0)

A route whose target has no `Http Settings` has no verb or path to forward to. Intent.Metadata.ApiGateway flags it in the designer, but a model can be saved with the error, so the template throws an `ElementException` on the route. Before this the writer hit a null `DownstreamHttpMethod` and the whole file failed with a `NullReferenceException` naming no element (evidence `2026-11-24-g2-repro`).

## Secured by default (1.2.0)

Decided with the client (plan `2026-11-18-ocelot-gateway-pattern-proof`, D1–D4): the gateway is the only way in, so every route is secured unless the model marks it `Unsecured`.

- **Output:** one `GlobalConfiguration.AuthenticationOptions.AuthenticationProviderKeys: ["Bearer"]` when any route is secured, and `"AuthenticationOptions": { "AllowAnonymous": true }` on each unsecured route. This fails closed: a hand-written route, or one missing settings, is secured. Proven on Ocelot 25.0.2.
- **Ignores the application's Default API Security setting** on purpose: it defaults to unsecured and would leave a new gateway open.
- **Written whether or not Intent.Security.JWT is installed**, and the scheme name is fixed at `Bearer`. Clients register bearer through their own module, which Intent cannot see. A gateway with no `Bearer` scheme refuses to start (verified 2026-11-24); that is accepted, since failing loudly beats running open. Add settings only when someone asks.
- **Resolution walks route → parent elements → package**, checking `Secured` before `Unsecured` at each level, so a conflict fails closed. Stereotypes are matched by id (Intent.Metadata.Security's `Secured` / `Unsecured`), so there is no assembly dependency on that module. Without it installed nothing can be opened, which is the safe direction. Intent.Metadata.ApiGateway's Manage Gateway Routes form applies the same rule in TypeScript (`gatewayRouteIsUnsecured`) to tag each route [SECURED] or [UNSECURED], so a change to this rule means changing that one too.
- **Route-level `Secured` / `Unsecured` come from Intent.Metadata.Security 1.1.7**, which targets Api Gateway Route through a package reference **by path** to the ApiGateway modeler, the way Intent.Metadata.WebApi targets it for Http Settings. A module reference was rejected: it pulled in Intent.Modelers.Services 4.0.0. The path reference adds no `.imodspec` dependency; where ApiGateway is not installed the extra target is simply unused. Security's generated `ApiGatewayRouteModelStereotypeExtensions.cs` must stay unapplied or ignored (as in WebApi): Security has no NuGet reference to the ApiGateway API, and this module matches by stereotype id anyway. Hence the `.imodspec` floor of Security 1.1.7 (runtime-verified 2026-11-24, evidence `2026-11-24-g3-route-unsecured`).
- **`GlobalConfiguration` is always written, even empty.** Under the JSON weaver an object the template stopped writing was deleted with every key added to it by hand: flipping a gateway to `Unsecured` deleted a hand-added `BaseUrl` (evidence `2026-11-24-g3-g5-runtime`). The self-merge keeps it for the same reason: it gives hand keys such as `BaseUrl` a home, and only its generated contents vary.

## Rate Limit stereotype (1.2.0)

- **A marker, with the numbers in configuration** (the user's call, 2026-11-24): the stereotype has no properties. Limits are tuned per environment and by operators, so they belong in a file, not the model. The first cut (Limit, Period, Wait, Client Whitelist properties, written to the route's own `RateLimitOptions`) was replaced before release.
- **Output is one shared rule, `GlobalConfiguration.RateLimitOptions`, listing the marked routes' `Key`s in `RouteKeys`.** Ocelot 25 has exactly one global rule, with no named rules to point at, so `RouteKeys` is the only way for routes to "point at" a shared section. The route itself gets nothing. Proven on 25.0.2 (evidence `2026-11-24-rate-limit-global`, s6–s15).
- **A bare route marker is not possible:** a route `RateLimitOptions` with only `EnableRateLimiting: true` stops Ocelot from starting ("RateLimitOptions.Period is empty"), even with a global rule (s7, s8).
- **Never written with empty `RouteKeys`, and never disabled.** A rule with no keys applies to every route (s11). A disabled rule with no keys switches off rate limits written by hand on routes (s12). So the rule is written only while at least one route is marked.
- **The values live in `ocelot.json`, seeded once** (the user's call, 2026-11-24, with the self-merge). The model owns `EnableRateLimiting` and `RouteKeys`; `Limit` 100, `Period` "1m", `ClientIdHeader` "Oc-Client" and `ClientWhitelist` [] are written only when missing, so hand values are kept. `Wait` is not seeded because Ocelot's default (the rest of the period) is sensible.
- **Rejected along the way:** (1) the values as `// IntentIgnore` keys under the weaver, because the weaver dropped them with the rule when the last route was unmarked; (2) a separate `ocelot.rate-limiting.json`, written `OnceOff` and loaded by a conditional second `AddJsonFile` (spikes L1–L4, evidence `2026-11-24-rate-limit-layered` and `2026-11-24-rate-limit-separate-file`). It worked, but the user preferred one file once the template could merge it, so the template and the `AddJsonFile` line were removed.
- **Unmarking the last route removes the whole rule, values included.** Accepted trade-off, documented in the README. Every way of keeping them is worse: a rule with no `RouteKeys` limits every route (s11), a disabled one switches off hand route limits (s12), and a placeholder key is a hack. Re-marking seeds the starting values again.
- **No migration for 1.2.0-pre.0 users of the separate file:** the second `AddJsonFile` goes on regeneration and the file is no longer read, so its values must be moved into `ocelot.json` by hand (done for the demo gateway, s20). Only test applications ever had it.
- **A hand-written route `RateLimitOptions` overrides the shared rule** for that route (s9), and a keyed shared rule leaves unkeyed routes alone (s14). That is the escape hatch for a route needing its own limit.
- **Lives in this module, not Intent.Metadata.ApiGateway:** the rule shape and the client header are Ocelot concepts, and a future YARP module should not inherit them.
- **Route only, no package default:** a package default would limit every route by accident.
- **Ocelot 25 answers 503 when a limited route gets no client header** (`ClientIdHeader`, configurable in the shared rule, s13). Accepted and documented.

## Dependency floors on the metadata modules (1.2.0)

- **`Intent.Metadata.ApiGateway` 1.0.0-pre.0 is raised by hand in the `.imodspec`; the install in this module's application stays at 1.0.0-beta.1.** The floor exists so that installing Ocelot also brings the Manage Gateway Routes form that tags routes [SECURED] / [UNSECURED] / [RATE LIMITED]. Without it, an application stayed on beta.6, because `beta.6` sorts below `pre.0`. This module's code needs nothing new from that version.
- **Updating the metadata-only install was tried and backed out (2026-11-25).** Updating it also raises the `.csproj` `PackageReference`s. `Intent.Modules.Metadata.ApiGateway` 1.0.0-pre.0 is not on NuGet (NU1102). ApiGateway's own Modelers.Services 4.0.0 needs a newer `Intent.Modules.Common` (NU1605). So the install went back to beta.1, the references went back to their published versions, and the floor was hand-raised, as beta.6 had been before.
- **Once ApiGateway 1.0.0 is on NuGet,** the install can follow it, and the hand-raised entry then becomes computed.
- **The `Intent.Metadata.Security` install was updated to 1.1.7-pre.0**, matching its floor (see *Secured by default*). This was safe because Security adds no `PackageReference`.
- **Publish order:** ApiGateway 1.0.0 and Security 1.1.7 (the Intent.Modules repo) go before this module, or it cannot install from the feed.

## `Priority` on bare paths (1.2.0)

Ocelot matches `/x/{id}` for `GET /x` when it is listed first, and `/x` then loses its own options (rate limit, anonymous). The template writes `"Priority": 2` on `/x` whenever a route with the same verb (or no verb) has `/x/{…}`. Proven on 25.0.2.

## `Jsonc`, `OcelotJsonMerger`, `OcelotJsonWriter` (the merge's parts)

- **`Jsonc.cs`** is a small comment-preserving JSON model: `JsoncParser` reads with `Utf8JsonReader` (comments allowed, trailing commas allowed, BOM stripped), keeps each scalar's raw text so numbers and escapes round-trip, and attaches comments to the member that follows (or to the previous one when on the same line). `JsoncWriter` writes it back in the normalised shape. `System.Text.Json` has no comment-preserving DOM, which is why this exists.
- **`OcelotJsonMerger`** holds every ownership rule, so the rules are in one place and the template only builds the generated tree.
- **`OcelotJsonWriter`** is a `DataFileWriter` that accepts only the root object, converts the Data File tree to `Jsonc`, merges and writes. It replaced `CommentingJsonWriter`, which existed only because Common's `DataFileJsonWriter` (checked up to 3.11.9) ignores `IDataFileValue.Comment` and double-indents nested values; with the template writing its own output those gaps no longer matter here.

## Unit tests (`Intent.Modules.ApiGateway.Ocelot.Tests`)

- **The merge's parts are unit-tested** (2026-11-26, after the stack-merge review found about 700 lines of merger code with no tests): `JsoncTests`, `OcelotJsonMergerTests`, `OcelotJsonWriterTests` and `OcelotJsonTemplateExistingFileTests`. They reach the internal parser, merger and writer through `AssemblyInternalsVisibleTo.cs` in this project, which the test project needs to compile. The test project is in `Modules/Intent.Modules.NET.sln`.
- **Three behaviours the review rated plausible bugs are pinned, not fixed** (developer decision, 2026-11-26). Their tests are named `PlausibleBug_…` and assert what the code does today:
  1. A root that is not an object (`[]`, a number, a string) is replaced by the generated content without a warning, and its comments go with it.
  2. Invalid JSON with no modelled routes throws an `InvalidOperationException`, not the friendly `ElementException`, because that exception takes its package from the first route.
  3. Output line endings follow `Environment.NewLine` (`JsoncWriter` uses `AppendLine`), so the same model writes CRLF on Windows and LF elsewhere, and input line endings are normalised to it.
  
  To fix one, change the behaviour and update its test in the same change. A failing `PlausibleBug_` test means the behaviour moved, not that the test is wrong.
