using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.DomainServices.DomainServiceInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Domain.Services.DefaultDiagram
{
    public interface IDomainServiceWithDefault
    {
        void OperationWithDefault(string param1 = "Operation Param 1 Value");
    }
}