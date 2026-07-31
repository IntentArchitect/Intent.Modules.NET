using Intent.RoslynWeaver.Attributes;
using WebAndWorker.Application.Interfaces.Mobile;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace WebAndWorker.Application.Implementation.Mobile
{
    [IntentManaged(Mode.Merge)]
    public class MobileCatalogService : IMobileCatalogService
    {
        [IntentManaged(Mode.Merge)]
        public MobileCatalogService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> GetMobileItems(CancellationToken cancellationToken = default)
        {
            // TODO: Implement GetMobileItems (MobileCatalogService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}