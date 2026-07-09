using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application
{
    public record ProductDto
    {
        public ProductDto()
        {
            Name = null!;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public decimal Price { get; init; }
        public bool IsActive { get; init; }
    }
}