using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Services.DomainServices
{
    public interface IMyDomainService
    {
        void DoSomething();
    }
}