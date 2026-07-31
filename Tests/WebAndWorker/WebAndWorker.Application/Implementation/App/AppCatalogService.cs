using Intent.RoslynWeaver.Attributes;
using WebAndWorker.Application.Interfaces.App;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace WebAndWorker.Application.Implementation.App
{
    [IntentManaged(Mode.Merge)]
    public class AppCatalogService : IAppCatalogService
    {
        [IntentManaged(Mode.Merge)]
        public AppCatalogService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> GetAppItems(CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetAppItems (AppCatalogService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}