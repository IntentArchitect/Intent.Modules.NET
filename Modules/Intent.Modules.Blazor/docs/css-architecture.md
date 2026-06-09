# Blazor CSS Architecture — Direction

## The reframe

The problem isn't that modules **share** CSS, it's *what* they share and *how* it gets
linked. Sharing a **token contract** is exactly what you want (it's how everything stays
visually consistent). What has caused the churn — the `.btn`/`.table` dedup, the
`AccountForms.css` link anchored to `MudBlazor.min.css`, the per-flavor `app.css`
divergence, the `ux-controls` merge/un-merge — is three different things masquerading as
one "shared CSS" problem:

1. multiple modules **redefining the same implementation classes** in a hand-edited shared
   file (no ownership rule),
2. **fragile linking** (a hardcoded `<link>` list in `AppRazor` + factory extensions that
   scavenge the `<head>` by `href` string), and
3. **no per-module contribution seam**, so everything piles into `ux-tokens.css` / `app.css`.

A codegen system should treat CSS the way this codebase already treats component rendering
(`IRazorComponentBuilder` registry) and AI tasks (`IAITaskProvider`) — as a **composed,
registered, ordered** concern, not shared static files everyone hand-edits.

## Target architecture — layers with single owners

**1. Tokens = the only intentional shared dependency (a contract, not implementation).**
`ux-tokens.css` should be *only* design tokens (colors, spacing, type, radius, light/dark).
Custom properties are additive and collision-free, so every module can rely on them with zero
contention. This is the one thing it's *good* for all modules to share. Strip everything else
out of it.

**2. Generic component classes = one owner, consumed not redefined.**
`.btn`/`.form-control`/`.table`/`.alert` (the plain-HTML/Identity vocabulary) belong to
exactly **one** module's sheet — the base Blazor "components" sheet (what `ux-controls.css`
was trying to be). Sample pages and auth pages *use* those classes; they never re-declare
them. That single-ownership rule is what makes the dedup a one-time thing instead of a
recurring chore.

**3. Library bridges = the library module owns them.**
`ux-mudblazor.css` stays in the MudBlazor module; a Bootstrap/Blazorise module would own its
own. They map tokens → that library's variables. No base involvement.

**4. Page/component specifics = scoped `.razor.css` (the codegen-native answer).**
This is the big one for "modules not relying on the same CSS." Blazor CSS isolation is
*inherently* per-component and collision-free (scoped `[b-xxx]` attributes), and it's
auto-bundled into `{Project}.styles.css` which is already linked. The codegen should **emit a
`.razor.css` next to each generated component** for that component's own styling. Anything
page-specific lives with the page, owned by the module that generates the page — never in a
shared file.

## The missing piece: a stylesheet registration/ordering seam

Replace the hardcoded `<link>` list in `AppRazorTemplate` + the
`SelectHtmlElements(... href == "...")` scavenging with a real API — e.g. modules call
something like `application.RegisterStylesheet(path, order)` (or an `IStylesheetContributor`),
and **one** factory extension emits all `<link>`s in deterministic order. Then:

- a module adds/removes its own sheet without editing `AppRazor` or another module's file;
- ordering is explicit (tokens → base components → library bridge → app/scoped), not "insert
  above whatever `href` I happened to find";
- the `AccountForms.css`-anchored-to-MudBlazor class of bug disappears entirely.

This is the same registry pattern the codebase already uses well elsewhere — just applied to
CSS assets.

## Two ways to "compose," pick per case

- **(A) Link-per-module (preferred default):** each module ships a separate sheet, the seam
  links them ordered. Maximum isolation; no multi-writer file.
- **(B) Contribute-to-a-generated sheet (the Intent-idiomatic option):** treat a stylesheet
  like `Program.cs` / DI — a generated artifact with named regions that modules contribute
  blocks to via decorators/factory extensions. Still one file, but contributions are
  *structured and ordered* rather than ad-hoc hand-edits. Useful when you genuinely want one
  combined file, but it reintroduces multi-writer coupling, so reserve it for the token/base
  layer only.

## Why this ends the recurring pain

- **Dedup stops happening** because each class has exactly one owning module/sheet — there's
  no second place to drift.
- **Per-flavor divergence** (`SamplePages` vs `NoSamplePages` `app.css`) goes away: the
  generic component sheet is shipped/linked unconditionally via the seam; only genuinely
  sample-specific layout is sample-gated.
- **Linking fragility** goes away with the registration API.
- **"Modules relying on the same CSS"** reduces to "modules relying on the same *tokens*" —
  which is the desirable, collision-free kind of sharing.

## What not to do

Total isolation (every module duplicating its own `.btn`) — that just trades collisions for
inconsistency and bloat. The goal is **shared contract (tokens) + single-owner shared
implementation + per-component scoped CSS + a real linking seam**, not "no sharing."

## Pragmatic sequence (no big-bang)

1. Add the stylesheet-registration seam (highest leverage; retires the `<head>` scavenging).
2. Split `ux-tokens.css` into **tokens** (contract) + **base components** (single-owner
   sheet), both registered via the seam.
3. Keep moving page-specific styling into scoped `.razor.css` as components are touched.
4. Library bridges + sample-layout stay in their owning modules, registered, ordered.

(Step 1 is a base-module change and the lever that makes the rest clean.)
