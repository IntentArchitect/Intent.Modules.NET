using EventingSubscribers.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace EventingSubscribers.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class AccountService : IAccountService
    {
        [IntentManaged(Mode.Merge)]
        public AccountService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task ProcessUpgradeAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Implement ProcessUpgradeAsync (AccountService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}