### Version 1.0.4

- New Feature: Added `MultiPolygon` geometry type, usable on Domain and Service attributes the same way as `Point`.
- Improvement: The generated Serilog destructuring policy now applies to any NetTopologySuite geometry type (not just `Point`), preventing circular-reference logging issues.
- Improvement: The generated Swagger/OpenAPI schema filter now applies the `geojson` format to every geometry type, clears the reflected C# properties from the schema, and adds a description explaining that the `coordinates` shape depends on the geometry type — `Point` keeps its own realistic coordinate example.

### Version 1.0.3

- Fixed: Minimum client version.

### Version 1.0.2

- Improvement: Updated to work with Microsoft.OpenApi (2.4.1 and up) library version.

### Version 1.0.1

- Improvement: Removed automatic installation into VS designer

### Version 1.0.0

- Improvement: Updated module NuGet packages infrastructure.
- New Feature: [NetTopologySuite](https://nettopologysuite.github.io/NetTopologySuite/) introduced to work with geospatial coordinates.
