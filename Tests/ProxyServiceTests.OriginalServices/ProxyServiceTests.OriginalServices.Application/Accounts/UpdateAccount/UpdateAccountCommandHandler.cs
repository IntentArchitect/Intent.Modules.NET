using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.OriginalServices.Domain;
using ProxyServiceTests.OriginalServices.Domain.Common.Exceptions;
using ProxyServiceTests.OriginalServices.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.UpdateAccount
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.FindByIdAsync(request.Id, cancellationToken);
            if (account is null)
            {
                throw new NotFoundException($"Could not find Account '{request.Id}'");
            }

            account.Number = request.Number;
            account.Amount = new Money(
                amount: request.Amount.Amount,
                currency: request.Amount.Currency);
            account.ClientId = request.ClientId;
        }
    }
}