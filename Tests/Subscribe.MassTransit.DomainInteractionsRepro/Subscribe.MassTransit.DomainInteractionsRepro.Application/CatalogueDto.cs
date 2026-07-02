using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Application
{
    public record CatalogueDto
    {
        public CatalogueDto()
        {
            Name = null!;
            Code = null!;
            CatalogueItems = null!;
        }

        public string Name { get; init; }
        public string Code { get; init; }
        public List<CatalogueItemDto> CatalogueItems { get; init; }
    }
}