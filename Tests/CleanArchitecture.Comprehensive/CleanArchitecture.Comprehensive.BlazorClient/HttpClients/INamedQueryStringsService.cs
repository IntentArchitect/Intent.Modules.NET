using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients
{
    public interface INamedQueryStringsService : IDisposable
    {
        Task NamedQueryStringsAsync(string par1, CancellationToken cancellationToken = default);
    }
}