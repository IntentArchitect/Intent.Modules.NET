### Version 2.0.0

- Improvement: Component code-behind and styles are now emitted as separate `razor.cs` and `razor.css` files, allowing improved separation of concerns of C# logic and scoped CSS.

> **NOTE**
>
> This is a breaking change for some components where you previously customized the generated code.
>
> **For `razor.cs`:** if your `@code { }` block was unchanged from what Intent generated, delete the block — Intent will now manage the code-behind in the separate `razor.cs` file. If you had customizations, migrate them into the newly generated `razor.cs` and appropriately use `[IntentIgnore]` to continue managing your own code
>
> **For `razor.css`:** there are no build failures. If your `<style>` block was unchanged from what Intent generated, you can safely delete it. If you had customizations, copy the content into the `*.razor.css` file. Intent will not interfere with your modifications to this file 

- Improvement: Decouples layout from `Intent.Modules.Blazor.Authentication` which necessarily cannot run in `InteractiveServer` nor `InteractiveWebAssembly` due to ASP.NET identity requiring static `no render` mode

> **NOTE**
>
> As part of this layout refactor the app bar and drawer now inject dedicated `ThemeToggle`, `AppUserMenu` and `NavLinks` components. `MainLayout.razor` is **merged** on regeneration (it is not fully overwritten, so your customizations are preserved), which means that when you upgrade an **existing** app the Software Factory keeps your previous layout markup and adds the new components on top. You may therefore see **duplicated controls** in the app bar or drawer — e.g. two theme toggles, an old user menu next to the new `AppUserMenu`, or a doubled navigation list.
>
> **To fix:** delete your generated `MainLayout.razor` and re-run the Software Factory — it will be regenerated cleanly with only the new components. The file is at `Components/Layout/MainLayout.razor` for single-project (`InteractiveServer`) apps, or `<YourApp>.Client/Components/Layout/MainLayout.razor` for two-project (`InteractiveAuto` / `InteractiveWebAssembly`) apps. The seeded sample user-menu (a `Menu` element with Profile / My Account / Logout) is removed from your application model by the upgrade migration, so once that has run and you have regenerated `MainLayout.razor`, the old menu no longer renders either.

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
