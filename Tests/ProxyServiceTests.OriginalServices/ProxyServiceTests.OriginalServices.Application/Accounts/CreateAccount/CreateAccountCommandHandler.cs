using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ProxyServiceTests.OriginalServices.Domain;
using ProxyServiceTests.OriginalServices.Domain.Entities;
using ProxyServiceTests.OriginalServices.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Application.Accounts.CreateAccount
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAccountCommandHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                Number = request.Number,
                Amount = new Money(
                    amount: request.Amount.Amount,
                    currency: request.Amount.Currency),
                ClientId = request.ClientId
            };

            _accountRepository.Add(account);
            await _accountRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return account.Id;
        }
    }
}