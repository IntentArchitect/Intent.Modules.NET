using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Eventing;
using Solace.Tests.Domain.Entities;
using Solace.Tests.Domain.Repositories;
using Solace.Tests.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Solace.Tests.Application.Accounts.CreateAccount
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public CreateAccountCommandHandler(IAccountRepository accountRepository, IEventBus eventBus)
        {
            _accountRepository = accountRepository;
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var account = new Account
            {
                CustomerId = request.CustomerId
            };

            _accountRepository.Add(account);
            await _accountRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            _eventBus.Publish(new AccountCreatedEvent
            {
                Id = account.Id,
                CustomerId = account.CustomerId
            });
            _eventBus.Send(new CreateLedger
            {
                Id = account.Id,
                CustomerId = account.CustomerId
            });
            return account.Id;
        }
    }
}