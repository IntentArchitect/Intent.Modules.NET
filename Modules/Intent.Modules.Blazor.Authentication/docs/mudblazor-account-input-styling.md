# MudBlazor Account Pages — Input Styling Options

## Context

The Identity Account pages render in **static SSR** (`/Account/*` → render mode `null`). Posted
form fields therefore **must** be Blazor `InputBase<TValue>` components (`InputText`,
`InputCheckbox`, `InputSelect`, `InputTextArea`) or raw `<input name="...">`, because those emit the
FormName-compatible names that `[SupplyParameterFromForm]` binds from. MudBlazor input components
(`MudTextField`, `MudCheckBox`, …) do **not** post under static SSR (see
`mudblazor-account-variant-plan.md`).

So the rule for the MudBlazor account variants is:
- **Posted fields:** `InputText` / `InputCheckbox` / `InputSelect` / `InputTextArea`.
- **Chrome only:** `MudPaper`, `MudStack`, `MudGrid`, `MudText`, `MudAlert`, `MudButton`, icons.

That leaves one open question: **how to visually style the `InputText`/`InputCheckbox` fields** so
they look at home next to the MudBlazor chrome. Three options were considered.

## Option A — `form-control` (CHOSEN)

Put `class="form-control"` / `class="form-check-input"` on the Blazor inputs.

- These classes are already styled by `ux-tokens.css` (Sections 9–11) from the shared design
  tokens, so the inputs match the app palette / radius / typography.
- **Pros:** robust; renders identically with and without MudBlazor installed; zero new CSS;
  SSR-safe; no dependency on MudBlazor internals.
- **Cons:** not pixel-identical to a real `MudTextField` (no floating-label notch / ripple).
- **Status: in use.** Example (reference page `LoginWith2fa.razor`):
  ```razor
  <label for="two-factor-code" class="form-label">Authenticator code</label>
  <InputText id="two-factor-code" class="form-control" @bind-Value="Input.TwoFactorCode" autocomplete="off" />
  <ValidationMessage For="() => Input.TwoFactorCode" class="text-danger" />
  ```

## Option B — Dedicated `.mud-input-like` style

Author one small token-based "outlined input" rule (e.g. `.mud-input-like`) in
`ux-tokens.css`/controls and apply it to the Blazor inputs.

- **Pros:** closest visual match to MudBlazor's outlined field while staying robust and SSR-safe;
  owned by us, theme-driven.
- **Cons:** one-off CSS to author and maintain; still an approximation of the real component.
- **To adopt:** add the `.mud-input-like` rule, then swap `class="form-control"` →
  `class="mud-input-like"` in the variant `.razor` files.

## Option C — Borrow MudBlazor's own classes

Put MudBlazor's internal classes (`mud-input mud-input-outlined`, `mud-input-slot mud-input-root`,
…) directly on the bare `<input>`.

- **Pros:** closest in name; reuses MudBlazor's stylesheet.
- **Cons:** **fragile** — these classes are MudBlazor-internal and structure-dependent. On a
  class-less `<input>` (no MudBlazor component wrapper) they reproduce only part of the look
  (no label notch / adornment / ripple), so it tends to render partially or oddly, and can break
  across MudBlazor versions. **Not recommended.**
- **To adopt:** swap `class="form-control"` → the relevant `mud-input*` classes in the variant
  `.razor` files (and verify rendering against the installed MudBlazor version).

## Switching later

All three differ only by the `class` on the `InputText`/`InputCheckbox` elements in the
`content/ComponentsMudBlazor/Account/**` variant `.razor` files. The structural pattern
(`EditForm` + `InputBase<T>` + MudBlazor chrome) stays the same regardless of which option is used.
