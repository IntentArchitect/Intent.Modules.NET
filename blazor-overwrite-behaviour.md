# Blazor OverwriteBehaviour — branch delta tracker

Tracks `StaticContentTemplateRegistration` behaviours across `main`/current-work vs `feature-blazor-design` (fbd).

`Always*` = base default (no override). `Always✓` = explicit override returning `Always`. `OnceOff` = overrides to `OnceOff`.

## Intent.Modules.Blazor

| Class | current branch | feature-blazor-design |
|-------|---------------|----------------------|
| `BlazorSkillSampleFiles` | Always* | Always* |
| `ThemeArtifacts` | Always✓ | Always✓ |
| `ThemeToggle` | Always* | **absent** |
| `SamplePages` | OnceOff | OnceOff |
| `NoSamplePages` | OnceOff | OnceOff |
| `WasmSamplePages` | OnceOff | OnceOff |
| `WasmNoSample` | OnceOff | OnceOff |

## Intent.Modules.Blazor.Components.MudBlazor

| Class | current branch | feature-blazor-design |
|-------|---------------|----------------------|
| `HomeStyle` | Always* | Always* |
| `ThemeArtifacts` | Always* | Always* |

## Delta summary

One delta vs fbd: `ThemeToggle` (base-default Always) exists on the current branch, absent from fbd.

## Mechanism verification (2026-06-15)

All 8 matching classes verified congruent between branches — not just behaviour value but also:
- `[IntentMerge]` decoration (present on the 4 OnceOff + ThemeToggle classes, absent on the plain-Always ones)
- `[IntentIgnore]` on `Blazor.ThemeArtifacts`'s explicit `Always` override
- `Register` gate conditions (ComponentLib/Server/Wasm/IncludeSample guards are identical)
- `Replacements` entries (ApplicationName + Namespace on gated ones, empty on ungated)

`ThemeToggle` is the only existence delta. Its `Register` gate skips registration when `Intent.Blazor.Components.MudBlazor` is installed.
