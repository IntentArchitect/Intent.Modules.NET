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
