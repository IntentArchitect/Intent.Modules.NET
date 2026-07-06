using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Application.GetCatalogueById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCatalogueByIdQueryHandler : IRequestHandler<GetCatalogueByIdQuery, CatalogueDto>
    {
        [IntentManaged(Mode.Merge)]
        public GetCatalogueByIdQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<CatalogueDto> Handle(GetCatalogueByIdQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetCatalogueByIdQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}