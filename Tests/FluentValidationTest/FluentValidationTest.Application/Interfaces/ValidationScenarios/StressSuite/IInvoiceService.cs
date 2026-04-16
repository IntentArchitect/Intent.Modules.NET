using FluentValidationTest.Application.ValidationScenarios.StressSuite;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace FluentValidationTest.Application.Interfaces.ValidationScenarios.StressSuite
{
    public interface IInvoiceService
    {
        Task SubmitInvoice(InvoiceDto dto, CancellationToken cancellationToken = default);
    }
}