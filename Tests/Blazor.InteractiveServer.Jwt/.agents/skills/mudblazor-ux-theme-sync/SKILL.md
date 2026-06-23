---
name: mudblazor-ux-theme-sync
description: Updates ux-tokens.css and ux-mudblazor.css to match a new or replaced design.md. Use when a design specification (any format or structure) has changed and the MudBlazor theme needs to reflect the new colours, typography, spacing, radii, or component styles.
paths:
contentHash: B67E87D944EB138AB52F60C986CE09F6F47FFF8D629B76C13AC3D2885C852E72
---
## Purpose

Translate a design specification (design.md) into a working MudBlazor CSS theme across four files: `ux-tokens.css` (design tokens), `ux-mudblazor.css` (MudBlazor palette bridge and component overrides), `ux-base.css` (global base styles and animations), and `ux-components.css` (component primitives and utilities). The design.md may use any structure or naming convention — your job is to understand its design intent and express it faithfully in the CSS token and component override system, without breaking the parts of the theme the new spec does not address.

- --

## MANDATORY: Read All Three Files Before Changing Anything

STOP — you MUST read all five files in full before writing a single line:

1. `design.md` — the source of truth for the new design intent
2. `ux-tokens.css` — design tokens, base styles, utility classes (library-agnostic)
3. `ux-mudblazor.css` — MudBlazor palette bridge and component overrides
4. `ux-base.css` — global resets, background patterns, accessibility rules, animations
5. `ux-components.css` — form controls, buttons, feedback components, utility classes

If any of the five files is missing or unreadable: stop, report which file is missing, and do not proceed.

- --

## Step 1 — Extract Design Intent from design.md

The design.md may be structured as a reference table, a narrative description, a Figma export, a brand guide, or any other format. Work from intent, not from structural matching.

Extract the following where present:

- *Colours**
- Primary / brand colour(s) and their hover/pressed variants
- Secondary or accent colour(s)
- Background and surface layer colours (page bg, card, elevated card, input bg)
- Text colours (primary, muted/secondary, disabled)
- Border colours
- Status colours: success, warning, error/danger, info
- Any gradient definitions
- *Typography**
- Font family (body, monospace)
- Type scale: display, h1–h6 / body-lg / body-md / body-sm / label sizes
- Font weights and line-heights
- *Spacing**
- Spacing scale tokens if defined
- *Shape**
- Border-radius values at each tier (small, medium, large, xl, full/pill)
- *Motion**
- Duration and easing values if defined
- *Component-level rules**
- Any explicit guidance for buttons, cards, dialogs, tables, inputs, navigation, chips, badges, snackbars, tooltips, or other components

If the design.md omits a category entirely, treat the existing values in both CSS files as correct and leave them unchanged.

- --

## Step 2 — Map Design Intent to CSS Tokens

The theme is split across two files. Map extracted design values to the correct file and section — update only the sections affected by the design change.

| Design concept | File | Section |
|---|---|---|
| Brand palette / raw colour values | `ux-tokens.css` | Section 1: Brand Palette |
| Typography scale and fonts | `ux-tokens.css` | Section 2: Typography Scale |
| Spacing, radius, motion | `ux-tokens.css` | Section 3: Spacing, Radius & Motion |
| Semantic colours for dark mode | `ux-tokens.css` | Section 4: Semantic Tokens — Dark |
| Semantic colours for light mode | `ux-tokens.css` | Section 5: Semantic Tokens — Light Override |
| Backward-compat aliases | `ux-tokens.css` | Section 6: Backward-compat Aliases |
| MudBlazor palette bridge | `ux-mudblazor.css` | Section 1: MudBlazor Palette Bridge |
| MudBlazor typography overrides | `ux-mudblazor.css` | Section 2: MudBlazor Typography Override |
| Component-specific rules | `ux-mudblazor.css` | Section 3: MudBlazor Component Overrides |
| Page header utility | `ux-mudblazor.css` | Section 4: MudBlazor Page Header Utility |
| Utility classes or animations | `ux-tokens.css` | Sections 8–9 |
| Background/gradient patterns, page radial glow | `ux-base.css` | Section 1: Global Reset & Base Elements |
| Light-mode dot-grid overlay colour | `ux-base.css` | Section 1: Global Reset & Base Elements |
| Global keyframe animation curves | `ux-base.css` | Section 3: Global Animations |
| Standard form control styles | `ux-components.css` | Section 1: Standard Form Controls |
| Button styles (primary, outline, danger, link) | `ux-components.css` | Section 2: Buttons |
| Alert / table / feedback styles | `ux-components.css` | Section 3: Feedback & Data Display |
| Utility and badge classes | `ux-components.css` | Section 4: Utility Classes |

If a file does not use numbered sections, map by token name and component selector instead.

> **Note on ux-components.css:** This file uses only `var()` token references — most design changes (colours, typography, spacing, radii) flow through token updates in `ux-tokens.css` without requiring edits here. Edit `ux-components.css` only when the design explicitly introduces a new component variant or structural rule that cannot be expressed via tokens alone.

- --

## Step 3 — Determine Change Scope

Before editing, classify every extracted design value as one of:

- **Token update** — a raw value changes (e.g. a hex colour, a font size, a radius). Update the custom property value in place.
- **New token** — the design introduces a concept that has no existing token. Add it in the appropriate section following existing naming conventions.
- **Component rule update** — a component override (selector + property) needs a different value. Update the property in place; do not restructure the rule.
- **No change** — the design is silent on this area. Leave the existing CSS untouched.

Do not restructure sections, reorder rules, or clean up unrelated code as part of this task.

- --

## Step 4 — Detect Theme Polarity and Handle Dual-Theme

