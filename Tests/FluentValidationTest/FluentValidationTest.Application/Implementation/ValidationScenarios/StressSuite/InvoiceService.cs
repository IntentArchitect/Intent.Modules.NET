using FluentValidationTest.Application.Interfaces.ValidationScenarios.StressSuite;
using FluentValidationTest.Application.ValidationScenarios.StressSuite;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace FluentValidationTest.Application.Implementation.ValidationScenarios.StressSuite
{
    [IntentManaged(Mode.Merge)]
    public class InvoiceService : IInvoiceService
    {
        [IntentManaged(Mode.Merge)]
        public InvoiceService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task SubmitInvoice(InvoiceDto dto, CancellationToken cancellationToken = default)
        {
            // TODO: Implement SubmitInvoice (InvoiceService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }
    }
}