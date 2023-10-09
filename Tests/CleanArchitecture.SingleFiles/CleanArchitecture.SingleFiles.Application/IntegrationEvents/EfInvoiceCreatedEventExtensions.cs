using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Messages
{
    public static class EfInvoiceCreatedEventExtensions
    {
        public static EfInvoiceCreatedEvent MapToEfInvoiceCreatedEvent(this EfInvoice projectFrom)
        {
            return new EfInvoiceCreatedEvent
            {
                Description = projectFrom.Description,
            };
        }
    }
}