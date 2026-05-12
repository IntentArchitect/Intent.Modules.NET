using EventingSubscribers.Application.Common.Eventing;
using EventingSubscribers.Domain.Common.Exceptions;
using EventingSubscribers.Domain.Repositories;
using EventingSubscribers.Eventing.Messages;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventHandler", Version = "1.0")]

namespace EventingSubscribers.Application.IntegrationEvents.EventHandlers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AccountUpgradedEventHandler : IIntegrationEventHandler<AccountUpgradedEvent>
    {
        private readonly IAccountRepository _accountRepository;

        [IntentManaged(Mode.Merge)]
        public AccountUpgradedEventHandler(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task HandleAsync(AccountUpgradedEvent message, CancellationToken cancellationToken = default)
        {
            var updateAccount = await _accountRepository.FindByIdAsync(message.Id, cancellationToken);
            if (updateAccount is null)
            {
                throw new NotFoundException($"Could not find Account '{message.Id}'");
            }

            updateAccount.Tier = message.NewTier;
        }
    }
}