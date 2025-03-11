using AspNetControllers.SecuredByDefault.Application.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AspNetControllers.SecuredByDefault.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class TestService : ITestService
    {
        [IntentManaged(Mode.Merge)]
        public TestService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Operation(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation (TestService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}