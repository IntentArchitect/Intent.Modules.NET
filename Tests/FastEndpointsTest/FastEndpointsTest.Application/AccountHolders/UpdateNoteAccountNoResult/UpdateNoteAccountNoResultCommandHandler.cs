using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Repositories.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.AccountHolders.UpdateNoteAccountNoResult
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateNoteAccountNoResultCommandHandler : IRequestHandler<UpdateNoteAccountNoResultCommand>
    {
        private readonly IAccountHolderRepository _accountHolderRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateNoteAccountNoResultCommandHandler(IAccountHolderRepository accountHolderRepository)
        {
            _accountHolderRepository = accountHolderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateNoteAccountNoResultCommand request, CancellationToken cancellationToken)
        {
            var accountHolder = await _accountHolderRepository.FindByIdAsync(request.AccountHolderId, cancellationToken);
            if (accountHolder is null)
            {
                throw new NotFoundException($"Could not find AccountHolder '{request.AccountHolderId}'");
            }

            var account = accountHolder.Accounts.FirstOrDefault(x => x.Id == request.Id);
            if (account is null)
            {
                throw new NotFoundException($"Could not find Account '{request.Id}'");
            }

            var updateNoteResult = account.UpdateNote(request.Note);
        }
    }
}