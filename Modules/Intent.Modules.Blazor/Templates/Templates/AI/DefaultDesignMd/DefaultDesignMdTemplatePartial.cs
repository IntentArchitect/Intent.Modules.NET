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

namespace Intent.Modules.Blazor.Templates.Templates.AI.DefaultDesignMd
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DefaultDesignMdTemplate : MarkdownBaseTemplate<object>, IMarkdownFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.AI.DefaultDesignMdTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DefaultDesignMdTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            WithContentHashing = true;
            MarkdownFile = new MarkdownFile($"design")
                .FromMarkdown("""
# UX Design System

Design tokens and component rules for the AI-themed shell.  
All values are expressed as CSS custom properties.

---

## Brand Palette

| Token | Value | Role |
|---|---|---|
| `--brand-sky` | `#0EA5E9` | Primary accent (dark mode) |
| `--brand-sky-dim` | `#0284C7` | Primary hover / light-mode primary |
| `--brand-sky-deep` | `#0369A1` | Primary pressed / light-mode hover |
| `--brand-cobalt` | `#6366F1` | Gradient endpoint (indigo) |
| `--brand-cobalt-dim` | `#4F46E5` | Gradient endpoint hover |

**Signature gradient** — sky → cobalt at 135°:

```css
--gradient-brand:      linear-gradient(135deg, var(--brand-sky) 0%, var(--brand-cobalt) 100%);
--gradient-brand-soft: linear-gradient(135deg, rgba(14,165,233,0.14) 0%, rgba(99,102,241,0.10) 100%);
```

Legacy aliases (`--brand-blue`, `--brand-violet`, `--brand-cyan`) exist for backward compatibility and resolve to the sky/cobalt tokens.

---

## Semantic Colour Tokens

### Dark Mode (default)

| Token | Value | Usage |
|---|---|---|
| `--bg` | `#06090F` | Page background |
| `--surface` | `#0C1220` | Cards, sidebars, top bar |
| `--surface-2` | `#121A2E` | Table head, dialog title, form inputs |
| `--surface-3` | `#19223A` | Tooltips, scrollbar thumb |
| `--text` | `#E8F0FC` | Primary text |
| `--text-muted` | `#7D8EAD` | Secondary / label text |
| `--text-secondary` | `#485470` | Disabled / de-emphasised text |
| `--border` | `rgba(14,165,233,0.12)` | Default border |
| `--border-light` | `rgba(14,165,233,0.06)` | Subtle dividers |
| `--primary` | `var(--brand-sky)` | Interactive accent |
| `--primary-subtle` | `rgba(14,165,233,0.10)` | Hover fills, row highlights |
| `--primary-glow` | `rgba(14,165,233,0.35)` | Glow shadows on buttons |

### Light Mode (`[data-theme="light"]`)

| Token | Value |
|---|---|
| `--bg` | `#F0F7FF` |
| `--surface` | `#FFFFFF` |
| `--surface-2` | `#E6F3FD` |
| `--surface-3` | `#D1E9FA` |
| `--text` | `#07111E` |
| `--text-muted` | `#3D5470` |
| `--primary` | `var(--brand-sky-dim)` |

The root element carries a fixed ambient radial gradient (sky top-left, cobalt bottom-right) in dark mode. Light mode removes this gradient and shows a subtle dot-grid on the main content area only.

### Status Colours

| Token | Dark | Light |
|---|---|---|
| `--success` | `#3DD68C` | `#16A34A` |
| `--warning` | `#F5C542` | `#CA8A04` |
| `--danger` | `#F06080` | `#DC2626` |
| `--info` | `var(--brand-sky)` | `#0369A1` |

Each status colour has a matching `*-subtle` variant (≈13–14 % opacity) used for tag/badge/notification backgrounds.

---

## Typography

**Fonts:** `Geist` (sans) · `Geist Mono` (code)

| Token | Size | Line-height | Usage |
|---|---|---|---|
| `--type-display` | `2.75rem` | `1.15` | Hero headlines |
| `--type-h1` | `2.1875rem` | `1.20` | Page `<h1>` |
| `--type-h2` | `1.75rem` | `1.50` | Section headings |
| `--type-h3` | `1.4375rem` | `1.30` | Sub-headings |
| `--type-body-lg` | `1.125rem` | `1.60` | Large body / sub-section title |
| `--type-body-md` | `1rem` | `1.60` | Default body text |
| `--type-body-sm` | `0.875rem` | `1.60` | Table cells, small body |
| `--type-label-lg` | `0.875rem` | `1.40` | Nav links, button labels |
| `--type-label-md` | `0.75rem` | `1.40` | Table headers, tags, tooltips |

Headings: `font-weight: 600`, `letter-spacing: -0.02em`, `text-wrap: balance`.

---

## Spacing

| Token | Value |
|---|---|
| `--space-1` | `0.25rem` |
| `--space-2` | `0.5rem` |
| `--space-3` | `0.75rem` |
| `--space-4` | `1rem` |
| `--space-5` | `1.5rem` |
| `--space-6` | `2rem` |
| `--space-7` | `3rem` |

---

## Border Radius

| Token | Value | Usage |
|---|---|---|
| `--radius-sm` | `0.5rem` | Inputs, nav links, small elements |
| `--radius-md` | `0.75rem` | Buttons, popovers, notifications |
| `--radius-lg` | `1rem` | Cards, tables |
| `--radius-xl` | `1.375rem` | Dialogs, date pickers |
| `--radius-full` | `9999px` | Pills, tags, badges, scrollbar |

---

## Motion

| Token | Value |
|---|---|
| `--dur-fast` | `100ms` |
| `--dur-med` | `200ms` |
| `--dur-slow` | `350ms` |
| `--ease-out` | `cubic-bezier(0, 0, .2, 1)` |
| `--ease-standard` | `cubic-bezier(.2, .8, .2, 1)` |

`prefers-reduced-motion: reduce` globally removes all transitions and animations.

Named keyframes: `fadeInUp` · `fadeIn` · `orb-float` · `shimmer` · `pulse-dot`.  
Utility classes: `.ux-fade-in-up` (`--dur-slow`) · `.ux-fade-in` (`--dur-med`).

---

## Shadows

| Token | Usage |
|---|---|
| `--shadow-1` | Minimal elevation (inputs) |
| `--shadow-2` | Cards, tables, tooltips |
| `--shadow-3` | Hovered cards |
| `--shadow-4` | Popovers, date pickers, elevated dialogs |
| `--shadow-5` | Modal dialogs |
| `--glow-primary` | Button/icon CTA glow (sky) |
| `--glow-cyan` | Counter-accent glow (cobalt) |

---

## Component Patterns

### Top Bar
Glassmorphism: `rgba(12,18,32,0.82)` + `backdrop-filter: blur(20px) saturate(1.4)` with a 1 px `--border` bottom edge. Light mode uses `rgba(255,255,255,0.82)`. Icon buttons are muted (`--text-muted`) and highlight to `--primary` on hover; no borders or backgrounds at rest.

### Sidebar / Nav Drawer

`--surface` fill with `--border` right edge. Anatomy:
- **Brand strip** — 32 × 32 gradient icon (`--gradient-brand` + `--glow-primary`), gradient clip text name.
- **Section labels** — 0.65 rem, 600 weight, 0.08 em tracking, uppercase, `--text-secondary`.
- **Nav links** — `--radius-sm`, fast colour/bg transition. Active state uses `--gradient-brand-soft` fill, `--primary` text, and a 3 px left-edge pill in `--gradient-brand`.

### Cards
- Default: `--surface`, `--border`, `--radius-lg`, `--shadow-2`.
- Hover: border brightens to `rgba(14,165,233,0.30)`, `--shadow-3`, `translateY(-1px)`.
- List-page cards (`.ux-fade-in-up`): no hover lift; add a 3 px gradient top accent and ambient shadow.
- Card header: `--surface-2` → `--surface` gradient background, `--border` bottom.
- Card content padding: `--space-5` (shrinks to `--space-4` on mobile).

### Buttons
All buttons share: `--radius-md`, `height: 36px`, `font-weight: 500`, `letter-spacing: 0.04em`, `text-transform: uppercase`, `--dur-fast` transition. Hover adds `translateY(-1px)`.

| Variant | Rest | Hover |
|---|---|---|
| Filled primary | `--gradient-brand` fill, no border, `--glow-primary` shadow | `brightness(1.08)`, stronger glow |
| Outlined/text primary | Transparent, `--primary` border + text | `--primary-subtle` fill, glow |
| Secondary / default | Same as outlined primary | Same as outlined primary |
| Error | `--danger` border + text | `--danger-subtle` fill, danger glow |
| Disabled | `opacity: 0.38`, `pointer-events: none` | — |

Icon buttons use `--radius-sm`, dimmed colour at rest (62 % opacity), full colour + subtle fill on hover.

### Tables
- Container: `--surface`, `--border`, `--radius-lg`, `--shadow-2`, `overflow: hidden`.
- Header: `--surface-2` → tinted gradient; 12 px, 600 weight, 0.06 em tracking, uppercase, `--text-secondary`.
- Row height: 44 px; `--border-light` bottom divider; hover: `--primary-subtle` fill + `inset 3px 0 0 var(--primary)` left accent.
- Pagination: `--surface-2` background, `--border` top.

### Form Controls
- Height: `min-height: 44px`; background `--surface-2`; border `--border`; `--radius-sm`.
- Focus: border becomes `--primary`; box-shadow `0 0 0 3px var(--focus), 0 0 12px var(--primary-glow)`.
- Focused label: `--primary`, 0.75 rem, 500 weight.
- Numeric spin buttons: minimal 24 px column, no borders, dimmed primary colour, subtle fill on hover.

### Dialogs / Modals

- Width: 70%; `--surface`; `--radius-xl`; `--shadow-5` + `0 0 80px rgba(14,165,233,0.10)`.
- Title area: `--surface-2` fill, `--border` bottom; title text rendered as gradient clip text (`--gradient-brand`).
- Content: `--surface` fill, `--text-muted` body text.
- Actions: `--surface-2` fill, `--border` top.

### Date / Time Pickers
- Container: `--surface`, `--border`, `--radius-xl`, `--shadow-4`.
- Toolbar: `--gradient-brand` fill, white text and buttons.
- Day selected: `--gradient-brand` fill, white text.
- Today: `--primary` border.

### Notifications / Toasts

`--radius-md`; 1 px border; each severity uses a `color-mix(14%, --surface-2)` tinted background, full status colour for text, and 30 % opacity border.

### Popovers / Menus
`--surface-2` fill; `--border`; `--radius-md`; `--shadow-4`. List items: `--text-muted` at rest, `--text` + `--hover-overlay` on hover.

### Tags / Chips

Pill shape (`--radius-full`); `--surface-2` / `--border` base. Status variants use the frosted-glass pattern (13 % opacity tinted fill, 32 % border).

### Tooltips
`--surface-3`; `--border`; `--radius-sm`; 12 px text; `--shadow-2`.

---

## Page Header

A full-bleed transparent band that bleeds edge-to-edge across the main content area, compensating for sidebar and content padding via negative margins.

- Title: `clamp(1.5rem, 3vw, 1.875rem)`, 700 weight, −0.03em tracking, `--gradient-brand` clip text.
- Icon badge: `--gradient-brand` background, white icon, `--radius-md`, `0 2px 14px rgba(14,165,233,0.40)` shadow.
- Subtitle: `--text-muted`, 1 rem, `margin-top: 0.375rem`.

---

## Home Page Components

### Hero
Full-viewport hero section with three blurred ambient orbs (600 px sky, 500 px cobalt, 320 px sky) animated via `orb-float`.

- **Badge** — pill, `--primary` tinted, pulsing dot.
- **Title** — `clamp(2rem, 4.5vw, 3rem)`, 700 weight; gradient span for accent words.
- **CTA button** — 46 px height, `--gradient-brand`, `--glow-primary`; hover adds stronger glow + `translateY(-2px)`.
- **Stats strip** — inline pill, `--border`-outlined, columns with value / label pairs separated by 1 px dividers.

### Bento Grid
3 × 2 grid of 130 px tiles (collapses to 2-col at 700 px, 1-col at 420 px). Each tile:
- Shell: `--surface`, `--border`, `--radius-lg`, flex column.
- Hover: border brightens to `--tc` (per-tile colour custom property), gradient top-line + tint overlay appear, `translateY(-3px)`, glow shadow.
- Icon: 36 × 36, `--radius-sm`, 13 % tinted fill, glow on hover.

---

## Utility Classes

| Class | Effect |
|---|---|
| `.ux-gradient-text` | `--gradient-brand` clip text |
| `.ux-text-primary` | `color: --primary` |
| `.ux-bg-primary` | `background-color: --primary` |
| `.ux-shadow-sm/shadow/shadow-lg` | shadow-1/2/4 |
| `.ux-rounded/rounded-lg/rounded-xl` | radius-sm/md/lg |
| `.ux-fade-in-up` | `fadeInUp` entrance animation |
| `.ux-fade-in` | `fadeIn` entrance animation |
| `.cursor-pointer` | pointer cursor + 0.80 opacity hover |
| `.badge-success/danger/warning/info/neutral` | Frosted-glass status badge pill |
| `.nav-brand` / `.nav-brand-icon` / `.nav-brand-name` | Sidebar branding strip |
| `.nav-section-label` | Nav section group label |

---

## Accessibility

- `:focus-visible`: 2 px `--primary` outline, 2 px offset, `--radius-sm`.
- Heading elements suppress the focus ring when programmatic focus is applied during navigation.
- `prefers-reduced-motion` disables all transitions and animations globally.

---

## Backward-Compatible Aliases (`--ux-*`)

A full set of `--ux-*` tokens maps old naming conventions to the current semantic tokens. Use the semantic tokens (`--primary`, `--surface`, `--text`, etc.) in new code; `--ux-*` aliases exist only to avoid breaking existing components.

""");
        }

        [IntentManaged(Mode.Fully)]
        public override IMarkdownFile MarkdownFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig() => MarkdownFile.GetConfig();

    }
}