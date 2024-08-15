using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OutputCachingRedis.Tests.Domain.Entities;
using OutputCachingRedis.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace OutputCachingRedis.Tests.Application.Accounts.CreateAccount
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
                Name = request.Name
            };

            _accountRepository.Add(account);
            await _accountRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return account.Id;
        }
    }
}