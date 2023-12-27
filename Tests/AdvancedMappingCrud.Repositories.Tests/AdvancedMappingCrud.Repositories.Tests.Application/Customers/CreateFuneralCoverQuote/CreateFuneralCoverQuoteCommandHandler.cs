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

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateFuneralCoverQuote
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateFuneralCoverQuoteCommandHandler : IRequestHandler<CreateFuneralCoverQuoteCommand>
    {
        private readonly IFuneralCoverQuoteRepository _funeralCoverQuoteRepository;

        [IntentManaged(Mode.Merge)]
        public CreateFuneralCoverQuoteCommandHandler(IFuneralCoverQuoteRepository funeralCoverQuoteRepository)
        {
            _funeralCoverQuoteRepository = funeralCoverQuoteRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateFuneralCoverQuoteCommand request, CancellationToken cancellationToken)
        {
            var funeralCoverQuote = new FuneralCoverQuote(
                refNo: request.RefNo,
                personId: request.PersonId,
                personEmail: request.PersonEmail)
            {
                RefNo = request.RefNo,
                QuoteLines = request.QuoteLines
                    .Select(ql => new QuoteLine(
                        productId: ql.ProductId)
                    {
                        ProductId = ql.ProductId
                    })
                    .ToList()
            };

            funeralCoverQuote.NotifyQuoteCreated();

            _funeralCoverQuoteRepository.Add(funeralCoverQuote);
        }
    }
}