using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateCorporateFuneralCoverQuote
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCorporateFuneralCoverQuoteCommandHandler : IRequestHandler<CreateCorporateFuneralCoverQuoteCommand>
    {
        private readonly ICorporateFuneralCoverQuoteRepository _corporateFuneralCoverQuoteRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCorporateFuneralCoverQuoteCommandHandler(ICorporateFuneralCoverQuoteRepository corporateFuneralCoverQuoteRepository)
        {
            _corporateFuneralCoverQuoteRepository = corporateFuneralCoverQuoteRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateCorporateFuneralCoverQuoteCommand request, CancellationToken cancellationToken)
        {
            var corporateFuneralCoverQuote = new CorporateFuneralCoverQuote(
                refNo: request.RefNo,
                personId: request.PersonId,
                personEmail: request.PersonEmail)
            {
                Corporate = request.Corporate,
                Registration = request.Registration,
                QuoteLines = request.QuoteLines
                    .Select(ql => new QuoteLine(
                        productId: ql.ProductId))
                    .ToList()
            };

            _corporateFuneralCoverQuoteRepository.Add(corporateFuneralCoverQuote);
        }
    }
}