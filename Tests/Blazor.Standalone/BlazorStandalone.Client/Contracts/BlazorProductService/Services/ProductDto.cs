using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace BlazorStandalone.Client.Contracts.BlazorProductService.Services
{
    public class ProductDto
    {
        public ProductDto()
        {
            Name = null!;
            Ref = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Ref { get; set; }
        public int Qty { get; set; }

        public static ProductDto Create(Guid id, string name, string @ref, int qty)
        {
            return new ProductDto
            {
                Id = id,
                Name = name,
                Ref = @ref,
                Qty = qty
            };
        }
    }
}