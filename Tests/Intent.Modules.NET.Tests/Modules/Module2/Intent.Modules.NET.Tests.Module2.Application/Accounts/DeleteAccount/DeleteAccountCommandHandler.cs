using Intent.Modules.NET.Tests.Domain.Core.Common.Exceptions;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.Accounts.DeleteAccount;
using Intent.Modules.NET.Tests.Module2.Domain.Common.Interfaces;
using Intent.Modules.NET.Tests.Module2.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Accounts.DeleteAccount
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        [IntentManaged(Mode.Merge)]
        public DeleteAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.FindByIdAsync(request.Id, cancellationToken);
            if (account is null)
            {
                throw new NotFoundException($"Could not find Account '{request.Id}'");
            }

            _accountRepository.Remove(account);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}