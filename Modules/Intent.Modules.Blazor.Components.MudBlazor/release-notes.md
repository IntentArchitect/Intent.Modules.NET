### Version 2.0.4

- Fixed: A customized `Home.razor` could be silently replaced by this module's default home page on a later Software Factory run, and a Home page designed in the User Interface designer could never generate at all. The default is now seeded once, only when `Home.razor` does not exist — delete the file to get it back.

### Version 2.0.3

- Improvement: Entire menu/layout mechanism is now more dynamic and flexible between AI and modeling.
- Improvement: Each bundled AI skill's sample files are no longer overwritten once that skill's own `SKILL.md` has been hand-edited, on the assumption you have taken over maintenance of the whole skill.
- Improvement: Lots of minor bug fixes, improvements and styling tweaks.

### Version 2.0.2

- Fixed: Software Factory crashed wiring up MudBlazor when Blazor components were generated into an application with more than one ASP.NET Core host. MudBlazor registration and CSS now apply to each Blazor host correctly.

### Version 2.0.1

- Improvement: Updated NuGet package versions.
- Improvement: Application home label out of the box now reflect the application name.
- Improvement: Menu items without a link are no longer included in the output.
- Improvement: Navigations (without a related menu item) will not be included in the output.

### Version 2.0.0

- Improvement: Component code-behind and styles are now emitted as separate `razor.cs` and `razor.css` files, allowing improved separation of concerns of C# logic and scoped CSS.

> **NOTE**
>
> This is a breaking change **where you had customized the generated code** — on a normal upgrade the Software Factory reconciles the split for you, so no action is required.
>
> **For `razor.cs`:** if your `@code { }` block was unchanged from what Intent generated, the Software Factory removes the inline block and moves the code into the separate `razor.cs` automatically — nothing to do. **Only if you customized the `@code`** (or took ownership of it with `[IntentIgnore]`) is your inline block preserved, where it then clashes with the generated `razor.cs` (CS0102 / CS0111 duplicate-definition errors); in that case migrate your changes into the `razor.cs`, keep them with `[IntentIgnore]`, and remove the inline block.
>
> **For `razor.css`:** there are no build failures. An unchanged `<style>` block is superseded by the generated `*.razor.css` and can be removed; if you had customizations, copy them into the `*.razor.css` file — Intent will not interfere with your modifications there.

- Improvement: Decouples layout from `Intent.Modules.Blazor.Authentication` which necessarily cannot run in `InteractiveServer` nor `InteractiveWebAssembly` due to ASP.NET identity requiring static `no render` mode

> **NOTE**
>
> As part of this layout refactor the app bar and drawer now inject dedicated `ThemeToggle`, `AppUserMenu` and `NavLinks` components in place of the previously inlined controls. On a normal upgrade the Software Factory reconciles `MainLayout.razor` for you — the old controls are removed and the new components added — so **no action is required**.
>
> **If you have taken ownership of `MainLayout.razor`** — e.g. added an `@Intent.Ignore` instruction or otherwise hand-edited the generated markup so Intent no longer manages it — the merge preserves your version, and you may then see **duplicated controls** in the app bar or drawer (two theme toggles, an old user menu next to the new `AppUserMenu`, or a doubled navigation list).
>
> **In that case:** delete your `MainLayout.razor` and re-run the Software Factory to regenerate it cleanly with only the new components, then re-apply any customizations you want to keep. The file is at `Components/Layout/MainLayout.razor` for single-project (`InteractiveServer`) apps, or `<YourApp>.Client/Components/Layout/MainLayout.razor` for two-project (`InteractiveAuto` / `InteractiveWebAssembly`) apps. (The seeded sample user-menu — a `Menu` element with Profile / My Account / Logout — is removed from your application model by the upgrade migration regardless.)

### Version 1.1.1

- Improvement: Added support for Design.md.
- Improvement: Updated out of the box styling.
- Improvement: Support for light and dark modes.
- Improvement: Refinement of AI context
- Improvement: Ensure @page directive starts with a forward-slash in razor pages

### Version 1.1.0

- Improvement: Updated references from the conversion of Blazor AI to use skills for Page implementation, and align with new AI implementation.

### Version 1.0.8

- Fixed: Minimum client version.

### Version 1.0.7

- Improvement: Updated NuGet package versions.

### Version 1.0.6

- Improvement: Updated NuGet package versions.

### Version 1.0.5

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.4

- Improvement: Updated NuGet package versions.

### Version 1.0.3

- Improvement: Updated NuGet package versions.

### Version 1.0.2

- Improvement: Updated NuGet package versions.

### Version 1.0.1

- Improvement: Updated NuGet package versions.

### Version 1.0.0

- Improvement: Updated NuGet package versions.
- Improvement: Removed MudAppBar Elevation
- Improvement: Updated NuGet package versions.
- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5. 
- Improvement: Updated NuGet package versions.
- Initial release.
- Improvement : Support for sub menu items.
- Improvement : Support for `Layout` attributes on `Containers`.
- Improvement : Support for `OnSelected` event on `Select`.
- Improvement : Support for `Radio Button Group`
- Improvement : Improved `Layout` and `Container` razor generation.
- Improvement : Support for `Button` _Disabled_ binding
- Improvement : Updated `MudDialogInstance` to `IMudDialogInstance`
- Improvement : Updated MudBlazor NuGet Package to 8.3
- Improvement : Appearance options for Grids.
- Improvement : Added docs around Styling.
