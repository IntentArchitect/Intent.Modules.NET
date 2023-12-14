using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.Customers;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.ApproveQuote
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ApproveQuoteCommandHandler : IRequestHandler<ApproveQuoteCommand>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IPersonService _personService;

        [IntentManaged(Mode.Merge)]
        public ApproveQuoteCommandHandler(IQuoteRepository quoteRepository, IPersonService personService)
        {
            _quoteRepository = quoteRepository;
            _personService = personService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ApproveQuoteCommand request, CancellationToken cancellationToken)
        {
            var entity = await _quoteRepository.FindByIdAsync(request.QuoteId, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find Quote '{request.QuoteId}'");
            }
            var result = await _personService.GetPersonById(entity.PersonId, cancellationToken);

            entity.PersonEmail = result.Email;
        }
    }
}