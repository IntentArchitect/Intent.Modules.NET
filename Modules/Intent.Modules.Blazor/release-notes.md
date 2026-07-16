### Version 2.0.1

- Improvement: Updated NuGet package versions.
- Improvement: Domain package reference automatically added to UI designer.
- Improvement: Modelling context now added from the template, allowing for a consistent experience when not using auto-generated AI tasks.
- Improvement: Having a space in the page name will no longer generate incorrect code.
- Improvement: Component razor files are effectively written "Once off" (except for relevent directives).
- Improvement: Removed weaving from RazorComponent.
- Improvement: Removed sample/example pages being added out the box.
- Fixed: MudDatePicker not rendering label correctly.
- Fixed: Rendering issue for auto complete fields on MS Edge.

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
- Improvement: Domain package reference automatically added to UI designer.

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
