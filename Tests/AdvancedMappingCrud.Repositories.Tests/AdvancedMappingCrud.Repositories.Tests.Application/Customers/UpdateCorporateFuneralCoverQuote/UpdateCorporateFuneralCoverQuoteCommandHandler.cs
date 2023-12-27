using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.UpdateCorporateFuneralCoverQuote
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCorporateFuneralCoverQuoteCommandHandler : IRequestHandler<UpdateCorporateFuneralCoverQuoteCommand>
    {
        private readonly ICorporateFuneralCoverQuoteRepository _corporateFuneralCoverQuoteRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCorporateFuneralCoverQuoteCommandHandler(ICorporateFuneralCoverQuoteRepository corporateFuneralCoverQuoteRepository)
        {
            _corporateFuneralCoverQuoteRepository = corporateFuneralCoverQuoteRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCorporateFuneralCoverQuoteCommand request, CancellationToken cancellationToken)
        {
            var corporateFuneralCoverQuote = await _corporateFuneralCoverQuoteRepository.FindByIdAsync(request.Id, cancellationToken);
            if (corporateFuneralCoverQuote is null)
            {
                throw new NotFoundException($"Could not find CorporateFuneralCoverQuote '{request.Id}'");
            }

            corporateFuneralCoverQuote.Corporate = request.Corporate;
            corporateFuneralCoverQuote.Amount = request.Amount;
            corporateFuneralCoverQuote.RefNo = request.RefNo;
            corporateFuneralCoverQuote.PersonId = request.PersonId;
            corporateFuneralCoverQuote.PersonEmail = request.PersonEmail;
            corporateFuneralCoverQuote.Registration = request.Registration;

            corporateFuneralCoverQuote.NotifyQuoteCreated();
        }
    }
}