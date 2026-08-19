# Intent.NetTopologySuite

This module introduces geospatial geometry types into your `Domain` and `Services` models. It generates no files of its own; instead it makes geometry types available for use as attribute types and wires up supporting infrastructure (JSON serialization, Swagger examples, structured logging, and Entity Framework Core column mapping) wherever those types are used.

## What is NetTopologySuite?

[NetTopologySuite](https://nettopologysuite.github.io/NetTopologySuite/) is an open-source, spatially-aware .NET library for working with geometric shapes (points, lines, polygons and their multi-part variants) and geospatial operations. It is the .NET ecosystem's standard geometry library and is directly supported by Entity Framework Core's spatial data providers.

## What This Module Generates

This module does not generate any files directly. It contributes:

* Two geometry types — `Point` and `MultiPolygon` — available for selection as an attribute type in the `Domain` and `Services` designers.
* A `GeoDestructureSerilogPolicy` (via `GeoDestructureSerilogPolicyTemplatePartial`) that formats geometry values sensibly when they are logged through Serilog — `Point` gets a concise `Point(x, y)` form, and every other geometry type (including `MultiPolygon`) falls back to its well-known text (WKT) representation. Without this, NetTopologySuite geometries cause circular-reference errors when Serilog tries to log them by default.
* A `GeoJsonSchemaSwaggerFilter` (via `GeoJsonSchemaSwaggerFilterTemplatePartial`) that marks any geometry property's OpenAPI schema with the `geojson` format, clears the schema's reflected C# properties (`Area`, `Centroid`, `NumGeometries`, etc.) in favour of a description explaining that the `coordinates` shape depends on the geometry type, and supplies a realistic coordinate example specifically for `Point`.

## Using Geometry Types

Once installed, `Point` and `MultiPolygon` appear as regular types when setting an attribute's type in the `Domain` designer, alongside primitives like `string` and `int`:

```csharp
public class DeliveryZone
{
    public Guid Id { get; set; }

    public Point DepotLocation { get; set; }

    public MultiPolygon CoverageArea { get; set; }
}
```

Both types resolve to their `NetTopologySuite.Geometries` namespace equivalents, so the generated code compiles directly against the `NetTopologySuite` NuGet package, which this module adds automatically to any project that uses one of these types.

## Entity Framework Core Integration

When an entity attribute uses `Point`, `MultiPolygon`, or any future geometry type from this module, and the application's `Database Provider` setting (from `Intent.EntityFrameworkCore`) is `PostgreSQL`, the generated `EntityTypeConfiguration` maps the column with an explicit PostgreSQL geography column type:

```csharp
builder.Property(x => x.DepotLocation)
    .IsRequired()
    .HasColumnType("geography (point)");

builder.Property(x => x.CoverageArea)
    .IsRequired()
    .HasColumnType("geography (multipolygon)");
```

> [!NOTE]
> This mapping is applied automatically for any type whose `C#` stereotype `Namespace` is `NetTopologySuite.Geometries` — it is not hardcoded to a specific geometry type, so it applies uniformly to `Point`, `MultiPolygon`, and any geometry type added to this module in the future.

## API Serialization and Documentation

When `Intent.AspNetCore.Controllers` is installed, this module registers a `GeoJsonConverterFactory` so geometry properties serialize to and from standard GeoJSON over the API, and adds a Swagger schema filter that marks every geometry property with the `geojson` format and a description of the shape (instead of dumping the geometry's reflected C# properties) — with a realistic coordinate example shown specifically for `Point`.

## Related Modules

### [Intent.EntityFrameworkCore](../../Intent.Modules.EntityFrameworkCore/docs/README.md)

Provides the `Database Provider` setting this module inspects to decide whether to emit PostgreSQL-specific `geography` column mappings, and generates the `EntityTypeConfiguration` classes this module extends.

### [Intent.AspNetCore.Controllers](../../Intent.Modules.AspNetCore.Controllers/docs/README.md)

When installed, this module wires GeoJSON serialization into the generated `Startup`/`Program` files and adds Swagger example generation for geometry properties on generated controllers.

## External Resources

* [NetTopologySuite GitHub](https://github.com/NetTopologySuite/NetTopologySuite)
* [NetTopologySuite.IO.GeoJSON4STJ](https://www.nuget.org/packages/NetTopologySuite.IO.GeoJSON4STJ)