### 4a — Detect Polarity

Before updating any tokens, determine the **default theme polarity** of both the existing CSS and the incoming design.

- *Existing CSS polarity** — inspect the `:root` block in `ux-tokens.css`:
- If `:root` has dark surface values (bg near-black or deep navy), the existing default is **dark**. Its override selector is `[data-theme="light"]`.
- If `:root` has light surface values (bg near-white or pale), the existing default is **light**. Its override selector is `[data-theme="dark"]`.
- *Incoming design polarity** — read the design.md:
- If the design states a light default (e.g. "light is the default experience"), the new default is **light**.
- If the design states a dark default, the new default is **dark**.
- If the design is silent, preserve the existing polarity.

### 4b — Polarity Flip

If the polarities differ (e.g. existing is dark-default, incoming is light-default), a **polarity flip** is required:

1. Apply the new design's default palette to `:root`.
2. Rename the existing override selector **throughout both files**:
  - Flipping dark→light: rename every `[data-theme="light"]` selector to `[data-theme="dark"]`
  - Flipping light→dark: rename every `[data-theme="dark"]` selector to `[data-theme="light"]`
3. Update the renamed override block's token values to the opposite-polarity equivalents of the new design (dark surfaces for a dark override, light surfaces for a light override). If the design does not specify the opposite-polarity palette, derive it: invert surface lightness, flip text from dark-to-light or light-to-dark, adjust shadow strength accordingly.
4. Check every rule in **all four CSS files** that uses `[data-theme="..."]` as a selector prefix — including `:root[data-theme="..."]` forms in `ux-base.css` — rename those too.
- *The rename must be global across both files.** A partial rename produces broken behaviour: the CSS default shows correctly but the toggled state never triggers its overrides.

### 4c — No Polarity Flip

If the polarities match, update only the token values in each block. Do not rename selectors.

If the design specifies both dark and light variants, update both blocks independently. Colours appropriate for dark surfaces must not be applied to light surfaces and vice versa.

- --

## Step 5 — Preserve the MudBlazor Bridge

The MudBlazor palette bridge (`--mud-palette-*` properties) lives in `ux-mudblazor.css` Section 1 and references semantic tokens from `ux-tokens.css` via `var(...)`. After updating semantic tokens, verify that the bridge still resolves correctly. Only edit bridge entries if:

- A semantic token it references has been removed or renamed, or
- The design explicitly maps a new colour to a specific MudBlazor palette role

Do not change bridge entries to hardcoded hex values — always keep them pointing to semantic tokens.

- --

## Step 6 — Preserve Backward-Compatible Aliases

The `--ux-*` alias block lives in `ux-tokens.css` Section 6 and maps legacy names to current semantic tokens via `var(...)`. After updating semantic tokens, verify aliases still resolve. Only edit alias entries if a referenced token has been renamed or removed. Do not remove aliases.

- --

## Step 7 — Apply Changes

Apply the minimum set of edits needed to express the new design intent. Use in-place value replacement wherever possible.

Forbidden:

- Deleting CSS rules or properties that the new design does not address
- Changing selectors, **except** `[data-theme="..."]` attribute selectors when a polarity flip (Step 4b) requires it
- Reordering declarations within a rule
- Reformatting or re-commenting unrelated sections
- Introducing `!important` on properties that did not previously have it
- Removing `!important` from MudBlazor component overrides (MudBlazor requires it)
- --

## Handling Ambiguity

- *Incomplete design spec** — if the design.md defines colours but no typography, update colours only.
- *Conflicting values** — if the design.md contains contradictory values (e.g. two different values for primary), use the most prominent or last-defined one and note the conflict in a comment adjacent to the token.
- *Unknown design format** — if the design.md structure is unrecognised (e.g. raw Figma JSON, a screenshot description, a prose brief), extract colour values, font names, and size values by reading carefully before proceeding. Do not assume token names match.
- *Missing files** — if the target project has no `ux-tokens.css` or `ux-mudblazor.css`, stop and report which file is missing rather than creating one from scratch. Theme creation is out of scope.
- --

## Definition of Done

- [ ] design.md was read in full before any edits were made
- [ ] ux-tokens.css was read in full before any edits were made
- [ ] ux-mudblazor.css was read in full before any edits were made
- [ ] ux-base.css was read in full before any edits were made
- [ ] ux-components.css was read in full before any edits were made
- [ ] Existing CSS polarity (dark-default or light-default) was identified before editing
- [ ] If polarity flipped: every `[data-theme="..."]` selector across all four CSS files was renamed globally, not partially
- [ ] If polarity flipped: `:root[data-theme="..."]` selectors in ux-base.css were included in the global rename
- [ ] If polarity flipped: the renamed override block has opposite-polarity token values, not a duplicate of the default
- [ ] All brand palette tokens reflect the new design
- [ ] All semantic colour tokens for the affected theme variant(s) are updated
- [ ] Typography tokens are updated if the design specifies fonts or a type scale
- [ ] Spacing, radius, and motion tokens are updated if the design specifies them
- [ ] The MudBlazor palette bridge still references semantic tokens (no hardcoded hex values introduced)
- [ ] Backward-compatible `--ux-*` aliases still resolve correctly
- [ ] No unrelated CSS rules or sections were modified
- [ ] ux-base.css edits are limited to background/gradient values and animation durations — no structural or selector changes beyond polarity rename
- [ ] ux-components.css was only edited if the design explicitly introduces a new component variant or structural rule; token-only design changes were not applied here
- [ ] `!important` annotations on MudBlazor component overrides are preserved
- [ ] Both dark and light theme blocks are internally consistent
- [ ] Changes are limited to what the new design.md specifies; everything else is left unchanged
