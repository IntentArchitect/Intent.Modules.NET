using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace WebAndWorker.Application.Interfaces.Mobile
{
    public interface IMobileCatalogService
    {
        Task<string> GetMobileItems(CancellationToken cancellationToken = default);
    }
}