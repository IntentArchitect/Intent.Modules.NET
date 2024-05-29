using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AccountHolders.UpdateNoteAccountHolder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateNoteAccountHolderCommandHandler : IRequestHandler<UpdateNoteAccountHolderCommand, string>
    {
        private readonly IAccountHolderRepository _accountHolderRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateNoteAccountHolderCommandHandler(IAccountHolderRepository accountHolderRepository)
        {
            _accountHolderRepository = accountHolderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(UpdateNoteAccountHolderCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _accountHolderRepository.FindByIdAsync(request.AccountHolderId, cancellationToken);
            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(AccountHolder)} of Id '{request.AccountHolderId}' could not be found");
            }

            var existingAccount = aggregateRoot.Accounts.FirstOrDefault(p => p.Id == request.Id);
            if (existingAccount is null)
            {
                throw new NotFoundException($"{nameof(Account)} of Id '{request.Id}' could not be found associated with {nameof(AccountHolder)} of Id '{request.AccountHolderId}'");
            }

            var result = existingAccount.UpdateNote(request.Note);
            return result;
        }
    }
}