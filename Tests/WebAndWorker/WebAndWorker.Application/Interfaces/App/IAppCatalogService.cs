using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace WebAndWorker.Application.Interfaces.App
{
    public interface IAppCatalogService
    {
        Task<string> GetAppItems(CancellationToken cancellationToken = default);
    }
}