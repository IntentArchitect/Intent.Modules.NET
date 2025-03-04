using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Interfaces
{
    public interface IDummyService
    {
        Task DummyOperation(Guid id, string name, CancellationToken cancellationToken = default);
    }
}