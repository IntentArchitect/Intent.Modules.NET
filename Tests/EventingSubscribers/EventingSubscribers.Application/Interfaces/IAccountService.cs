using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace EventingSubscribers.Application.Interfaces
{
    public interface IAccountService
    {
        Task ProcessUpgradeAsync(CancellationToken cancellationToken = default);
    }
}