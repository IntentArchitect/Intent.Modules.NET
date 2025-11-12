### Version 1.0.6

- Improvement: Updated module documentation to use centralized documentation site.
- Improvement: Improved WASM configuration for debugging.

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
