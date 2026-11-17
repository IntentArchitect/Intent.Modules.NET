using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Domain.Guardrails.Contracts.EntityContract", Version = "1.0")]

namespace eShop.Catalog.API.Model;

public class CatalogType
{
    public CatalogType(string type)
    {
        Type = type;
    }

    public int Id { get; set; }

    [Required]
    public string Type { get; set; }
}
