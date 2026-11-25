using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.AI.BlazorUxThemeSyncSkill
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class BlazorUxThemeSyncSkillTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.AI.BlazorUxThemeSyncSkillTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BlazorUxThemeSyncSkillTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"SKILL", "md", "blazor-ux-theme-sync")
                .FromMarkdown("""
---
name: blazor-ux-theme-sync
description: Updates ux-tokens.css to match a new or replaced design.md. Use when a design specification has changed and a standard Blazor app needs its colours, typography, spacing, radii, base styles, or native control styles refreshed without MudBlazor.
paths:
  - "**/ux-tokens.css"
  - "**/design.md"
---

# Blazor theme sync

Express a changed `design.md` in the standard Blazor app's existing CSS theme, changing only what the design addresses. Creating a theme from scratch is out of scope.

No component library. Bootstrap-compatible selectors (`.btn`, `.form-control`, `.alert`) are fine, because generated pages use them and sample-page apps may load Bootstrap first. MudBlazor components, `.mud-*` selectors and `--mud-*` variables are not. If the app has `ux-mudblazor.css`, stop and use `mudblazor-ux-theme-sync` instead.

## 1. Read first

Read in full: `design.md`, `ux-tokens.css`, and `ux-base.css` and `ux-components.css` where the app has them. If `design.md` or `ux-tokens.css` is missing, stop and say which.

## 2. Extract the design intent

`design.md` can be in any form: a token table, prose, a brand guide, a Figma export. Read it for intent, not for matching names. Pull out what it states for:

- **Colour:** brand and accent (with hover and pressed), backgrounds and surfaces, text, borders, status (success, warning, error, info), gradients.
- **Typography:** font families, type scale, weights, line heights.
- **Shape and motion:** spacing scale, radius tiers, durations and easing.
- **Controls:** any explicit rules for buttons, links, cards or panels, inputs, selects, checkboxes, alerts, tables, badges, navigation, page headers, focus or loading states, utility classes.

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
| Page background, base HTML elements | `ux-base.css` → 1 Global Reset & Base Elements |
| Focus ring, reduced motion | `ux-base.css` → 2 Accessibility & Motion |
| Keyframes | `ux-base.css` → 3 Global Animations |
| Forms and inputs | `ux-components.css` → 1 Standard Form Controls |
| Buttons and links | `ux-components.css` → 2 Buttons |
| Alerts, tables, status messages | `ux-components.css` → 3 Feedback & Data Display |
| Utility classes, badges | `ux-components.css` → 4 Utility Classes |
| Layout helper classes | wherever they already exist, scoped CSS included; never new ones |

Without numbered sections, or where a file is absent, map by token name and selector. Each value is one of: a token value to replace in place; a new token, added to the right section in the existing naming style; a control property to change in place; or no change.

## 4. Theme polarity

The default theme is whichever `:root` holds: dark surfaces mean dark-default, usually with a `:root[data-theme="light"]` override; light surfaces mean light-default, with `[data-theme="dark"]`. The design's default is what it states; if it's silent, keep the current one.

- **Same polarity:** update only the affected values in each block. If the design gives both palettes, update each block from its own palette.
- **Flip:** put the design's default palette in `:root`, then rename the override selector everywhere it appears, in every theme CSS file (`ux-base.css` has `:root[data-theme="..."]` rules too). Dark to light renames every `[data-theme="light"]` to `[data-theme="dark"]`, and the reverse. Give the renamed block the opposite-polarity values. A partial rename leaves the toggled theme without its overrides.

## 5. Keep the plumbing working

- `--ux-*` aliases map old names to semantic tokens through `var()`. They stay, and still resolve; re-point one only when its target was renamed or removed. New or updated rules use semantic tokens (`--primary`, `--surface`, `--text`, `--border`).
- Control styles (`.form-control`, `.form-select`, `.form-floating`, `.form-check`, `.input-group`, `.btn`, `.btn-primary`, `.btn-danger`, `.btn-link`, `.alert`, `.alert-danger`, `.alert-success`, `.alert-warning`, `.table`, and existing utilities) reference semantic tokens and stay compatible with plain HTML and Blazor's `EditForm`, `InputText`, `InputSelect`, `InputNumber` and `InputDate`. Keep focus-visible and validation states readable, and keep them working in static SSR and form-post account pages: no JavaScript-only or interactive-only assumptions.
- Bootstrap may or may not be linked. When it is, the theme loads after it and overrides its visual values while keeping its class names; when it isn't, the theme alone must make these classes render. Never add, remove or edit Bootstrap assets or stylesheet links, and never recreate or vendor Bootstrap in the theme.

## 6. Edit minimally

One colour is written several ways, and a value you rebase in one spelling stays on the old palette in
the others. Before calling a colour done, search for every form of it — `#0EA5E9` and `#0ea5e9`,
`rgb(14, 165, 233)` and `rgb(14,165,233)`, and the same two spacings inside `rgba(...)` — across every
stylesheet, not just the token file. Missing the spaced variant is the usual way a theme is left with
most of its borders, hovers, focus rings and tints still on the previous brand.

Never:
- delete rules or properties the design doesn't address;
- change selectors, other than the polarity rename;
- restructure or reorder sections or declarations, or reformat unrelated rules;
- replace standard controls with component-library components, or add a component-library dependency.

## Definition of Done

- [ ] `design.md` and every theme CSS file read before any edit; current and target polarity identified
- [ ] Palette, semantic, typography, spacing, radius and motion tokens reflect what the design specifies, and only that
- [ ] On a flip: every `[data-theme]` selector renamed in every theme file, and the override block holds opposite-polarity values
- [ ] `--ux-*` aliases resolve; control rules use semantic tokens
- [ ] Controls work with built-in Blazor inputs and plain HTML, with or without Bootstrap
- [ ] No Bootstrap asset or link changed; no `.mud-*`, `--mud-*` or component-library dependency added
- [ ] Unrelated CSS unchanged
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
