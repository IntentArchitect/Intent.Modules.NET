using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Application
{
    public record CreateCatalogueItemDto
    {
        public CreateCatalogueItemDto()
        {
            Name = null!;
        }

        public string Name { get; init; }
        public int Sequence { get; init; }
    }
}