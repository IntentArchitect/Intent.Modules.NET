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
description: Updates ux-tokens.css and ux-controls.css to match a new or replaced design.md. Use when a design specification has changed and a standard Blazor app needs its colours, typography, spacing, radii, base styles, or native control styles refreshed without MudBlazor.
paths:
  - "**/ux-tokens.css"
  - "**/ux-controls.css"
  - "**/design.md"
---

## Purpose

Translate a design specification (`design.md`) into the standard Blazor CSS theme (`ux-tokens.css` and `ux-controls.css`). The design may use any structure or naming convention. Work from design intent and express it through the existing token and control styling system without introducing a component library dependency or changing stylesheet wiring.

This skill is for standard Blazor apps. It may style Bootstrap-compatible selectors such as `.btn`, `.form-control`, and `.alert` because generated standard Blazor pages use those classes and sample-page apps may load Bootstrap before the UX theme files. Do not use MudBlazor components, MudBlazor CSS selectors, or `--mud-*` CSS variables.

If `ux-mudblazor.css` exists in the target app, stop and use the `mudblazor-ux-theme-sync` skill instead.

---

## MANDATORY: Read All Three Files Before Changing Anything

STOP - you MUST read all three files in full before writing a single line:

1. `design.md` - the source of truth for the new design intent
2. `ux-tokens.css` - design tokens, base styles, utility classes, aliases, and theme polarity
3. `ux-controls.css` - standard Blazor/native control styles

If any file is missing or unreadable: stop, report which file is missing, and do not proceed.

---

## Step 1 - Extract Design Intent from design.md

The design may be a reference table, a prose brief, a Figma export, a brand guide, or another format. Extract the intent before touching CSS.

Extract the following where present:

**Colours**
- Primary or brand colours and hover/pressed variants
- Secondary or accent colours
- Background and surface colours
- Text colours
- Border colours
- Status colours: success, warning, error/danger, info
- Any gradient definitions

**Typography**
- Font family
- Type scale
- Font weights
- Line heights

**Spacing**
- Spacing scale tokens if defined

**Shape**
- Border-radius values at each tier

**Motion**
- Duration and easing values if defined

**Control-level rules**
- Buttons, links, cards or panels, inputs, selects, checkboxes, alerts, tables, badges, navigation, page headers, focus states, loading states, or utility classes

If the design omits a category entirely, treat the existing values in both CSS files as correct and leave them unchanged.

---

## Step 2 - Map Design Intent to CSS Files

The theme is split across two files. Update only the sections affected by the design change.

| Design concept | File | Typical section |
|---|---|---|
| Brand palette / raw colour values | `ux-tokens.css` | Brand Palette |
| Typography scale and fonts | `ux-tokens.css` | Typography Scale |
| Spacing, radius, motion | `ux-tokens.css` | Spacing, Radius & Motion |
| Semantic colours for the default theme | `ux-tokens.css` | Semantic Tokens |
| Semantic colours for the alternate theme | `ux-tokens.css` | `[data-theme="..."]` override |
| Backward-compatible aliases | `ux-tokens.css` | Backward-compat Aliases |
| Base HTML styles | `ux-tokens.css` | Global Base Styles |
| Utility classes and animations | `ux-tokens.css` | Utility Classes / Motion |
| Forms and inputs | `ux-controls.css` | `.form-control`, `.form-select`, `.form-floating`, `.form-check` |
| Buttons and links | `ux-controls.css` | `.btn`, `.btn-primary`, `.btn-danger`, `.btn-link` |
| Alerts and status messages | `ux-controls.css` | `.alert`, `.alert-*`, `.text-*` |
| Tables | `ux-controls.css` | `.table` |
| Layout helper classes | `ux-controls.css` or scoped CSS | Only when already present |

If a file does not use numbered sections, map by token name, selector, and existing naming conventions.

---

## Step 3 - Determine Change Scope

Classify every extracted design value as one of:

- **Token update** - a raw token value changes. Update the custom property value in place.
- **New token** - the design introduces a concept that has no existing token. Add it in the appropriate section following existing naming conventions.
- **Control rule update** - a selector/property in `ux-controls.css` needs a different value. Update the property in place.
- **No change** - the design is silent on this area. Leave the existing CSS untouched.

Do not restructure sections, reorder rules, or clean up unrelated CSS.

---

## Step 4 - Detect Theme Polarity and Handle Dual Theme

### 4a - Detect Polarity

Before updating tokens, determine the default theme polarity of the existing CSS and the incoming design.

**Existing CSS polarity** - inspect the `:root` block in `ux-tokens.css`:
- If `:root` has dark surface values, the existing default is dark. Its override selector is usually `:root[data-theme="light"]`.
- If `:root` has light surface values, the existing default is light. Its override selector is usually `:root[data-theme="dark"]`.

**Incoming design polarity** - read `design.md`:
- If the design states a light default, the new default is light.
- If the design states a dark default, the new default is dark.
- If the design is silent, preserve the existing polarity.

