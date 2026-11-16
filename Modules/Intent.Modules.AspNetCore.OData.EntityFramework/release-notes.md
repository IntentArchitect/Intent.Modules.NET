### Version 1.1.0

- Improvement: Updated NuGet package versions:
  - `Microsoft.AspNetCore.OData` 9.4.1 → 9.5.0 (net8.0); 7.7.9 → 7.8.1 (netstandard2.0)
- Fixed: The Software Factory no longer fails with "More than one instance" when the application has more than one host. OData and its Swagger settings are now configured in each host that has controllers.
- Fixed: The OData document filter is only generated in a host that configures Swashbuckle, so a host using Scalar (or no Swagger) no longer fails to compile, including when another host in the same application uses Swashbuckle.

### Version 1.0.11

- Fixed: Updated for `Intent.Modules.EntityFrameworkCore`'s `DbContextManager.GetDbContext` signature change (new `Default Connection String Name` module setting support).

### Version 1.0.10

- Improvement: Updated to work with Microsoft.OpenApi (2.4.1 and up) library version.

### Version 1.0.9

- Improvement: Updated NuGet package versions.

### Version 1.0.8

- Improvement: Updated module documentation to use centralized documentation site.

### Version 1.0.7

- Improvement: Added support for singular DBSet names

### Version 1.0.6

- Improvement: Updated topic documentation format.

### Version 1.0.5

- Improvement: Updated NuGet package versions.

### Version 1.0.4

- Improvement: Updated NuGet package versions.

### Version 1.0.3

- Improvement: Updated NuGet package versions.

### Version 1.0.2

- Improvement: Updated NuGet package versions.

### Version 1.0.1

- Improvement: Added stereotype descriptions in preperation for Intent Architect 4.5.
- Improvement: Updated NuGet package versions.

### Version 1.0.0

- Improvement: Removed the generated OData routes from swagger.

- Improvement: Added context menu item on aggregates `Expose with OData` as well as limited stereotype usage to only aggregates.

- Fixed: Added `[ODataRouteComponent("odata")]` to all ODataControllers.

- Improvement: Now only checks for Entities that have the IHasDomainEvent implemented.

- Fixed: Updated the generated controller names to follow OData naming conventions. This helps prevent conflicts with other similarly named controllers.

Initial Release
