using System;
using CleanArchitecture.TestApplication.Domain.Contracts.DomainServices;
using CleanArchitecture.TestApplication.Domain.DomainServices;
using CleanArchitecture.TestApplication.Domain.Entities.DomainServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceImplementation", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Services.DomainServices
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TestDomainService : ITestDomainService
    {
        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public TestDomainService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void ProcessEntity(ConcreteEntityA entity)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void ProcessContract(PassthroughObj obj)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void ProcessValueObject(MoneyVO vo)
        {
            throw new NotImplementedException("Implement your domain service logic here...");
        }
    }
}