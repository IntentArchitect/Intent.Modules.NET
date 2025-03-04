using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class DummyService : IDummyService
    {
        [IntentManaged(Mode.Merge)]
        public DummyService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task DummyOperation(Guid id, string name, CancellationToken cancellationToken = default)
        {
            // TODO: Implement DummyOperation (DummyService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}