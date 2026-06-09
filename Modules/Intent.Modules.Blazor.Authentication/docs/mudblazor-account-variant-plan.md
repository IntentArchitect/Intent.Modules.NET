# MudBlazor Account-Pages Variant — Remediation Plan

## Context

An in-progress change makes the Identity **Account** pages render with MudBlazor
components when the `Intent.Blazor.Components.MudBlazor` module is installed:

- New content set `content/ComponentsMudBlazor/Account/**` — 31 `.razor` files
  re-skinned with `MudCard`/`MudTextField`/`MudButton`/`MudTable`/`MudLink` (no `.cs`).
- New registrations `MudBlazorAccountFolder{Pages,Shared}FilesStaticContentTemplateRegistration`
  publish those `.razor` to `Components/Account/**`.
- The original `AccountFolder{Pages,Shared}` registrations were rewired so that **when
  MudBlazor is installed** they copy **only the `.cs` code-behind** (via a reflection-based
  `RegisterFiltered`), and the plain HTML `.razor` otherwise.

So with MudBlazor on: markup comes from the Mud set, shared code-behind from the plain set.

This plan covers fixing problems **#1, #3, #4**. Problem **#2 is deferred** (see below).

---

## VERIFIED finding (concern #3): MudBlazor inputs do not work on the Account pages

`App.razor.GetRenderModeForPage()` returns **`null` (static SSR) for every `/Account/*` path**
(interactive server mode is used only off `/Account`). Identity requires this for cookie
sign-in + antiforgery + form POST.

Therefore, on the Account pages:

- `MudTextField` / `MudCheckBox` `@bind-Value` and all MudBlazor JS behaviour are **inert**
  (no interactive circuit). Field values can only reach the server through **HTML form POST**
  bound by `[SupplyParameterFromForm]`.
- Form-POST binding is **name-based**: each field must post `name="<FormName-scoped field>"`.
  Blazor's `InputBase<TValue>` components (`InputText`, `InputCheckbox`, …) generate
  FormName-compatible names automatically (per the
  [MS forms-binding docs](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms/binding)).
  `MudTextField` / `MudCheckBox` are **not** `InputBase<T>`, so they don't — and the variant
  supplies no `name`.
