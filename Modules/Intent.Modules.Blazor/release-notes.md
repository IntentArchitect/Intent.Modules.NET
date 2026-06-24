### Version 2.0.0

- Improvement: Component code-behind and styles are now emitted as separate `razor.cs` and `razor.css` files, allowing improved separation of concerns of C# logic and scoped CSS.

> **NOTE**
>
> This is a breaking change for some components where you previously customized the generated code.
>
> **For `razor.cs`:** if your `@code { }` block was unchanged from what Intent generated, delete the block — Intent will now manage the code-behind in the separate `razor.cs` file. If you had customizations, migrate them into the newly generated `razor.cs` and appropriately use `[IntentIgnore]` to continue managing your own code
>
> **For `razor.css`:** there are no build failures. If your `<style>` block was unchanged from what Intent generated, you can safely delete it. If you had customizations, copy the content into the `*.razor.css` file. Intent will not interfere with your modifications to this file 

- Improvement: Decouples layout from `Intent.Modules.Blazor.Authentication` which necessarily cannot run in `InteractiveServer` nor `InteractiveWebAssembly` mode due to ASP.NET identity requiring static `no render` mode

> **NOTE**
>
> As part of this layout refactor the app bar and drawer now inject dedicated `ThemeToggle`, `AppUserMenu` and `NavLinks` components. `MainLayout.razor` is **merged** on regeneration (it is not fully overwritten, so your customizations are preserved), which means that when you upgrade an **existing** app the Software Factory keeps your previous layout markup and adds the new components on top. You may therefore see **duplicated controls** in the app bar or drawer — e.g. two theme toggles, an old user menu next to the new `AppUserMenu`, or a doubled navigation list.
>
> **To fix:** delete your generated `MainLayout.razor` and re-run the Software Factory — it will be regenerated cleanly with only the new components. The file is at `Components/Layout/MainLayout.razor` for single-project (`InteractiveServer`) apps, or `<YourApp>.Client/Components/Layout/MainLayout.razor` for two-project (`InteractiveAuto` / `InteractiveWebAssembly`) apps. The seeded sample user-menu (a `Menu` element with Profile / My Account / Logout) is removed from your application model by the upgrade migration, so once that has run and you have regenerated `MainLayout.razor`, the old menu no longer renders either.


### Version 1.1.1

- Improvement: Added support for Design.md.
- Improvement: Updated out of the box styling.
- Improvement: Support for light and dark modes.

### Version 1.1.0

- Improvement: Converted Blazor AI to use skills for Page implementation, and align with new AI implementation.

### Version 1.0.12

- Fixed: Issue where SF would give an exception in certain situations
- Fixed: Issue where route URL was not generated correctly when containing route parameters

### Version 1.0.11

- Fixed: `UseStaticFiles` to be called before `UseRouting` in program.cs.

### Version 1.0.10

- Improvement: Updated NuGet package versions.

### Version 1.0.9

- Fixed: Updated `Intent.Persistence.SDK` to latest version to resolve compatibility issues.

### Version 1.0.8

- Fixed: WASM Client launch profile was not getting created.
- Fixed: Made `NavigateTo` routes relative rather than absolute, these are relative to the RootPath making it better for sub path hosting, and should still function the same for normal hosting .
- Fixed: Fixed an container and host configuration for "Auto".

### Version 1.0.7

- Improvement: Updated NuGet package versions.
- Improvement: Improved WASM configuration for debugging.
- Improvement: Help Topic around routing parameters.
- Fixed: **Software Factory** crashed when you had a service invocation with no mapping.

### Version 1.0.6

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.5

- Improvement: Updated NuGet package versions.
- Improvement: Updated error messaging when Route of a `Page` contains incorrect route parameters.
- Fixed: SF Crash in scenario of remote service with missing mappings.
- Fixed: When newer versions of the `Intent.Code.Weaving.Razor` module was installed, ``<>f__AnonymousDelegate0`2[System.String,System.String]`` would appear on the home page.
- Fixed: Updated duplicate `https` ApplicationUrl entries, to instead have an `https` and `http` entry.

### Version 1.0.4

- Improvement: Updated NuGet package versions.
- Improvement: Navigations between pages will create methods to encapsulate the navigation.
- Fixed: When creating proxy services from services not defined in a folder, error no longer occurs.

### Version 1.0.3

- Fixed: Add type source for `Domain Enums`.

### Version 1.0.2

- Improvement: Updated NuGet package versions.
- Improvement: Added help topics.
- Fixed: Blazor server local service resolved not handling remote services correctly.
- Fixed: Corrected comparison of application Ids when determining the locality of a service invocation.

### Version 1.0.1

- Improvement: Exposed services in the local application prefers direct invocation from the UI rather than going via proxies.

### Version 1.0.0

- Improvement: Updated NuGet package versions.
- Initial release.
