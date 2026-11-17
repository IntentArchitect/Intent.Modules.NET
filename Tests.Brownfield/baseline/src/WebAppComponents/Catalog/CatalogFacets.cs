using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.DtoContract", Version = "1.0")]

namespace eShop.WebAppComponents.Catalog;

public record CatalogFacetCount(int Id, int Count);

public record CatalogFacets(
    IReadOnlyList<CatalogFacetCount> BrandCounts,
    IReadOnlyList<CatalogFacetCount> TypeCounts,
    int BrandTotal,
    int TypeTotal);
