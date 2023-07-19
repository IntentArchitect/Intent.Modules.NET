using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients
{
    public interface ISecuredService : IDisposable
    {
        Task SecuredAsync(CancellationToken cancellationToken = default);
    }
}