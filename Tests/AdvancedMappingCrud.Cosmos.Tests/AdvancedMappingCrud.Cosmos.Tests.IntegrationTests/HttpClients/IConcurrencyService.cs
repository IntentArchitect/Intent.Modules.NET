using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ProxyServiceContract", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.HttpClients
{
    public interface IConcurrencyService : IDisposable
    {
        Task UpdateEntityAfterEtagWasChangedByPreviousOperationTestAsync(CancellationToken cancellationToken = default);
    }
}