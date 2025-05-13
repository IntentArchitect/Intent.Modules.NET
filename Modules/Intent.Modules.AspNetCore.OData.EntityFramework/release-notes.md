### Version 1.0.0

- Fixed: Added `[ODataRouteComponent("odata")]` to all ODataControllers.

- Improvement: Now only checks for Entities that have the IHasDomainEvent implemented.

- Fixed: Updated the generated controller names to follow OData naming conventions. This helps prevent conflicts with other similarly named controllers.

Initial Release