using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AspNetControllers.SecuredByDefault.Application.Interfaces
{
    public interface ITestService
    {
        Task Operation(CancellationToken cancellationToken = default);
    }
}