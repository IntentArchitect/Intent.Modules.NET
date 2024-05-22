using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace ValueObjects.Record.Domain.Services
{
    public interface ITestDomainService
    {
        void TestOperation(Money amount, Address address);
    }
}