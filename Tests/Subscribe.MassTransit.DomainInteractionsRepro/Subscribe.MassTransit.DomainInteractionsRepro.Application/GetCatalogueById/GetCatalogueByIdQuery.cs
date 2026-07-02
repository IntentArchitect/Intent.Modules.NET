using Intent.RoslynWeaver.Attributes;
using MediatR;
using Subscribe.MassTransit.DomainInteractionsRepro.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Subscribe.MassTransit.DomainInteractionsRepro.Application.GetCatalogueById
{
    public class GetCatalogueByIdQuery : IRequest<CatalogueDto>, IQuery
    {
        public GetCatalogueByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}