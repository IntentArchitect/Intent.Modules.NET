using Intent.Modules.NET.Tests.Application.Core.Common.Eventing;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.Accounts.CreateAccount;
using Intent.Modules.NET.Tests.Module2.Domain.Common.Interfaces;
using Intent.Modules.NET.Tests.Module2.Domain.Entities;
using Intent.Modules.NET.Tests.Module2.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Module2.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Accounts.CreateAccount
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        [IntentManaged(Mode.Merge)]
        public CreateAccountCommandHandler(IAccountRepository accountRepository, IEventBus eventBus, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _eventBus = eventBus;
            _unitOfWork = unitOfWork;
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
            _eventBus.Publish(new AccountCreatedIEEvent
            {
                Id = account.Id,
                Name = account.Name
            });
            return account.Id;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}