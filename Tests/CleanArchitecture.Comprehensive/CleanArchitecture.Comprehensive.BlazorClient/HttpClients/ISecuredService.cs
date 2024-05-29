using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients
{
    public interface ISecuredService : IDisposable
    {
        Task SecuredAsync(CancellationToken cancellationToken = default);
    }
}