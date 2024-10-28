using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Entities.DDD;
using FastEndpointsTest.Domain.Repositories.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.AccountHolders.OlderMappingUpdateAccountNote
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class OlderMappingUpdateAccountNoteCommandHandler : IRequestHandler<OlderMappingUpdateAccountNoteCommand, string>
    {
        private readonly IAccountHolderRepository _accountHolderRepository;

        [IntentManaged(Mode.Merge)]
        public OlderMappingUpdateAccountNoteCommandHandler(IAccountHolderRepository accountHolderRepository)
        {
            _accountHolderRepository = accountHolderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(OlderMappingUpdateAccountNoteCommand request, CancellationToken cancellationToken)
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