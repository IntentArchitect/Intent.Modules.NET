using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace FastEndpointsTest.Domain.Services.DDD
{
    public interface IContractOnlyDomainService
    {
        void Operation(string param);
    }
}