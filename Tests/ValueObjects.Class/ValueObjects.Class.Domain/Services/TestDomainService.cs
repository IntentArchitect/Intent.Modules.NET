using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace ValueObjects.Class.Domain.Services
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestDomainService : ITestDomainService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public TestDomainService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void TestOperation(Money amount, Address address)
        {
            // TODO: Implement TestOperation (TestDomainService) functionality
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}