### 4b - Polarity Flip

If the polarities differ, a polarity flip is required:

1. Apply the new design's default palette to `:root`.
2. Rename the existing override selector globally in both CSS files:
   - Flipping dark to light: rename every `[data-theme="light"]` selector to `[data-theme="dark"]`
   - Flipping light to dark: rename every `[data-theme="dark"]` selector to `[data-theme="light"]`
3. Update the renamed override block with opposite-polarity token values.
4. Check all control rules in `ux-controls.css` that use `[data-theme="..."]` selectors.

The rename must be global across both files.

### 4c - No Polarity Flip

If the polarities match, update only the affected token values in each block. Do not rename selectors.

If the design specifies both dark and light variants, update both blocks independently.

---

## Step 5 - Preserve Backward-Compatible Aliases

The `--ux-*` alias block in `ux-tokens.css` maps older names to current semantic tokens via `var(...)`.

After updating semantic tokens:

- Verify aliases still resolve.
- Only edit aliases if a referenced token was renamed or removed.
- Do not remove aliases.
- Prefer semantic tokens such as `--primary`, `--surface`, `--text`, and `--border` in new or updated rules.

---

## Step 6 - Update Standard Control Styles

Use `ux-controls.css` for app-wide native/control styling. These selectors support standard Blazor account pages and generated standard Blazor pages.

Standard Blazor apps may or may not load `bootstrap/bootstrap.min.css` depending on generated settings and content. When Bootstrap is already linked, `ux-controls.css` is loaded after it and acts as the app's design-system override. When Bootstrap is not linked, `ux-controls.css` must still provide enough styling for these control classes to render correctly.

Supported control families include:

- `.form-control`
- `.form-select`
- `.form-floating`
- `.form-check`
- `.input-group`
- `.btn`
- `.btn-primary`
- `.btn-danger`
- `.btn-link`
- `.alert`
- `.alert-danger`
- `.alert-success`
- `.alert-warning`
- `.table`
- utility classes already present in the file

When updating controls:

- Keep selectors compatible with plain HTML and Blazor built-in components such as `InputText`, `InputSelect`, `InputNumber`, `InputDate`, and `EditForm`.
- Use token references from `ux-tokens.css`.
- Preserve focus-visible and validation readability.
- Keep form controls usable in static SSR and form-post account pages.
- Do not rely on Bootstrap being loaded. Treat these as standard Blazor compatibility classes and make them work with or without Bootstrap present.
- If Bootstrap is already present, preserve compatibility with its class names and override its visual values through `ux-controls.css`.

Forbidden:

- Adding MudBlazor selectors such as `.mud-*`
- Adding `--mud-*` variables
- Adding, removing, or editing Bootstrap assets or stylesheet links
- Replacing standard controls with MudBlazor components
- Introducing JavaScript-only or interactive-only styling assumptions

---

## Step 7 - Apply Changes

Apply the minimum edits needed to express the new design intent.

Use in-place value replacement wherever possible.

Forbidden:

- Deleting CSS rules or properties that the design does not address
- Reordering declarations or sections as cleanup
- Reformatting unrelated rules
- Changing selectors except for required `[data-theme="..."]` polarity flips
- Introducing new component-library dependencies
- Recreating or vendoring Bootstrap in `ux-controls.css`

---

## Handling Ambiguity

**Incomplete design spec** - if the design defines colours but no typography, update colours only.

**Conflicting values** - if the design contains contradictory values, use the most prominent or last-defined value and note the conflict in a short comment adjacent to the token.

**Unknown design format** - if the structure is unrecognised, carefully extract colours, font names, sizing, and component intent before editing.

**Missing files** - if the target project has no `ux-tokens.css` or `ux-controls.css`, stop and report the missing file rather than creating a theme from scratch.

---

## Definition of Done

- [ ] `design.md` was read in full before any edits were made
- [ ] `ux-tokens.css` was read in full before any edits were made
- [ ] `ux-controls.css` was read in full before any edits were made
- [ ] Existing CSS polarity was identified before editing
- [ ] If polarity flipped, every `[data-theme="..."]` selector across both files was renamed globally
- [ ] If polarity flipped, the renamed override block has opposite-polarity token values
- [ ] Brand palette tokens reflect the new design where specified
- [ ] Semantic colour tokens are updated for affected theme variants
- [ ] Typography tokens are updated if the design specifies fonts or a type scale
- [ ] Spacing, radius, and motion tokens are updated if the design specifies them
- [ ] `--ux-*` aliases still resolve
- [ ] Standard control styles reference semantic tokens
- [ ] Standard control styles remain compatible with built-in Blazor inputs and plain HTML
- [ ] Standard control styles work whether Bootstrap is present or absent
- [ ] No Bootstrap assets or stylesheet links were added, removed, or edited
- [ ] No MudBlazor selectors, `--mud-*` variables, or component-library dependencies were introduced
- [ ] No unrelated CSS rules or sections were modified
- [ ] Changes are limited to what the new `design.md` specifies
""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}
