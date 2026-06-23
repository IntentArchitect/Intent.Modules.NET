### Version 2.0.0

- Improvement: Component code-behind and styles are now emitted as separate `razor.cs` and `razor.css` files, allowing cleaner separation of C# logic and scoped CSS.

> **NOTE**
>
> This is a breaking change for any component where you previously customized the generated code.
>
> **For `razor.cs`:** if your `@code { }` block was unchanged from what Intent generated, delete the block — Intent will now manage the code-behind in the separate `razor.cs` file. If you had customizations, migrate them into the newly generated `razor.cs`.
>
> **For `razor.css`:** there are no build failures, however any custom styles should be reconciled with the content of the newly generated `razor.css` file.

- Improvement: Decouples layout from `Intent.Modules.Blazor.Authentication` which necessarily cannot run in `InteractiveServer` nor `InteractiveWebAssembly` due to ASP.NET identity requiring static no render mode

### Version 1.1.1

- Improvement: Added support for Design.md.
- Improvement: Updated out of the box styling.
- Improvement: Support for light and dark modes.

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
