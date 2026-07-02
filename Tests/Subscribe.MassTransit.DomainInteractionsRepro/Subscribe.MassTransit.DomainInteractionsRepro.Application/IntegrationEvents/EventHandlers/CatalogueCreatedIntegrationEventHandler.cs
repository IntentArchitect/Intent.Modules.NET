using Intent.RoslynWeaver.Attributes;
using MediatR;
using Subscribe.MassTransit.DomainInteractionsRepro.Application.Common.Eventing;
using Subscribe.MassTransit.DomainInteractionsRepro.Application.GetCatalogueById;
using Subscribe.MassTransit.DomainInteractionsRepro.Domain.Entities;
using Subscribe.MassTransit.DomainInteractionsRepro.Domain.Repositories;
using Subscribe.MassTransit.DomainInteractionsRepro.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CatalogueCreatedIntegrationEventHandler : IIntegrationEventHandler<CatalogueCreatedIntegrationEvent>
    {
        private readonly ISender _mediator;
        private readonly ICatalogueRepository _catalogueRepository;

        [IntentManaged(Mode.Merge)]
        public CatalogueCreatedIntegrationEventHandler(ISender mediator, ICatalogueRepository catalogueRepository)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _catalogueRepository = catalogueRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(
            CatalogueCreatedIntegrationEvent message,
            CancellationToken cancellationToken = default)
        {
            var query = new GetCatalogueByIdQuery(
                id: message.Id);
            var catalogueDto = await _mediator.Send(query, cancellationToken);
            var catalogue = new Catalogue
            {
                Name = catalogueDto.Name,
                Code = catalogueDto.Code,
                CatalogueItems = catalogueDto.CatalogueItems
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