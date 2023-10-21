using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.MessageExtensions", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Messages
{
    public static class EfInvoiceUpdatedEventExtensions
    {
        public static EfInvoiceUpdatedEvent MapToEfInvoiceUpdatedEvent(this EfInvoice projectFrom)
        {
            return new EfInvoiceUpdatedEvent
            {
                Description = projectFrom.Description,
            };
        }
    }
}