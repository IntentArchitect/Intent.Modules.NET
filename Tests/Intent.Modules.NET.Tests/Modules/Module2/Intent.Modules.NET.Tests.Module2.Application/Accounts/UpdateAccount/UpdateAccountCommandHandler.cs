using Intent.Modules.NET.Tests.Domain.Core.Common.Exceptions;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.Accounts.UpdateAccount;
using Intent.Modules.NET.Tests.Module2.Domain.Common.Interfaces;
using Intent.Modules.NET.Tests.Module2.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Accounts.UpdateAccount
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        [IntentManaged(Mode.Merge)]
        public UpdateAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.FindByIdAsync(request.Id, cancellationToken);
            if (account is null)
            {
                throw new NotFoundException($"Could not find Account '{request.Id}'");
            }

            account.Name = request.Name;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}