- Nuance: `MudBaseInput` exposes `UserAttributes`
  ([docs](https://mudblazor.com/api/MudBaseInput%601)), so splatting `Name="Input.Foo"` *can*
  coerce a single text value into the POST. But that is **not a reliable, general replacement**:
  it does not carry validation integration, FormName scoping, antiforgery conventions, or checkbox
  semantics — `InputCheckbox` emits a paired hidden field so an *unchecked* box still posts `false`,
  whereas `MudCheckBox` posts nothing, silently breaking `bool` binding.
- As written (`@bind-Value`, no `name`), submitted `Input.*` is **empty**. `MudButton
  ButtonType="Submit"` still submits the form, so handlers fire **with empty input**.

**Conclusion:** the Mud variant form pages will *compile* (after the build fixes below) but are
**functionally broken** — they cannot capture user input under static SSR. MudBlazor input
components are incompatible with the static-SSR Identity form-post flow.

---

## Build errors to clear first (RZ9986)

"Component attributes do not support complex content (mixed C# and markup)" — a Blazor
*component* attribute mixed a literal with an `@expression`. Fix each by using a single
interpolated expression `@($"...{x}...")`:

| File | Line | Fix |
|---|---|---|
| `ComponentsMudBlazor/Account/Pages/LoginWith2fa.razor` | 34 | `Href="@($"Account/LoginWithRecoveryCode?ReturnUrl={ReturnUrl}")"` |
| `ComponentsMudBlazor/Account/Pages/Manage/ExternalLogins.razor` | 29 | `Title="@($"Remove this {context.ProviderDisplayName} login from your account")"` |
| `ComponentsMudBlazor/Account/Pages/Manage/ExternalLogins.razor` | 46 | `Title="@($"Log in using your {provider.DisplayName} account")"` |
| `ComponentsMudBlazor/Account/Shared/ExternalLoginPicker.razor` | 23 | `Title="@($"Log in using your {provider.DisplayName} account")"` |

(Re-build after; RZ9986s can mask later ones.)

---

## Fix #1 — remove the fragile reflection-based registration

**Problem:** the rewired registrations resolve content via `GetType().Assembly.Location` +
`Path.Combine("..","content",ContentSubFolder)` and **reflect the private `GetBinaryFiles`**
(`BindingFlags.NonPublic`) purely to filter the copy by file extension. This assumes a dev-tree
layout, depends on a private SDK method, and is likely to break when the module is packaged as a
`.nupkg`. It re-implements `StaticContentTemplateRegistration`.

**Fix:** eliminate the need to extension-filter by **separating content into whole folders**, each
mapped 1:1 to a normal `StaticContentTemplateRegistration` (no reflection, no filtering,
packaging-safe). Recommended layout ("shared code-behind"):

```
content/Account.CodeBehind/**      → *.razor.cs   (always; both modes)
content/Account.Html/**            → *.razor      (when MudBlazor NOT installed)
content/Account.MudBlazor/**       → *.razor      (when MudBlazor installed)   (= today's ComponentsMudBlazor)
```

- 3 registrations, each `ContentSubFolder` = its folder, `<location>` = `Components/Account/...`.
- Gate: all require `IsAspnetcoreIdentity()`; `Account.Html` adds `!mudInstalled`,
  `Account.MudBlazor` adds `mudInstalled`.
- No `Assembly.Location`, no private-method reflection.

**Alternative (zero cross-registration coupling, see #4):** mutually-exclusive *complete* sets —
`Account.Html` and `Account.MudBlazor` each ship `.razor` **and** `.razor.cs`; only one runs.
Trade-off: the code-behind is duplicated across the two folders.

---

## Fix #3 — make the Account forms actually submit under SSR

Given the verified finding, **do not use MudBlazor input components for posted fields**. For every
Account *form* page, keep SSR-post-correct inputs and use MudBlazor for chrome only:

- Posted fields: `<InputText>` / `<InputCheckbox>` / `<InputSelect>` (emit the form-post `name`,
  bind via `[SupplyParameterFromForm]`). Style them with `ux-controls.css` / a MudBlazor-ish class.
- Chrome only (safe in static SSR): `MudCard`, `MudCardContent`, `MudText`, layout, and
  `MudButton ButtonType="Submit"` (renders `<button type=submit>`).
- Also verify the external-login provider buttons still POST `name="Provider" value="…"` — confirm
  MudButton forwards `Name`/`Value` to the underlying `<button>`; if not, keep a plain `<button>`.

**Implication:** for SSR form pages the "MudBlazor variant" reduces to MudBlazor *chrome* around
plain Blazor inputs. If that delta isn't worth a separate file set, the plain-HTML pages styled by
`ux-controls.css` (which already render correctly in both modes) remain the pragmatic default, and
Mud variants are reserved for non-form pages. **Open decision below.**

(The only way to use real `MudTextField` here is to make `/Account` interactive — which changes the
Identity auth model and is out of scope.)

---

## Fix #4 — remove the registration coupling

The current coupling (two registrations writing siblings to one path, one of them via reflection)
is resolved by Fix #1:

- If "shared code-behind" layout: the `.razor`↔`.razor.cs` pairing is ordinary Blazor partial-class
  pairing, not a code smell — but **both** markup variants must target the **same** code-behind
  member surface. Make that contract explicit (the `.razor.cs` owns the members; both `.razor`
  variants use only those) and keep it covered by the build of a reference app in both modes.
- If the zero-coupling alternative (complete sets) is chosen, there is no cross-registration
  coupling at all (at the cost of duplicated `.razor.cs`).

---

## Deferred — #2 (NOT in this pass)

The **RazorBuilder-generated** pages — `Login`, `Register`, `ForgotPassword`, `ResetPassword`,
`ConfirmEmail`, `ResendEmailConfirmation` — have **no** Mud variant and their templates are
unchanged, so with MudBlazor installed they stay plain HTML while the Manage/2FA pages go Mud.
This inconsistency (the most-seen Login/Register pages are the unconverted ones) will be addressed
later.

---

## Open decisions

1. **Registration layout:** shared-code-behind (3 folders, DRY) vs complete-sets (2 folders, zero
   coupling, duplicated `.cs`). Recommend shared-code-behind.
2. **Form pages:** given #3, is a Mud *chrome-only* variant worth maintaining for form pages, or do
   form pages stay plain-HTML + `ux-controls.css` (already works in both modes) and Mud variants are
   limited to non-form/chrome pages?

---

## Verification (acceptance)

Run the software factory and build/launch a generated app in **both** flavours:

1. **Non-MudBlazor + Identity** — Account pages render styled (ux-controls.css) and **forms submit**.
2. **MudBlazor + Identity** — Account pages render with Mud chrome and **forms submit** (post a
   login / change-password and confirm `Input.*` is populated server-side — the specific thing #3
   showed was broken).
3. Confirm no regression to sample pages and to MudBlazor components elsewhere.
