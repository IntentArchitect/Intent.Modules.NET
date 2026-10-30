using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.DtoContract", Version = "1.0")]

namespace eShop.Catalog.API.Model;

public record PaginationRequest(
    [property: Description("Number of items to return in a single page of results")]
    [property: DefaultValue(10)]
    int PageSize = 10,

    [property: Description("The index of the page of results to return")]
    [property: DefaultValue(0)]
    int PageIndex = 0
);
