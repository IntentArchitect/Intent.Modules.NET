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
                    | `--border-focus` | `var(--primary)` | Focus ring border |
                    | `--hover-overlay` | `rgba(14,165,233,0.07)` | Row / item hover fill |
                    | `--hover-overlay-strong` | `rgba(14,165,233,0.14)` | Pressed / selected fill |
                    | `--primary` | `var(--brand-sky)` | Interactive accent |
                    | `--primary-dark` | `var(--brand-sky-dim)` | Hover state of primary |
                    | `--primary-subtle` | `rgba(14,165,233,0.10)` | Hover fills, focus rings |
                    | `--primary-glow` | `rgba(14,165,233,0.35)` | Glow shadows on buttons |
                    | `--primary-contrast` | `#FFFFFF` | Text on primary backgrounds |
                    | `--focus` | `rgba(14,165,233,0.30)` | Focus box-shadow colour |
                    | `--secondary` | `#19223A` | Secondary surface |
                    | `--secondary-dark` | `#0F1728` | Secondary hover |

                    ### Light Mode (`[data-theme="light"]`)

                    | Token | Value |
                    |---|---|
                    | `--bg` | `#E8EAED` |
                    | `--surface` | `#F4F5F7` |
                    | `--surface-2` | `#ECEEF1` |
                    | `--surface-3` | `#E2E5E9` |
                    | `--text` | `#07111E` |
                    | `--text-muted` | `#3D5470` |
                    | `--text-secondary` | `#7A93B0` |
                    | `--border` | `rgba(2,132,199,0.13)` |
                    | `--border-light` | `rgba(2,132,199,0.07)` |
                    | `--hover-overlay` | `rgba(2,132,199,0.05)` |
                    | `--hover-overlay-strong` | `rgba(2,132,199,0.10)` |
                    | `--primary` | `var(--brand-sky-dim)` |
                    | `--primary-dark` | `var(--brand-sky-deep)` |
                    | `--primary-subtle` | `rgba(2,132,199,0.09)` |
                    | `--primary-glow` | `rgba(2,132,199,0.25)` |
                    | `--focus` | `rgba(2,132,199,0.22)` |
                    | `--gradient-brand` | `linear-gradient(135deg, var(--brand-sky-dim) 0%, var(--brand-cobalt-dim) 100%)` |
                    | `--gradient-brand-soft` | `linear-gradient(135deg, rgba(2,132,199,0.10) 0%, rgba(79,70,229,0.08) 100%)` |
                    | `--secondary` | `#D1E9FA` |
                    | `--secondary-dark` | `#BAD9F5` |
                    | `--glow-primary` | `0 0 20px rgba(2,132,199,0.28), 0 0 6px rgba(2,132,199,0.16)` |
                    | `--glow-cyan` | `0 0 20px rgba(79,70,229,0.22), 0 0 6px rgba(79,70,229,0.12)` |

                    In dark mode the root element carries a fixed ambient radial gradient (sky top-left, cobalt bottom-right, `background-attachment: fixed`). Light mode removes this gradient and shows a subtle dot-grid on the body, using cyan lines at `rgba(2,132,199,0.06)` and `opacity: 0.55`.

                    ### Status Colours

                    | Token | Dark | Light |
                    |---|---|---|
                    | `--success` | `#3DD68C` | `#16A34A` |
                    | `--warning` | `#F5C542` | `#CA8A04` |
                    | `--danger` | `#F06080` | `#DC2626` |
                    | `--info` | `var(--brand-sky)` | `#0369A1` |

                    Each status colour has a matching `*-subtle` variant used for badge/notification backgrounds:

                    | Token | Dark | Light |
                    |---|---|---|
                    | `--success-subtle` | `rgba(61,214,140,0.14)` | `rgba(22,163,74,0.10)` |
                    | `--warning-subtle` | `rgba(245,197,66,0.14)` | `rgba(202,138,4,0.10)` |
                    | `--danger-subtle` | `rgba(240,96,128,0.14)` | `rgba(220,38,38,0.10)` |
                    | `--info-subtle` | `rgba(14,165,233,0.12)` | `rgba(3,105,161,0.10)` |

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

                    Links: `color: var(--primary)`, `text-decoration: none`; hover uses `var(--primary-dark)`.

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
                    | `--radius-sm` | `0.5rem` | Buttons, inputs, nav links, small elements |
                    | `--radius-md` | `0.75rem` | Form controls, popovers, notifications |
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

                    Named keyframes in `ux-base.css`: `fadeInUp` (opacity + translateY + blur) · `fadeIn`.
                    Additional keyframes used by page-specific CSS: `orb-float` · `shimmer` · `pulse-dot`.
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

                    ## Standard HTML Components

                    These CSS classes are defined in `ux-components.css` and apply to raw HTML elements (no MudBlazor dependency).

                    ### Form Controls

                    `.form-control` and `.form-select`:
                    - Font/colour/background: `var(--font-primary)` · `var(--text)` · `var(--surface)`
                    - Border: `var(--border-width) solid var(--border)`, `border-radius: var(--radius-md)`
                    - Focus: `border-color: var(--border-focus)`, `box-shadow: 0 0 0 3px var(--primary-subtle)`
                    - Disabled: `color: var(--text-muted)`, `background: var(--surface-2)`
                    - Placeholder: `color: var(--text-secondary)`

                    `.form-label`: `font-size: var(--type-body-sm)`, `color: var(--text-muted)`, `margin-bottom: var(--space-1)`.

                    `.form-floating`: label transitions to `var(--type-label-md)` size and `var(--primary)` colour on focus or when a value is present.

                    `.form-check` / `.checkbox`: flex row with `gap: var(--space-2)`, `accent-color: var(--primary)`.

                    `.input-group` / `.input-group-text`: stretch layout; addon uses `var(--surface-2)` background, `var(--border)`, `var(--radius-md)`.

                    ### Buttons

                    `.btn`: `border-radius: var(--radius-sm)`, `padding: 0.55rem 1.25rem`, `font-size: var(--type-body-sm)`, `font-weight: 600`, `line-height: 1.4`. Transitions `box-shadow` + `transform` at `--dur-fast`. Active state forces `translateY(0)` and `--shadow-1`.

                    | Variant | Class | Rest | Hover |
                    |---|---|---|---|
                    | Primary | `.btn-primary` | `--gradient-brand` fill, `--shadow-1` | `--glow-primary` + `--shadow-2`, `translateY(-1px)` |
                    | Outline | `.btn-outline` | Transparent, `--text-muted`, `1px solid --border` | `--hover-overlay` fill, `--text`, `color-mix(primary 50%)` border |
                    | Danger | `.btn-danger` | `--danger` fill, `--shadow-1` | `--shadow-2`, `translateY(-1px)` |
                    | Link | `.btn-link` | Transparent, `--primary`, no padding-inline | `--primary-dark`, underline |
                    | Icon | `.btn-icon` | `2rem × 2rem`, `--radius-sm`, `--text-muted` | `--hover-overlay` fill, `--text` |
                    | Large | `.btn-lg` | Adds `padding: var(--space-3) var(--space-5)`, `font-size: var(--type-body-lg)` | — |

                    ### Alerts

                    `.alert`: `var(--surface-2)` background, `var(--border)`, `var(--radius-md)`, `padding: var(--space-3) var(--space-4)`.

                    | Class | Text | Background | Border |
                    |---|---|---|---|
                    | `.alert-danger` | `--danger` | `--danger-subtle` | `color-mix(--danger 30%, transparent)` |
                    | `.alert-success` | `--success` | `--success-subtle` | `color-mix(--success 30%, transparent)` |
                    | `.alert-warning` | `--warning` | `--warning-subtle` | `color-mix(--warning 30%, transparent)` |

                    ### Tables

                    `.table`: full width, `border-collapse: collapse`, `font-size: var(--type-body-sm)`.
                    - `thead th`: `var(--surface-2)` background, `var(--text-muted)`, `var(--type-label-md)`, 600 weight, uppercase, `0.06em` tracking, `--border` bottom divider.
                    - `tbody td`: `var(--text)`, `--border-light` bottom divider.
                    - Row hover: `background: var(--hover-overlay)`.

                    ---

                    ## Component Patterns

                    The following describe design intent for MudBlazor component overrides (implemented in `ux-mudblazor.css`).

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

                    ### Buttons (MudBlazor)
                    All MudBlazor buttons: `--radius-md`, `height: 36px`, `font-weight: 500`, `letter-spacing: 0.04em`, `text-transform: uppercase`, `--dur-fast` transition. Hover adds `translateY(-1px)`.

                    | Variant | Rest | Hover |
                    |---|---|---|
                    | Filled primary | `--gradient-brand` fill, no border, `--glow-primary` shadow | `brightness(1.08)`, stronger glow |
                    | Outlined/text primary | Transparent, `--primary` border + text | `--primary-subtle` fill, glow |
                    | Secondary / default | Same as outlined primary | Same as outlined primary |
                    | Error | `--danger` border + text | `--danger-subtle` fill, danger glow |
                    | Disabled | `opacity: 0.38`, `pointer-events: none` | — |

                    Icon buttons use `--radius-sm`, dimmed colour at rest (62 % opacity), full colour + subtle fill on hover.

                    ### Tables (MudBlazor)
                    - Container: `--surface`, `--border`, `--radius-lg`, `--shadow-2`, `overflow: hidden`.
                    - Header: `--surface-2` → tinted gradient; 12 px, 600 weight, 0.06 em tracking, uppercase, `--text-secondary`.
                    - Row height: 44 px; `--border-light` bottom divider; hover: `--primary-subtle` fill + `inset 3px 0 0 var(--primary)` left accent.
                    - Pagination: `--surface-2` background, `--border` top.

                    ### Form Controls (MudBlazor)
                    - Height: `min-height: 44px`; background `--surface-2`; border `--border`; `--radius-sm`.
                    - Focus: border becomes `--primary`; box-shadow `0 0 0 3px var(--focus), 0 0 12px var(--primary-glow)`.
                    - Focused label: `--primary`, 0.75 rem, 500 weight.
                    - Numeric spin buttons: minimal 24 px column, no borders, dimmed primary colour, subtle fill on hover.
                    - Autofill (Chrome/Edge): overrides browser autofill styling so the field keeps its themed surface background and text colour instead of the native highlight.

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

                    `--radius-md`; 1 px border; each severity uses `color-mix(status 14%, transparent)` tinted background with the status colour for text, and `color-mix(status 30%, transparent)` border.

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
                    | `.text-danger/success/info` | Status text colour |
                    | `.text-white` | `color: var(--primary-contrast)` |
                    | `.w-100` / `.h-100` | 100% width / height |
                    | `.mb-3` | `margin-bottom: var(--space-3)` |
                    | `.font-weight-bold` | `font-weight: 700` |
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var config = MarkdownFile.GetConfig();
            return new TemplateFileConfig(config.FileName, config.FileExtension, config.LocationInProject,
                OverwriteBehaviour.OnceOff, config.CodeGenType);
        }

    }
}
