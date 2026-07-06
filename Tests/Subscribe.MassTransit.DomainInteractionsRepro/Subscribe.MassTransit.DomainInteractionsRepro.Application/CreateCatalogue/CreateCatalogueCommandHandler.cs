using Intent.RoslynWeaver.Attributes;
using MediatR;
using Subscribe.MassTransit.DomainInteractionsRepro.Domain.Entities;
using Subscribe.MassTransit.DomainInteractionsRepro.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Application.CreateCatalogue
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCatalogueCommandHandler : IRequestHandler<CreateCatalogueCommand>
    {
        private readonly ICatalogueRepository _catalogueRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCatalogueCommandHandler(ICatalogueRepository catalogueRepository)
        {
            _catalogueRepository = catalogueRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateCatalogueCommand request, CancellationToken cancellationToken)
        {
            var catalogue = new Catalogue
            {
                Name = request.Name,
                Code = request.Code,
                CatalogueItems = request.CatalogueItems
                    .Select(ci => new CatalogueItem
                    {
                        Name = ci.Name,
                        Sequence = ci.Sequence
                    })
                    .ToList()
            };

            _catalogueRepository.Add(catalogue);
        }
    }
}