using CleanArchitecture.TestApplication.Domain.Contracts.DomainServices;
using CleanArchitecture.TestApplication.Domain.DomainServices;
using CleanArchitecture.TestApplication.Domain.Entities.DomainServices;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Services.DomainServices
{
    public interface ITestDomainService
    {
        void ProcessEntity(ConcreteEntityA entity);
        void ProcessContract(PassthroughObj obj);
        void ProcessValueObject(MoneyVO vo);
    }
}