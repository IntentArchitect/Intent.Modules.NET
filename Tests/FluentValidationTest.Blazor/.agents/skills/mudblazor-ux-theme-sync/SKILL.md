---
name: mudblazor-ux-theme-sync
description: Updates ux-tokens.css and ux-mudblazor.css to match a new or replaced design.md. Use when a design specification (any format or structure) has changed and the MudBlazor theme needs to reflect the new colours, typography, spacing, radii, or component styles.
paths:
  - "**/ux-tokens.css"
  - "**/ux-mudblazor.css"
  - "**/ux-base.css"
  - "**/ux-components.css"
  - "**/design.md"
contentHash: A1270621B94CCB2F4A5DAB5CFED1A399B87ADB1EFE6FAB69E218288D5F63899D
---
# MudBlazor theme sync

Express a changed `design.md` in the app's existing CSS theme, changing only what the design addresses. Creating a theme from scratch is out of scope.

## 1. Read first

Read all five in full: `design.md`, `ux-tokens.css`, `ux-mudblazor.css`, `ux-base.css`, `ux-components.css`. If any CSS file is missing, stop and say which.

## 2. Extract the design intent

`design.md` can be in any form: a token table, prose, a brand guide, a Figma export. Read it for intent, not for matching names. Pull out what it states for:

- **Colour:** brand and accent (with hover and pressed), page / surface / card / input backgrounds, text (primary, muted, disabled), borders, status (success, warning, error, info), gradients.
- **Typography:** font families, type scale, weights, line heights.
- **Shape and motion:** spacing scale, radius tiers, durations and easing.
- **Components:** any explicit rules for buttons, cards, dialogs, tables, inputs, navigation, chips and the like.

A category the design doesn't mention keeps its current values. If the design contradicts itself, use the most prominent (or last) value and leave a CSS comment beside the token noting the conflict.

## 3. Where each value goes

| Design concept | File → section |
|---|---|
| Raw brand palette | `ux-tokens.css` → 1 Brand Palette |
| Fonts and type scale | `ux-tokens.css` → 2 Typography Scale |
| Spacing, radius, motion | `ux-tokens.css` → 3 Spacing, Radius & Motion |
| Semantic colours, default theme | `ux-tokens.css` → 4 Semantic Tokens (`:root`) |
| Semantic colours, other theme | `ux-tokens.css` → 5 Semantic Tokens override (`[data-theme="..."]`) |
| Legacy `--ux-*` aliases | `ux-tokens.css` → 6 Backward-compat Aliases |
| Utility classes, animations | `ux-tokens.css` → 8–9 |
| `--mud-palette-*` bridge | `ux-mudblazor.css` → 1 Palette Bridge |
| MudBlazor typography | `ux-mudblazor.css` → 2 Typography Override |
| MudBlazor component rules | `ux-mudblazor.css` → 3 Component Overrides |
| Page header utility | `ux-mudblazor.css` → 4 Page Header Utility |
| Page background, gradient glow, light-mode dot grid | `ux-base.css` → 1 Global Reset & Base Elements |
| Keyframe curves | `ux-base.css` → 3 Global Animations |
| Form controls, buttons, alerts, tables, badges | `ux-components.css` → 1–4 |

Without numbered sections, map by token name and selector. `ux-components.css` uses only `var()` references, so most changes reach it through the tokens: edit it only for a new component variant or rule the tokens can't express. In `ux-base.css`, change only background, gradient and animation values.

Each value is one of: a token value to replace in place; a new token, added to the right section in the existing naming style; a component property to change in place; or no change.

## 4. Theme polarity

The default theme is whichever `:root` holds: dark surfaces mean dark-default, with a `[data-theme="light"]` override; light surfaces mean light-default, with `[data-theme="dark"]`. The design's default is what it states; if it's silent, keep the current one.

- **Same polarity:** update the values in each block. If the design gives both palettes, update each from its own palette; never put dark-surface colours in the light block or the reverse.
- **Flip:** put the design's default palette in `:root`, then rename the override selector in **all four** CSS files, `:root[data-theme="..."]` forms in `ux-base.css` included (dark→light renames every `[data-theme="light"]` to `[data-theme="dark"]`, and the reverse). Give the renamed block the opposite palette; if the design doesn't supply it, derive it by inverting surface lightness and text, and adjusting shadow strength. A partial rename leaves the toggled theme without its overrides.

## 5. Keep the plumbing working

- The `--mud-palette-*` bridge points at semantic tokens through `var()`: never hardcode a hex value there. Edit an entry only when its token was renamed or removed, or the design maps a colour to that MudBlazor role.
- `--ux-*` aliases stay; re-point one only when its target token was renamed or removed.
- Browser workarounds (`:-webkit-autofill`, vendor prefixes) stay even though the design never mentions them; re-point their tokens if renamed.

## 6. Edit minimally

One colour is written several ways, and a value you rebase in one spelling stays on the old palette in
the others. Before calling a colour done, search for every form of it — `#0EA5E9` and `#0ea5e9`,
`rgb(14, 165, 233)` and `rgb(14,165,233)`, and the same two spacings inside `rgba(...)` — across every
stylesheet, not just the token file. Missing the spaced variant is the usual way a theme is left with
most of its borders, hovers, focus rings and tints still on the previous brand.

Never:

- delete rules or properties the design doesn't address;
- change selectors, other than the polarity rename;
- reorder declarations, or reformat or re-comment unrelated sections;
- add `!important` where there was none, or remove it from MudBlazor overrides, which need it.

## Definition of Done

- [ ] All five files read before any edit; current and target polarity identified
- [ ] Palette, semantic, typography, spacing, radius and motion tokens reflect what the design specifies, and only that
- [ ] On a flip: every `[data-theme]` selector renamed in all four files, and the override block holds the opposite palette
- [ ] Dark and light blocks each internally consistent
- [ ] Bridge and aliases resolve through `var()`; no hex in the bridge
- [ ] `ux-base.css` and `ux-components.css` touched only as allowed above
- [ ] `!important` on MudBlazor overrides preserved; unrelated CSS unchanged
