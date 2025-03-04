using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients
{
    public interface IDummyService : IDisposable
    {
        Task DummyOperationAsync(Guid id, string name, CancellationToken cancellationToken = default);
    }
}