using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.ServiceContract", Version = "2.0")]

namespace AspNetControllers.SecuredByDefault.Application.IntegrationServices
{
    public interface ITestService : IDisposable
    {
        Task OperationAsync(CancellationToken cancellationToken = default);
    }
}