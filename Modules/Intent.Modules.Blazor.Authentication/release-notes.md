### Version 2.0.2

- Improvement: Added Authentication selection type to the package, and model out the relevant pages bsed on selection
- Improvement: A number of styling updates to bootstrap pages to align more with Mudblazor styling.
- Improvement: Default stylesheets not overwritten by the software factory.
- Improvement: Added context menu to set the authentication type

### Version 2.0.1

- Improvement: Reduced unnecessary warnings by seeding `[SupplyParameterFromForm]` input models in `OnInitialized (Input ??= new())` instead of a property initializer which should rather be set to `default!`

### Version 2.0.0

- Improvement: Component code-behind and styles are now emitted as separate `razor.cs` and `razor.css` files, allowing improved separation of concerns of C# logic and scoped CSS.

> **NOTE**
>
> This is a breaking change for some components where you previously customized the generated code.
>
> **For `razor.cs`:** if your `@code { }` block was unchanged from what Intent generated, delete the block — Intent will now manage the code-behind in the separate `razor.cs` file. If you had customizations, migrate them into the newly generated `razor.cs` and appropriately use `[IntentIgnore]` to continue managing your own code
>
> **For `razor.css`:** there are no build failures. If your `<style>` block was unchanged from what Intent generated, you can safely delete it. If you had customizations, copy the content into the `*.razor.css` file. Intent will not interfere with your modifications to this file 

- Improvement: Default UI theming
- Fixed: Navbar icons did not work due to coupling with main Blazor modules running `Interactive` modes which is not supported by Blazor's ASP.NET Identity 

### Version 1.1.1

- Improvement: Updated NuGet package versions.

### Version 1.1.0

- Improvement: Updated references from the conversion of Blazor AI to use skills for Page implementation, and align with new AI implementation.

### Version 1.0.7

- Improvement: Updated NuGet package versions.

### Version 1.0.6

- Improvement: Updated NuGet package versions.
- Improvement: Fixed AntiForgery exception when re-logging in.
- Improvement: Refresh token support for WASM / JWT authentication.

### Version 1.0.5

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.4

- Improvement: Updated NuGet package versions.

### Version 1.0.3

- Improvement: Updated NuGet package versions.

### Version 1.0.2

- Improvement: Updated NuGet package versions.
- Fixed: Syntax generation fix.

### Version 1.0.1

- Fixed: De-coupled an optional template.

### Version 1.0.0

- Fixed: No default authentication type was set.
- New Feature: Blazor Authentication Module.
