using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.CreateQuote
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateQuoteCommandHandler : IRequestHandler<CreateQuoteCommand>
    {
        private readonly IQuoteRepository _quoteRepository;

        [IntentManaged(Mode.Merge)]
        public CreateQuoteCommandHandler(IQuoteRepository quoteRepository)
        {
            _quoteRepository = quoteRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateQuoteCommand request, CancellationToken cancellationToken)
        {
            var quote = new Quote(
                refNo: request.RefNo,
                personId: request.PersonId,
                personEmail: request.PersonEmail);

            quote.NotifyQuoteCreated();

            _quoteRepository.Add(quote);
        }
    }
}